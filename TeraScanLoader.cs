using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace EMIEMC_Viewer
{
    // TeraScan session file (.tb): camelCase JSON written by TeraScan.Core MeasurementSessionSerializer
    public static class TeraScanLoader
    {
        public const string Magic = "TeraScan";
        public const int MaxKnownFormatVersion = 2;

        public static EmiMeasurement Load(string path)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            var root = serializer.DeserializeObject(File.ReadAllText(path)) as Dictionary<string, object>;

            if (root == null || Str(root, "magic") != Magic)
                throw new InvalidDataException("Not a valid TeraScan session (.tb) file.");

            EmiMeasurement m = new EmiMeasurement { SourceFormat = EmiMeasurement.TeraScan };

            int version = (int)(Num(root, "formatVersion") ?? 1);
            m.Info.Add("TeraScan session format v" + version + " (TeraScan " + Str(root, "appVersion") + ")");
            if (version > MaxKnownFormatVersion)
                m.Info.Add(" - Warning: format newer than v" + MaxKnownFormatVersion + ", reading known fields only");

            DateTimeOffset savedAt;
            if (DateTimeOffset.TryParse(Str(root, "savedAt"), CultureInfo.InvariantCulture, DateTimeStyles.None, out savedAt))
                m.Info.Add("Saved: " + savedAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            m.Info.Add("Instrument: " + Str(root, "instrumentIdentity"));
            if (Str(root, "receiverType") != null)
                m.Info.Add("Receiver: " + Str(root, "receiverType"));

            List<string> transducers = Arr(root, "activeTransducers").Select(t => t as string).Where(t => t != null).ToList();
            if (transducers.Count > 0)
                m.Info.Add("Transducers: " + string.Join(", ", transducers));

            // Traces (legacy files carry a single "trace" instead of "traces")
            List<Dictionary<string, object>> traces = Arr(root, "traces").OfType<Dictionary<string, object>>().ToList();
            if (traces.Count == 0 && Obj(root, "trace") != null)
                traces.Add(Obj(root, "trace"));

            foreach (var t in traces)
            {
                EmiTrace trace = ReadTrace(t);
                m.Traces.Add(trace);
                m.Info.Add("Trace: " + trace.Label + ", " + trace.FreqHz.Length + " points, " + trace.Unit);
            }

            if (m.Traces.Count == 0)
                throw new InvalidDataException("TeraScan session contains no trace.");

            // Limit lines
            foreach (var l in Arr(root, "limitLines").OfType<Dictionary<string, object>>())
            {
                if (!Bool(l, "enabled"))
                    continue;

                double[] fs = Doubles(Arr(l, "frequenciesHz"));
                double[] ls = Doubles(Arr(l, "levels"));
                string name = (Str(l, "name") ?? "").Trim();

                PointF[] pts = new PointF[Math.Min(fs.Length, ls.Length)];
                for (int i = 0; i < pts.Length; i++)
                    pts[i] = new PointF((float)(fs[i] / 1000000), (float)ls[i]);

                if (pts.Length < 2)
                    continue;

                m.Limits.Add(new EmiLimitLine { Name = name, Points = pts, Detector = LimitDetector(name) });
                m.Info.Add("Limit line: " + name);
            }

            // Margin line
            var margin = Obj(root, "marginLine");
            if (margin != null)
            {
                m.MarginOffsetDb = Num(margin, "offsetDb") ?? -6;
                m.MarginLineEnabled = Bool(margin, "enabled");
            }

            // Peak list
            bool judged = false, exceeds = false;

            foreach (var p in Arr(root, "peaks").OfType<Dictionary<string, object>>())
            {
                double level = Num(p, "level") ?? double.NaN;
                double? delta = Num(p, "deltaDb");

                // TeraScan verdict rule (ReceiverShellViewModel.UpdateFmaxVerdict): a row is judged when it has a
                // Max or QP delta; FAIL when any row exceeds (exceedsLimit is cleared by a passing QP final)
                judged |= delta.HasValue || Text(Str(p, "deltaQpText")) != null;
                exceeds |= Bool(p, "exceedsLimit") || Bool(p, "qpExceedsLimit");

                m.Peaks.Add(new EmiPeak
                {
                    FreqHz = Num(p, "frequencyHz") ?? 0,
                    Level = level,
                    LimitDb = delta.HasValue ? level - delta.Value : (double?)null,
                    QpLevel = Num(p, "qpLevel"),
                    Detector = PeakDetector(Str(p, "detText")),
                    Height = Text(Str(p, "heightText")),
                    Angle = Text(Str(p, "angleText")),
                    Pol = Text(Str(p, "polText")),
                });
            }

            if (judged)
            {
                m.Verdict = exceeds ? "FAIL" : "PASS";
                m.Info.Add("TeraScan verdict: " + m.Verdict);
            }

            return m;
        }

        private static EmiTrace ReadTrace(Dictionary<string, object> t)
        {
            double[] fs = Doubles(Arr(t, "frequenciesHz"));
            double[] ls = Doubles(Arr(t, "levels"));
            string rawUnit = Str(t, "unit") ?? "DbuV";

            double offset;
            string unit;

            switch (rawUnit)
            {
                case "DbuVm": unit = "dBµV/m"; offset = 0; break;
                case "DbuV": unit = "dBµV"; offset = 0; break;
                case "Dbm": unit = "dBµV"; offset = 107; break;
                default: throw new InvalidDataException("Trace unit not supported: " + rawUnit);
            }

            List<double> xs = new List<double>();
            List<double> ys = new List<double>();

            for (int i = 0; i < Math.Min(fs.Length, ls.Length); i++)
            {
                if (double.IsNaN(ls[i]) || double.IsInfinity(ls[i]))
                    continue;

                xs.Add(fs[i]);
                ys.Add(ls[i] + offset);
            }

            string detector = Str(t, "detector") ?? "";

            return new EmiTrace
            {
                FreqHz = xs.ToArray(),
                Level = ys.ToArray(),
                Unit = unit,
                Detector = TraceDetector(detector),
                Label = detector + " " + Str(t, "traceMode") + " x" + Num(t, "scanCount"),
            };
        }

        private static Detector TraceDetector(string d)
        {
            switch (d)
            {
                case "QuasiPeak": return Detector.QPeak;
                case "Average":
                case "CisprAverage": return Detector.Average;
                case "PositivePeak":
                case "AutoPeak": return Detector.Maximum;
                default: return Detector.Undefined;
            }
        }

        private static Detector LimitDetector(string name)
        {
            string n = " " + name + " ";
            if (n.Contains(" QP ", StringComparison.OrdinalIgnoreCase) || n.Contains("Quasi", StringComparison.OrdinalIgnoreCase))
                return Detector.QPeak;
            if (n.Contains(" AV ", StringComparison.OrdinalIgnoreCase) || n.Contains(" AVG ", StringComparison.OrdinalIgnoreCase) || n.Contains("Average", StringComparison.OrdinalIgnoreCase))
                return Detector.Average;
            return Detector.Undefined;
        }

        private static Detector PeakDetector(string detText)
        {
            switch (detText)
            {
                case "QP": return Detector.QPeak;
                case "AVG": return Detector.Average;
                default: return Detector.Undefined;
            }
        }

        // TeraScan writes "—" for unmeasured accessory values
        private static string Text(string s)
        {
            return (string.IsNullOrWhiteSpace(s) || s == "—") ? null : s;
        }

        // JSON helpers (JavaScriptSerializer.DeserializeObject: objects = Dictionary, arrays = object[])
        private static Dictionary<string, object> Obj(Dictionary<string, object> d, string key)
        {
            object v;
            return d.TryGetValue(key, out v) ? v as Dictionary<string, object> : null;
        }

        private static IEnumerable<object> Arr(Dictionary<string, object> d, string key)
        {
            object v;
            if (d.TryGetValue(key, out v) && v is IEnumerable && !(v is string))
                return ((IEnumerable)v).Cast<object>();
            return Enumerable.Empty<object>();
        }

        private static string Str(Dictionary<string, object> d, string key)
        {
            object v;
            return d.TryGetValue(key, out v) ? v as string : null;
        }

        private static bool Bool(Dictionary<string, object> d, string key)
        {
            object v;
            return d.TryGetValue(key, out v) && v is bool && (bool)v;
        }

        private static double? Num(Dictionary<string, object> d, string key)
        {
            object v;
            if (!d.TryGetValue(key, out v) || v == null)
                return null;
            return ToDouble(v);
        }

        // Numbers arrive as int/long/decimal/double; NaN/Infinity as "NaN"/"Infinity" strings
        private static double ToDouble(object v)
        {
            return Convert.ToDouble(v, CultureInfo.InvariantCulture);
        }

        private static double[] Doubles(IEnumerable<object> values)
        {
            return values.Select(v => v == null ? double.NaN : ToDouble(v)).ToArray();
        }
    }
}
