using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms.DataVisualization.Charting;
using System.Reflection;
using System.Globalization;

namespace EMIEMC_Viewer
{
    public partial class Form1 : Form
    {
        IndvLimit CurLim = null;
        EmiMeasurement CurMeasurement = null;     // file shown as Graph #1
        Detector CurDetector = Detector.Undefined; // detector of the Graph #1 trace
        bool UpdatingDetectorBox = false;

        public Form1()
        {
            InitializeComponent();
            EMIChart.Resize += (s, e) => PlacePeakList();

            Version appver = Assembly.GetExecutingAssembly().GetName().Version;
            this.Text += " " + appver.Major.ToString() + '.' + appver.Minor.ToString() + 'b';

        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);


            if (Graph1radioButton.Checked == true)
                Graph1PathBox.Text = files[files.Length - 1];
            else
                Graph2PathBox.Text = files[files.Length - 1];


            Process_EMCEMI_File();

        }


        private void Process_EMCEMI_File()
        {

            string currentfile = (Graph1radioButton.Checked) ? Graph1PathBox.Text : Graph2PathBox.Text;

            EmiMeasurement EMIObj;

            try
            {
                EMIObj = MeasurementLoader.Load(currentfile);

                StatusLabel.Text = "File: " + currentfile;
                AddLog("File Read OK: " + currentfile + " (" + EMIObj.SourceFormat + ")");

                foreach (string info in EMIObj.Info)
                    AddLog(info);
            }
            catch (Exception ex)
            {
                AddLog("Unable to read file: " + ex.Message);
                return;
            }

            if (Graph1radioButton.Checked)
            {
                CurMeasurement = EMIObj;
                ProcessMainGraph(EMIObj);
            }
            else
                AddGraph(EMIObj);

        }


        private List<Peaks> FindPeaks(EmiTrace trace, PointF[] CISPRPt)
        {
            double lowfreq, highfreq, pkrange, currange, curpeakvalue = -200, curpeakfreq = 0;
            int curpeakpoint = 0;
            double PointY;
            int pos = 0;

            int rangeperdecade = 1;

            var peaklist = new List<Peaks>();

            // Find Freq range for peak detection
            lowfreq = trace.FreqHz[0];
            highfreq = trace.FreqHz[trace.FreqHz.Length - 1];
            currange = lowfreq;

            AddLog("Low Freq: " + lowfreq / 1000000 + " MHz");
            AddLog("High Freq: " + highfreq / 1000000 + " MHz");
            //AddLog("Peak Seach Range: " + pkrange / 1000000 + " MHz");

            double[] Xvalues = trace.FreqHz;
            double[] Yvalues = trace.Level;

            pkrange = Math.Pow(10, Math.Floor(Math.Log10(lowfreq))) / rangeperdecade;

            foreach (double PointX in Xvalues)
            {
                PointY = Yvalues[pos];

                // Detect peak
                if (PointY > curpeakvalue)
                {
                    curpeakfreq = PointX;
                    curpeakvalue = PointY;
                    curpeakpoint = pos;
                }


                if (PointX >= (currange + pkrange))
                {
                    AddLog("Find Peaks for range: " + currange / 1000000 + " MHz to " + (currange + pkrange) / 1000000 + " MHz");
                    peaklist.Add(new Peaks(curpeakfreq, curpeakvalue, curpeakpoint));
                    curpeakfreq = 0;
                    curpeakvalue = 0;
                    curpeakpoint = 0;
                    currange += pkrange;
                    pkrange = Math.Pow(10, Math.Floor(Math.Log10(PointX))) / rangeperdecade;
                }

                pos++;
            }

            // Compute Margin against CISPR Limit provided
            foreach(Peaks cpk in peaklist)
                cpk.margin = LimitMath.LevelAt(CISPRPt, cpk.freq / 1000000) - cpk.value;

            // Sort By Peak Value
            peaklist = peaklist.OrderBy(o => o.margin).ToList();

            return peaklist;

        }

