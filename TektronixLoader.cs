using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace EMIEMC_Viewer
{
    // Tektronix SignalVu-PC EMC-EMI file (.emcemi, RSAPersist XML)
    public static class TektronixLoader
    {
        public static EmiMeasurement Load(string path)
        {
            RSAPersist RSAObj;

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (XmlReader reader = XmlReader.Create(fs))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(RSAPersist));
                RSAObj = (RSAPersist)xmlSerializer.Deserialize(reader);
            }

            Waveform wf = RSAObj.Internal.Composite.Items.Composite[1].Items.Waveform;

            EmiMeasurement m = new EmiMeasurement { SourceFormat = EmiMeasurement.Tektronix };
            m.Info.Add("YUnits: " + wf.YUnits);
            m.Info.Add("InternalYUnits: " + wf.InternalYUnits);

            bool dbuVm = false;
            string unit;

            if (wf.YUnits == "dBm")
            {
                m.Info.Add(" - dBm converted to dBuV/m");
                dbuVm = true;
                unit = "dBµV/m";
            }
            else if (wf.YUnits == "dBuVPerMeter")
                unit = "dBµV/m";
            else if (wf.YUnits == "dBuV")
                unit = "dBµV";
            else
                throw new InvalidDataException("Raw Unit not supported: " + wf.YUnits);

            List<double> xs = new List<double>();
            List<double> ys = new List<double>();

            for (int pos = 0; pos < wf.X.Count; pos++)
            {
                double PointX = Double.Parse(wf.X[pos], CultureInfo.InvariantCulture);
                double PointY = Double.Parse(wf.Y[pos], CultureInfo.InvariantCulture);
                if (dbuVm) { PointY += 107; }

                if (PointX / 1000000 > 10000 || PointY > 10000)
                    continue;

                xs.Add(PointX);
                ys.Add(PointY);
            }

            m.Traces.Add(new EmiTrace { FreqHz = xs.ToArray(), Level = ys.ToArray(), Unit = unit, Label = "Composite" });

            return m;
        }
    }
}
