using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace EMIEMC_Viewer
{
    public enum Detector
    {
        Undefined,
        Maximum,
        Average,
        QPeak
    }

    class EMILimits
    {

        public static List<IndvLimit> LimitList = new List<IndvLimit>
        {
            // CISPR 32 Class B Conducted (Max QuasiPeak)
            new IndvLimit() {   name="CISPR 32 Class B - 150 kHz to 30 MHz - QP", startfreq = 150000, unit = "dBµV",
                                AxisXMin = 0.15, AxisXMax = 30, AxisYMin = 20, AxisYMax = 70, detector = Detector.QPeak,
                                AxisFreq = new List<double>() { 0.15, 0.5, 1, 3, 10, 30 },
                                freqLimits = new PointF[] { new PointF(0.15f, 66),
                                                            new PointF(0.5f, 56),
                                                            new PointF(5, 56),
                                                            new PointF(5, 60),
                                                            new PointF(30, 60) }
                            },
            // CISPR 32 Class B Conducted (Max Average)
            new IndvLimit() {   name="CISPR 32 Class B - 150 kHz to 30 MHz - AVG", startfreq = 150000, unit = "dBµV",
                                AxisXMin = 0.15, AxisXMax = 30, AxisYMin = 20, AxisYMax = 60, detector = Detector.Average,
                                AxisFreq = new List<double>() { 0.15, 0.5, 1, 3, 10, 30 },
                                freqLimits = new PointF[] { new PointF(0.15f, 56),
                                                            new PointF(0.5f, 46),
                                                            new PointF(5, 46),
                                                            new PointF(5, 50),
                                                            new PointF(30, 50) }
                            },

            // CISPR 32 Class B Radiated
            new IndvLimit() {   name="CISPR 32 Class B - 30 MHz to 1 GHz (3m)", startfreq = 30000000, unit = "dBµV/m",
                                AxisXMin = 30, AxisXMax = 1000, AxisYMin = 0, AxisYMax = 50,
                                AxisFreq = new List<double>() { 30, 50, 100, 200, 300, 500, 1000 },
                                freqLimits = new PointF[] { new PointF(30, 40),
                                                            new PointF(230, 40),
                                                            new PointF(230, 47),
                                                            new PointF(1000, 47) }
                            },

            // CISPR 32 Class B Radiated
            new IndvLimit() {   name="CISPR 32 Class B - 1 GHz to 6 GHz (3m)", startfreq = 1000000000, unit = "dBµV/m",
                                AxisXMin = 1000, AxisXMax = 6000, AxisYMin = 40, AxisYMax = 80,
                                AxisFreq = new List<double>() { 1000, 2000, 3000, 4000, 5000, 6000 },
                                freqLimits = new PointF[] { new PointF(1000, 70),
                                                            new PointF(3000, 70),
                                                            new PointF(3000, 74),
                                                            new PointF(6000, 74) }
                            }
        };
    }

    public class IndvLimit
    {
        public string name { get; set; }
        public string unit { get; set; }
        public Detector detector { get; set; }
        public int startfreq { get; set; }
        public PointF[] freqLimits { get; set; }
        public double AxisXMin { get; set; }
        public double AxisXMax { get; set; }
        public double AxisYMin { get; set; }
        public double AxisYMax { get; set; }
        public List<double> AxisFreq { get; set; }

    }


}