        private void AddGraph(EmiMeasurement EMIObj, string seriename = "EMI2")
        {
            // Same detector as Graph1 when the file holds several traces
            EmiTrace trace = EMIObj.Traces.FirstOrDefault(t => t.Detector == CurDetector) ?? EMIObj.Traces[0];

            // If Graph1 not set, cancel
            if (EMIChart.Series["EMI1"].Points.Count < 10)
                return;

            // If Graph1 doesn't have same scale than new Graph, cancel
            if (Math.Abs(EMIChart.Series["EMI1"].Points[0].XValue - trace.FreqHz[0] / 1000000) > 0.000001)
            {
                MessageBox.Show("Both graph don't share the same range", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            EMIChart.Series[seriename].Points.Clear();

            // Draw Graph
            for (int pos = 0; pos < trace.FreqHz.Length; pos++)
                EMIChart.Series[seriename].Points.AddXY(trace.FreqHz[pos] / 1000000, trace.Level[pos]);

            // Add Legend
            EMIChart.Series["EMI2"].IsVisibleInLegend = true;
            EMIChart.Series["EMI2"].LegendText = Graph2PathBox.Text.Substring(Graph2PathBox.Text.LastIndexOf(@"\") + 1); ;

        }

        private void SetAxisLinLog()
        {
            double firstpoint = EMIChart.Series["EMI1"].Points[0].XValue;
            double logMin = Math.Log10(firstpoint);

            if (checkBox_IsLog.Checked)
            {
                EMIChart.ChartAreas[0].AxisX.IsLogarithmic = true;
                EMIChart.ChartAreas[0].AxisX.LogarithmBase = 10;

                EMIChart.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                EMIChart.ChartAreas[0].AxisX.MajorGrid.Interval = 1;
                EMIChart.ChartAreas[0].AxisX.MajorGrid.IntervalOffset = Math.Ceiling(logMin) - logMin;

                EMIChart.ChartAreas[0].AxisX.MajorTickMark.Enabled = true;
                EMIChart.ChartAreas[0].AxisX.MajorTickMark.Interval = 1;
                EMIChart.ChartAreas[0].AxisX.MajorTickMark.IntervalOffset = EMIChart.ChartAreas[0].AxisX.MajorGrid.IntervalOffset;

                EMIChart.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
                EMIChart.ChartAreas[0].AxisX.MinorGrid.Interval = 1;
                EMIChart.ChartAreas[0].AxisX.MinorGrid.IntervalOffset = Math.Floor(logMin) - logMin;

                EMIChart.ChartAreas[0].AxisX.MinorTickMark.Enabled = false;
                EMIChart.ChartAreas[0].AxisX.MinorTickMark.Interval = 1;
                EMIChart.ChartAreas[0].AxisX.MinorTickMark.IntervalOffset = EMIChart.ChartAreas[0].AxisX.MinorGrid.IntervalOffset;

                EMIChart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;

                EMIChart.ChartAreas[0].AxisX.IsLabelAutoFit = false;

                // Define Axis
                EMIChart.ChartAreas[0].AxisX.Minimum = firstpoint;
                EMIChart.ChartAreas[0].AxisX.Maximum = EMIChart.Series["EMI1"].Points[EMIChart.Series["EMI1"].Points.Count - 1].XValue;

                // Draw Custom label
                EMIChart.ChartAreas[0].AxisX.CustomLabels.Clear();
                double spacer = 0.7d;
                List<double> xs = CurLim.AxisLabelLogFreq;

                for (int i = 0; i < xs.Count; i++)
                {
                    CustomLabel cl = new CustomLabel();

                    if (xs[i] == 1 || xs[i] <= 0)
                    {
                        cl.FromPosition = -0.1f;
                        cl.ToPosition = 0.1f;
                    }
                    else
                    {
                        cl.FromPosition = Math.Log10(xs[i] * spacer);
                        cl.ToPosition = Math.Log10(xs[i] / spacer);
                    }

                    if (xs[i] >= 1000)
                        cl.Text = (xs[i] / 1000).ToString("F2") + " GHz";
                    else if (xs[i] < 1)
                        cl.Text = (xs[i] * 1000).ToString("F0") + " kHz";
                    else
                        cl.Text = xs[i] + " MHz";

                    EMIChart.ChartAreas[0].AxisX.CustomLabels.Add(cl);

                }

            }  else
            {
                double MidAx;
                int StdInterval;

                EMIChart.ChartAreas[0].AxisX.IsLogarithmic = false;

                EMIChart.ChartAreas[0].AxisX.Minimum = firstpoint;
                EMIChart.ChartAreas[0].AxisX.Maximum = EMIChart.Series["EMI1"].Points[EMIChart.Series["EMI1"].Points.Count - 1].XValue;

                MidAx = (EMIChart.ChartAreas[0].AxisX.Minimum + EMIChart.ChartAreas[0].AxisX.Maximum) / 2;
                StdInterval = (int)Math.Pow(10, Math.Floor(Math.Log10(MidAx)));

                EMIChart.ChartAreas[0].AxisX.MajorGrid.Interval = StdInterval;
                EMIChart.ChartAreas[0].AxisX.MajorGrid.IntervalOffset = 0 - firstpoint;

                EMIChart.ChartAreas[0].AxisX.MajorTickMark.Interval = StdInterval;
                EMIChart.ChartAreas[0].AxisX.MajorTickMark.IntervalOffset = 0 - firstpoint;

                EMIChart.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
                EMIChart.ChartAreas[0].AxisX.MinorGrid.Interval = StdInterval / 5;
                EMIChart.ChartAreas[0].AxisX.MinorGrid.IntervalOffset = 0 - firstpoint;

                EMIChart.ChartAreas[0].AxisX.MinorTickMark.Enabled = false;

                EMIChart.ChartAreas[0].AxisX.IsStartedFromZero = false;

                EMIChart.ChartAreas[0].AxisX.CustomLabels.Clear();

                EMIChart.ChartAreas[0].AxisX.LabelStyle.Interval = StdInterval;
                EMIChart.ChartAreas[0].AxisX.LabelStyle.IntervalOffset = 0 - firstpoint;

                EMIChart.ChartAreas[0].AxisX.LabelStyle.Format = "#0\" MHz\"";

            }

            SetYAxis();

        }

        private void SetYAxis()
        {
            // Compute Y Max
            double MaxValRAW = EMIChart.Series["EMI1"].Points.FindMaxByValue().YValues[0];
            double MaxLimRAW = EMIChart.Series["Limits1"].Points.FindMaxByValue().YValues[0];

            double MaxVal = Math.Ceiling(MaxValRAW / 10) * 10;
            double MaxLimVal = (Math.Ceiling(MaxLimRAW / 10) * 10);

            if (MaxLimRAW % 10 == 0)
                MaxLimVal += 10;

            if (MaxVal > MaxLimVal)
                EMIChart.ChartAreas[0].AxisY.Maximum = MaxVal;
            else
                EMIChart.ChartAreas[0].AxisY.Maximum = MaxLimVal;

            // Compute Y Min
            if (checkBox_YZero.Checked)
                EMIChart.ChartAreas[0].AxisY.Minimum = 0;
            else
                EMIChart.ChartAreas[0].AxisY.Minimum = Math.Floor(EMIChart.Series["EMI1"].Points.FindMinByValue().YValues[0] / 10) * 10;

            PlacePeakList();
        }

        // Pin the peak list's bottom-right corner to the plot area's bottom-right corner (axis coordinates),
        // so it follows the plot on resize and axis changes instead of a percentage of the whole control
        private void PlacePeakList()
        {
            TextAnnotation PkListAnnot = EMIChart.Annotations.FindByName("PeakList") as TextAnnotation;
            if (PkListAnnot == null || EMIChart.Width == 0 || EMIChart.Height == 0)
                return;

            ChartArea ca = EMIChart.ChartAreas[0];

            PkListAnnot.AxisX = ca.AxisX;
            PkListAnnot.AxisY = ca.AxisY;
            PkListAnnot.AnchorX = ca.AxisX.Maximum;
            PkListAnnot.AnchorY = ca.AxisY.Minimum;
            PkListAnnot.AnchorAlignment = ContentAlignment.BottomRight;

            // Size is a percentage of the chart: the chart's own auto size goes stale on resize (text drifts,
            // wraps or gets cut), so measure the text in pixels against the current chart size
            using (Graphics g = EMIChart.CreateGraphics())
            {
                SizeF size = g.MeasureString(PkListAnnot.Text, PkListAnnot.Font);
                PkListAnnot.Width = (size.Width + 6) * 100 / EMIChart.Width;
                PkListAnnot.Height = (size.Height + 4) * 100 / EMIChart.Height;
            }
        }


        // keepDetector: re-render after a CISPR_DetectorBox change, honour the box instead of the file defaults
        private void ProcessMainGraph(EmiMeasurement EMIObj, bool keepDetector = false, string seriename = "EMI1")
        {
            int firstpoint;

            // 0 = PASS / 1 = FAIL WITHIN 6 dB / 2 = FAIL
            int status = 0;
            string pkstr = "";

            int pkpoints = 6;

            EMILimits EMILimits = new EMILimits();

            EMIChart.Visible = true;
            DragDropLabel.Visible = false;

            EMIChart.Series["EMI1"].Points.Clear();
            EMIChart.Series["EMI2"].Points.Clear();
            EMIChart.Series["Limits1"].Points.Clear();
            EMIChart.Series["Limits2"].Points.Clear();
            Graph2PathBox.Text = "Graph #2";
            Graph1radioButton.Checked = true;

            groupBox_Chart.Enabled = true;
            groupBox_Legends.Enabled = true;
            groupBox_Limits.Enabled = true;
            checkBox_ForceFail.Checked = false;

            // Select trace (TeraScan conducted suite: QP + AVG traces, picked with the detector box)
            EmiTrace trace = EMIObj.Traces[0];
            bool multiTrace = EMIObj.Traces.Count > 1;

            if (multiTrace)
            {
                bool wantQP = keepDetector ? CISPR_DetectorBox.SelectedIndex == 1 : EMIObj.Traces.Any(t => t.Detector == Detector.QPeak);
                trace = EMIObj.Traces.FirstOrDefault(t => t.Detector == (wantQP ? Detector.QPeak : Detector.Average)) ?? EMIObj.Traces[0];
                SetDetectorBox(trace.Detector == Detector.QPeak ? 1 : 0);
                AddLog("Selected trace: " + trace.Label);
            }

            CISPR_DetectorBox.Enabled = multiTrace;
            CurDetector = trace.Detector;

            if (trace.FreqHz.Length < 2)
            {
                AddLog("Trace has no usable points");
                return;
            }

            firstpoint = (int)Math.Round(trace.FreqHz[0]);

            AddLog("First Point: " + firstpoint.ToString() + " Hz");

            // Find Limits

            // Limit line stored in the file (TeraScan): same detector as the trace first
            EmiLimitLine fileLimit = EMIObj.Limits.FirstOrDefault(l => l.Detector == trace.Detector)
                                  ?? EMIObj.Limits.FirstOrDefault(l => l.Detector == Detector.Undefined)
                                  ?? EMIObj.Limits.FirstOrDefault();

            if (fileLimit != null)
            {
                IndvLimit builtin = EMILimits.LimitList.Find(x => x.startfreq == firstpoint);

                CurLim = new IndvLimit()
                {
                    name = fileLimit.Name,
                    unit = trace.Unit,
                    detector = fileLimit.Detector,
                    startfreq = firstpoint,
                    freqLimits = fileLimit.Points,
                    AxisLabelLogFreq = (builtin != null) ? builtin.AxisLabelLogFreq
                                     : LogAxisLabels(trace.FreqHz[0] / 1000000, trace.FreqHz[trace.FreqHz.Length - 1] / 1000000)
                };
            }
            // Special case for Conducted EMI
            else if (firstpoint == 150000)
            {
                bool isQP;

                if (keepDetector)
                    isQP = CISPR_DetectorBox.SelectedIndex == 1;
                else if (EMIObj.SourceFormat == EmiMeasurement.Tektronix)
                    isQP = Graph1PathBox.Text.Contains("QP", StringComparison.OrdinalIgnoreCase);
                else
                    isQP = trace.Detector == Detector.QPeak;

                CurLim = EMILimits.LimitList.Find(x => x.startfreq == firstpoint && x.detector == (isQP ? Detector.QPeak : Detector.Average));
                SetDetectorBox(isQP ? 1 : 0);
                CISPR_DetectorBox.Enabled = true;
            }
            else
            {
                CurLim = EMILimits.LimitList.Find(x => x.startfreq == firstpoint);
            }

            if (CurLim != null) {
                AddLog("Found Limit: " + CurLim.name);
            } else {
                AddLog("Unable to match any limit with Start Point  = " + firstpoint);
                return;
            }

            // Additionnal Graph Settings
            EMIChart.ChartAreas[0].AxisY.LabelStyle.Format = "#0\" " + CurLim.unit + "\"";

            
            // Draw Limits
            foreach (PointF P in CurLim.freqLimits)
            {
                EMIChart.Series["Limits1"].Points.AddXY(P.X, P.Y);
                if (EMIObj.MarginLineEnabled)
                    EMIChart.Series["Limits2"].Points.AddXY(P.X, P.Y + EMIObj.MarginOffsetDb); // -6 dB Limit2
            }

            // Draw Graph
            for (int pos = 0; pos < trace.FreqHz.Length; pos++)
                EMIChart.Series[seriename].Points.AddXY(trace.FreqHz[pos] / 1000000, trace.Level[pos]);

            // Set Axis Lin or Log
            SetAxisLinLog();

            // Find Peaks: TeraScan peak list when stored in the file, otherwise detect them on the trace
            List<EmiPeak> Pk;

            if (EMIObj.Peaks.Count > 0)
            {
                // File order is TeraScan's ranking (worst margin first): keep the same 6 as the viewer's own search
                Pk = EMIObj.Peaks.Where(p => !multiTrace || p.Detector == Detector.Undefined || p.Detector == trace.Detector).ToList();
                AddLog("Peaks from file: " + Pk.Count + (Pk.Count > pkpoints ? " (first " + pkpoints + " shown)" : ""));
                Pk = Pk.Take(pkpoints).ToList();
            }
            else
            {
                Pk = FindPeaks(trace, CurLim.freqLimits).Take(pkpoints)
                        .Select(p => new EmiPeak() { FreqHz = p.freq, Level = p.value, LimitDb = p.value + p.margin }).ToList();
            }

            // Draw Annotations
            EMIChart.Annotations.Clear();

            // Add Peak
            for (int pkloop = 0; pkloop < Pk.Count; pkloop++)
            {
                EmiPeak cpk = Pk[pkloop];
                double? margin = PeakMargin(cpk);

                pkstr += "Peak #" + (pkloop + 1) + " : " + cpk.Level.ToString("F2") + " " + CurLim.unit + " @ " + (cpk.FreqHz / 1000000).ToString("F3") + " MHz";
                if (cpk.QpLevel.HasValue)
                    pkstr += " QP " + cpk.QpLevel.Value.ToString("F2");
                pkstr += " (margin: " + (margin.HasValue ? margin.Value.ToString("F2") : "n/a") + ")";

                string location = string.Join(" ", new[] { cpk.Pol, cpk.Angle, cpk.Height }.Where(s => s != null));
                if (location.Length > 0)
                    pkstr += " " + location;

                pkstr += Environment.NewLine;

                TextAnnotation TA_Peak = new TextAnnotation();
                TA_Peak.ForeColor = Color.White;

                if (pkloop == 0)
                    TA_Peak.Text = "Peak #1 (Max)";
                else
                    TA_Peak.Text = "Peak #" + (pkloop + 1);

                if (margin < 0)
                    status = 2;
                else if (margin < Math.Abs(EMIObj.MarginOffsetDb) && status == 0)
                    status = 1;

                TA_Peak.SetAnchor(EMIChart.Series[seriename].Points[NearestPoint(trace.FreqHz, cpk.FreqHz)]);
                EMIChart.Annotations.Add(TA_Peak);

            }

            // Verdict stored in the TeraScan file wins over the margin rule (whole file, all traces)
            if (EMIObj.Verdict != null)
                status = (EMIObj.Verdict == "FAIL") ? 2 : 0;

            // Add Peak lists
            TextAnnotation PkListAnnot = new TextAnnotation();
            PkListAnnot.Name = "PeakList";
            PkListAnnot.ForeColor = Color.White;
            PkListAnnot.Text = pkstr.TrimEnd();
            PkListAnnot.Alignment = ContentAlignment.TopLeft;
            PkListAnnot.AllowMoving = true;
            PkListAnnot.Font = new Font(new FontFamily("Consolas"), 8f);

            EMIChart.Annotations.Add(PkListAnnot);
            PlacePeakList();

            // Add Status
            RectangleAnnotation TA_Status = new RectangleAnnotation();
            TA_Status.AnchorX = 5;
            TA_Status.AnchorY = 15;
            TA_Status.AllowMoving = true;
            TA_Status.ForeColor = Color.Black;
            TA_Status.Name = "Status";

            switch (status)
            {
                case 0:
                    TA_Status.Text = "PASS";
                    TA_Status.BackColor = Color.MediumSpringGreen;
                    break;
                case 1:
                    TA_Status.Text = "FAIL?";
                    TA_Status.BackColor = Color.Orange;
                    break;
                default:
                    TA_Status.Text = "FAIL";
                    TA_Status.BackColor = Color.Red;
                    break;
            }

            TA_Status.Font = new Font("Consolas", 16F, FontStyle.Bold);

            EMIChart.Annotations.Add(TA_Status);

            // Add Norm
            TextAnnotation CISPRAnnot = new TextAnnotation();

            CISPRAnnot.Text = CurLim.name;
            CISPRAnnot.ForeColor = Color.White;
            CISPRAnnot.SetAnchor(EMIChart.Series["Limits1"].Points[EMIChart.Series["Limits1"].Points.Count - 1]);
            CISPRAnnot.Alignment = ContentAlignment.BottomCenter;
            CISPRAnnot.AnchorAlignment = ContentAlignment.BottomRight;

            EMIChart.Annotations.Add(CISPRAnnot);

            // Add Legend
            EMIChart.Series["EMI1"].LegendText = Graph1PathBox.Text.Substring(Graph1PathBox.Text.LastIndexOf(@"\") + 1);
            LegendBox.Text = EMIChart.Series["EMI1"].LegendText;
            EMIChart.Series["EMI2"].IsVisibleInLegend = false;

            // Set Theme
            SetGraphTheme();
        }

        // Margin against the drawn limit when the file did not store one
        private double? PeakMargin(EmiPeak pk)
        {
            if (pk.Margin.HasValue)
                return pk.Margin;

            return LimitMath.LevelAt(CurLim.freqLimits, pk.FreqHz / 1000000) - (pk.QpLevel ?? pk.Level);
        }

        private static int NearestPoint(double[] freqs, double freq)
        {
            int idx = Array.BinarySearch(freqs, freq);
            if (idx >= 0)
                return idx;

            idx = ~idx;
            if (idx == 0)
                return 0;
            if (idx >= freqs.Length)
                return freqs.Length - 1;

            return (freq - freqs[idx - 1] <= freqs[idx] - freq) ? idx - 1 : idx;
        }

        // Log axis labels (MHz) for a range not covered by the built-in limits: start, 1-3 steps per decade, stop
        private static List<double> LogAxisLabels(double startMHz, double stopMHz)
        {
            List<double> xs = new List<double>() { Math.Round(startMHz, 6) };

            for (double dec = Math.Pow(10, Math.Floor(Math.Log10(startMHz))); dec <= stopMHz; dec *= 10)
            {
                foreach (double k in new double[] { 1, 3 })
                {
                    double v = Math.Round(k * dec, 6);
                    if (v > startMHz * 1.4 && v < stopMHz / 1.4)
                        xs.Add(v);
                }
            }

            xs.Add(Math.Round(stopMHz, 6));
            return xs;
        }

        private void SetDetectorBox(int index)
        {
            UpdatingDetectorBox = true;
            CISPR_DetectorBox.SelectedIndex = index;
            UpdatingDetectorBox = false;
        }

        private void CISPR_DetectorBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UpdatingDetectorBox || CurMeasurement == null || !CISPR_DetectorBox.Enabled)
                return;

            ProcessMainGraph(CurMeasurement, true);
        }

        private void AddLog(string log, bool noCRLF = false)
        {
            if (noCRLF)
                LogBox.AppendText(log);
            else
                LogBox.AppendText(log + Environment.NewLine);
        }

        private void SaveGraphBtn_Click(object sender, EventArgs e)
        {
            string currentfile = (Graph1radioButton.Checked) ? Graph1PathBox.Text : Graph2PathBox.Text;

            // Never derive the name by replacing the extension text: a .tb path would be overwritten by the PNG
            string newfile = Path.Combine(Path.GetDirectoryName(currentfile), Path.GetFileNameWithoutExtension(currentfile) + " " + DateTime.Now.ToString("yyyyddMMHmmss") + ".png");

            EMIChart.SaveImage(newfile, ChartImageFormat.Png);
        }

        private void SetTitle_Btn_Click(object sender, EventArgs e)
        {
            string title = "prout";

            if (InputBox("Set Title", "Enter Graph title:", ref title) != DialogResult.OK)
                return;


            int anotnum = EMIChart.Annotations.IndexOf("GraphTitle");

            if (anotnum > 1)
                EMIChart.Annotations.RemoveAt(anotnum);


            TextAnnotation TitleAnnot = new TextAnnotation();

            TitleAnnot.Name = "GraphTitle";
            TitleAnnot.ForeColor = Color.White;
            TitleAnnot.Text = title;
            TitleAnnot.AnchorX = 52;
            TitleAnnot.AnchorY = 4;
            TitleAnnot.Alignment = ContentAlignment.TopLeft;
            TitleAnnot.Font = new Font("Consolas", 14F, FontStyle.Bold);

            TitleAnnot.AllowTextEditing = true;

            EMIChart.Annotations.Add(TitleAnnot);

        }

        private void SaveGraphBtn2_Click(object sender, EventArgs e)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                EMIChart.SaveImage(ms, ChartImageFormat.Png);
                Bitmap bm = new Bitmap(ms);
                Clipboard.SetImage(bm);
            }
        }

        private void OpenFileBtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (Graph1radioButton.Checked == true)
                    Graph1PathBox.Text = openFileDialog1.FileName;
                else
                    Graph2PathBox.Text = openFileDialog1.FileName;

                Process_EMCEMI_File();
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            EMIChart.Legends[0].Enabled = !checkBox1.Checked;
        }

        private void SetLegend1_Btn_Click(object sender, EventArgs e)
        {
            EMIChart.Series["EMI1"].LegendText = LegendBox.Text;
        }

        private void SetLegend2_Btn_Click(object sender, EventArgs e)
        {
            EMIChart.Series["EMI2"].LegendText = LegendBox.Text;
        }

        private void checkBox_IsLog_CheckedChanged(object sender, EventArgs e)
        {
            SetAxisLinLog();
        }

        private void checkBox_YZero_CheckedChanged(object sender, EventArgs e)
        {
            SetYAxis();
        }

        private void Theme_Box_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetGraphTheme();
        }

