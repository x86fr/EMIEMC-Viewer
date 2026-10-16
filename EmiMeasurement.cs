using System;
using System.Collections.Generic;
using System.Drawing;

namespace EMIEMC_Viewer
{
    // Format-neutral measurement, filled by TektronixLoader (.emcemi) or TeraScanLoader (.tb)
    public class EmiMeasurement
    {
        public const string Tektronix = "Tektronix";
        public const string TeraScan = "TeraScan";

        public string SourceFormat { get; set; }
        public List<EmiTrace> Traces { get; set; } = new List<EmiTrace>();

        // Limit lines stored in the file; empty = use the built-in EMILimits table
        public List<EmiLimitLine> Limits { get; set; } = new List<EmiLimitLine>();
        public double MarginOffsetDb { get; set; } = -6;
        public bool MarginLineEnabled { get; set; } = true;

        // Peaks stored in the file; empty = recompute with FindPeaks
        public List<EmiPeak> Peaks { get; set; } = new List<EmiPeak>();

        // Verdict stored in the file ("PASS" / "FAIL"); null = computed by the viewer from the peak margins
        public string Verdict { get; set; }

        // Extra lines for the log window
        public List<string> Info { get; set; } = new List<string>();
    }

    public class EmiTrace
    {
        public double[] FreqHz { get; set; }
        public double[] Level { get; set; }   // dBµV or dBµV/m (dBm already converted)
        public string Unit { get; set; }      // "dBµV" or "dBµV/m"
        public Detector Detector { get; set; } = Detector.Undefined;
        public string Label { get; set; }
    }

    public class EmiLimitLine
    {
        public string Name { get; set; }
        public PointF[] Points { get; set; }  // X in MHz, same convention as IndvLimit.freqLimits
        public Detector Detector { get; set; } = Detector.Undefined;
    }

    public class EmiPeak
    {
        public double FreqHz { get; set; }
        public double Level { get; set; }
        public double? LimitDb { get; set; }
        public double? QpLevel { get; set; }
        public Detector Detector { get; set; } = Detector.Undefined;  // Undefined = applies to any trace
        public string Height { get; set; }
        public string Angle { get; set; }
        public string Pol { get; set; }

        // Limit - measured value (QP final when measured); positive = below limit
        public double? Margin
        {
            get
            {
                if (LimitDb == null) return null;
                return LimitDb.Value - (QpLevel ?? Level);
            }
        }
    }

    public static class LimitMath
    {
        // Limit level at freqMHz, linear between points as drawn on the chart; at a step the lower segment wins.
        // Outside the line (incl. float rounding of the end points, e.g. 0.15f) the end level applies.
        public static double LevelAt(PointF[] points, double freqMHz)
        {
            if (freqMHz <= points[0].X)
                return points[0].Y;
            if (freqMHz >= points[points.Length - 1].X)
                return points[points.Length - 1].Y;

            for (int i = 0; i < points.Length - 1; i++)
            {
                if (freqMHz >= points[i].X && freqMHz <= points[i + 1].X)
                {
                    if (points[i + 1].X == points[i].X)
                        return Math.Min(points[i].Y, points[i + 1].Y);

                    double a = (points[i + 1].Y - points[i].Y) / (points[i + 1].X - points[i].X);
                    return points[i].Y + a * (freqMHz - points[i].X);
                }
            }
            return points[points.Length - 1].Y;
        }
    }
}