        private void SetGraphTheme()
        {
            switch(Theme_Box.SelectedItem)
            {
                default:
                case "Dark":
                    Theme_Box.SelectedItem = "Dark";
                    EMIChart.ChartAreas[0].BackColor = Color.Black;
                    EMIChart.BackColor = Color.Black;

                    EMIChart.Series["EMI1"].BorderColor = Color.White;
                    EMIChart.Series["EMI1"].Color = Color.Yellow;
                    EMIChart.Series["EMI1"].LabelForeColor = Color.White;

                    EMIChart.Series["EMI2"].Color = Color.Aqua;

                    EMIChart.Series["Limits1"].Color = Color.Salmon;
                    EMIChart.Series["Limits2"].Color = Color.Salmon;

                    EMIChart.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
                    EMIChart.ChartAreas[0].AxisX.LineColor = Color.White;
                    EMIChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gray;
                    EMIChart.ChartAreas[0].AxisX.MinorGrid.LineColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                    EMIChart.ChartAreas[0].AxisX.MinorTickMark.LineColor = Color.White; 
                    EMIChart.ChartAreas[0].AxisX.MajorTickMark.LineColor = Color.White;
                    EMIChart.ChartAreas[0].AxisX.TitleForeColor = Color.White;
                    EMIChart.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
                    EMIChart.ChartAreas[0].AxisY.LineColor = Color.White;
                    EMIChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                    EMIChart.ChartAreas[0].AxisY.MajorTickMark.LineColor = Color.White;

                    EMIChart.ChartAreas[0].AxisY.TitleForeColor = Color.White;

                    EMIChart.Legends[0].BackColor = Color.Black;
                    EMIChart.Legends[0].BorderColor = Color.Gainsboro;
                    EMIChart.Legends[0].ForeColor = Color.White;
                    EMIChart.Legends[0].InterlacedRowsColor = Color.White;
                    EMIChart.Legends[0].TitleForeColor = Color.White;

                    foreach (Annotation A in EMIChart.Annotations)
                    {
                        if(A.Name != "Status")
                            A.ForeColor = Color.White;
                    }

                    break;
                case "Light":
                    EMIChart.ChartAreas[0].BackColor = Color.White;
                    EMIChart.BackColor = Color.White;

                    EMIChart.Series["EMI1"].BorderColor = Color.White;
                    EMIChart.Series["EMI1"].Color = Color.DeepSkyBlue;
                    EMIChart.Series["EMI1"].LabelForeColor = Color.Black;

                    EMIChart.Series["EMI2"].Color = Color.SkyBlue;

                    EMIChart.Series["Limits1"].Color = Color.Red;
                    EMIChart.Series["Limits2"].Color = Color.Red;

                    EMIChart.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Black;
                    EMIChart.ChartAreas[0].AxisX.LineColor = Color.Black;
                    EMIChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gray;
                    EMIChart.ChartAreas[0].AxisX.MinorGrid.LineColor = Color.LightGray;
                    EMIChart.ChartAreas[0].AxisX.MinorTickMark.LineColor = Color.Black;
                    EMIChart.ChartAreas[0].AxisX.MajorTickMark.LineColor = Color.Black;
                    EMIChart.ChartAreas[0].AxisX.TitleForeColor = Color.Black;
                    EMIChart.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Black;
                    EMIChart.ChartAreas[0].AxisY.LineColor = Color.Black;
                    EMIChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.DarkGray;
                    EMIChart.ChartAreas[0].AxisY.MajorTickMark.LineColor = Color.Black;

                    EMIChart.ChartAreas[0].AxisY.TitleForeColor = Color.Black;

                    EMIChart.Legends[0].BackColor = Color.White;
                    EMIChart.Legends[0].BorderColor = Color.Gainsboro;
                    EMIChart.Legends[0].ForeColor = Color.Black;
                    EMIChart.Legends[0].InterlacedRowsColor = Color.Black;
                    EMIChart.Legends[0].TitleForeColor = Color.Black;

                    foreach (Annotation A in EMIChart.Annotations)
                    {
                        if (A.Name != "Status")
                            A.ForeColor = Color.Black;
                    }

                    break;


            }


        }

        // Input Box Functions
        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;

            return dialogResult;
        }

        private void checkBox_ForceFail_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox_ForceFail.Checked)
            {
                (EMIChart.Annotations["Status"] as TextAnnotation).Text = "FAIL";
                EMIChart.Annotations["Status"].BackColor = Color.Red;
            }
        }
    }
}
