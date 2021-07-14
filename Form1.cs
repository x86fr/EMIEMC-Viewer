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


namespace EMIEMC_Viewer
{
    public partial class Form1 : Form
    {
        private string currentfile = @"D:\temp\TD 500 MESH\H500.emcemi";

        public Form1()
        {
            InitializeComponent();
            button1_Click(null, null);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string file in files)
            {
                currentfile = file;
            }


            XmlSerializer xmlSerializer = new XmlSerializer(typeof(RSAPersist));

            FileStream fs = new FileStream(currentfile, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            RSAPersist RSAObj = new RSAPersist();

            try
            {

                RSAObj = (RSAPersist)xmlSerializer.Deserialize(reader);

                textBox1.Clear();
                StatusLabel.Text = "File: " + currentfile;
                AddLog("File Read OK: " + currentfile);

            } catch(Exception ex)
            {
                AddLog("Unable to read file: " + ex.Message);

            }

            fs.Close();


            ProcessGraph(RSAObj);


        }

        private List<Peaks> FindPeaks(RSAPersist EMIObj, DataPointCollection CISPRPt)
        {
            double lowfreq, highfreq, pkrange, currange, curpeakvalue = -200, curpeakfreq = 0;
            int curpeakpoint = 0;
            double PointX, PointY;
            int pos = 0;

            int nbrange = 20;

            var peaklist = new List<Peaks>();

            // Find Freq range for peak detection
            lowfreq = double.Parse(EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.X[0]);
            highfreq = double.Parse(EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.X[EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.X.Count - 1]);
            pkrange = (highfreq - lowfreq) / nbrange;
            currange = lowfreq;

            AddLog("Low Freq: " + lowfreq);
            AddLog("High Freq: " + highfreq);
            AddLog("Peak Seach Range: " + pkrange);

            foreach (string X in EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.X)
            {
                PointX = Double.Parse(X);
                PointY = Double.Parse(EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.Y[pos]);

                if (EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.YUnits == "dBm") 
                    PointY += 107; 

                // Detect peak
                if (PointY > curpeakvalue)
                {
                    curpeakfreq = PointX;
                    curpeakvalue = PointY;
                    curpeakpoint = pos;
                }

                // Add to list at each end of range
                if(PointX >= (currange + pkrange))
                {
                    peaklist.Add(new Peaks(curpeakfreq, curpeakvalue, curpeakpoint));
                    curpeakfreq = 0;
                    curpeakvalue *= 0.75;
                    curpeakpoint = 0;
                    currange += pkrange;
                }

                pos++;
            }

            // Compute Margin against CISPR Limit provided
            foreach(Peaks cpk in peaklist)
            {
                foreach(DataPoint cdp in CISPRPt)
                {
                    if ((cdp.XValue * 1000000) < cpk.freq)
                    {
                        cpk.margin = cdp.YValues[0] - cpk.value;
                    }
                }
            }

            // Sort By Peak Value
            peaklist = peaklist.OrderByDescending(o => o.value).ToList();

            return peaklist;

        }


        private void ProcessGraph(RSAPersist EMIObj)
        {
            int pos = 0, firstpoint;
            double PointX, PointY, logMin;
            bool dbuVm = false;

            // 0 = PASS / 1 = FAIL WITHIN 6 dB / 2 = FAIL
            int status = 0;
            string pkstr = "";

            int pkpoints = 5;

            List<double> xs = new List<double>();

            EMIChart.Series[0].Points.Clear();
            EMIChart.Series[1].Points.Clear();
            EMIChart.Series[2].Points.Clear();

            string YUnits = EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.YUnits;
            string IntYUnits = EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.InternalYUnits;

            AddLog("YUnits: " + YUnits);
            AddLog("InternalYUnits: " + IntYUnits);

            if (YUnits == "dBm") {
                AddLog(" - dBm converted to dBuV/m");
                dbuVm = true;
            }  else if (YUnits != "dBuVPerMeter") {
                AddLog(" Raw Unit not supported!");
                return;
            }

            firstpoint = int.Parse(EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.X[0]);
            logMin = Math.Log10(firstpoint);

            AddLog("First Point: " + firstpoint.ToString() + " Hz");

            EMIChart.ChartAreas[0].AxisX.Minimum = 0;
            EMIChart.ChartAreas[0].AxisX.Maximum = 1000;

            EMIChart.ChartAreas[0].AxisX.IsLogarithmic = true;
            EMIChart.ChartAreas[0].AxisX.LogarithmBase = 10;

            EMIChart.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            EMIChart.ChartAreas[0].AxisX.MajorGrid.Interval = 1;
            EMIChart.ChartAreas[0].AxisX.MajorGrid.IntervalOffset = Math.Ceiling(logMin) - logMin;

            EMIChart.ChartAreas[0].AxisX.MajorTickMark.Enabled = true;
            EMIChart.ChartAreas[0].AxisX.MajorTickMark.Interval = 1;
            EMIChart.ChartAreas[0].AxisX.MajorTickMark.IntervalOffset = EMIChart.ChartAreas[0].AxisX.MajorGrid.IntervalOffset;

            EMIChart.ChartAreas[0].AxisX.MinorGrid.Interval = 1;
            EMIChart.ChartAreas[0].AxisX.MinorGrid.IntervalOffset = Math.Floor(logMin) - logMin;

            EMIChart.ChartAreas[0].AxisX.MinorTickMark.Enabled = false;
            EMIChart.ChartAreas[0].AxisX.MinorTickMark.Interval = 1;
            EMIChart.ChartAreas[0].AxisX.MinorTickMark.IntervalOffset = EMIChart.ChartAreas[0].AxisX.MinorGrid.IntervalOffset;

            EMIChart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
            EMIChart.ChartAreas[0].AxisY.LabelStyle.Format = "#0\" dBµV/m\"";

            EMIChart.ChartAreas[0].AxisX.IsLabelAutoFit = false;


            // Draw Limits
            switch (firstpoint)
            {
                case 1000000000:
                    CISPR32_LimitBox.SelectedItem = "CISPR32 Class B - 1 GHz to 6 GHz (3m)";
                    EMIChart.Series[1].Points.AddXY(1000, 70);
                    EMIChart.Series[1].Points.AddXY(3000, 70);
                    EMIChart.Series[1].Points.AddXY(3000, 74);
                    EMIChart.Series[1].Points.AddXY(6000, 74);

                    // -6dB Limits
                    EMIChart.Series[2].Points.AddXY(1000, 64);
                    EMIChart.Series[2].Points.AddXY(3000, 64);
                    EMIChart.Series[2].Points.AddXY(3000, 66);
                    EMIChart.Series[2].Points.AddXY(6000, 66);

                    // Set Axis
                    xs = new List<double>() { 1000, 2000, 3000, 4000, 5000, 6000 };
                    EMIChart.ChartAreas[0].AxisY.Minimum = 40;
                    EMIChart.ChartAreas[0].AxisY.Maximum = 80;
                    EMIChart.ChartAreas[0].AxisX.Minimum = 1000;
                    EMIChart.ChartAreas[0].AxisX.Maximum = 6000;

                    break;
                case 30000000:
                    CISPR32_LimitBox.SelectedItem = "CISPR32 Class B - 30 MHz to 1 GHz (3m)";
                    EMIChart.Series[1].Points.AddXY(30, 40);
                    EMIChart.Series[1].Points.AddXY(230, 40);
                    EMIChart.Series[1].Points.AddXY(230, 47);
                    EMIChart.Series[1].Points.AddXY(1000, 47);

                    // -6dB Limits
                    EMIChart.Series[2].Points.AddXY(30, 34);
                    EMIChart.Series[2].Points.AddXY(230, 34);
                    EMIChart.Series[2].Points.AddXY(230, 41);
                    EMIChart.Series[2].Points.AddXY(1000, 41);

                    // Set Axis
                    xs = new List<double>() { 30, 50, 100, 200, 300, 500, 1000 };
                    EMIChart.ChartAreas[0].AxisY.Minimum = 0;
                    EMIChart.ChartAreas[0].AxisY.Maximum = 50;
                    EMIChart.ChartAreas[0].AxisX.Minimum = 30;
                    EMIChart.ChartAreas[0].AxisX.Maximum = 1000;
                    break;

                case 150000:
                    CISPR32_LimitBox.SelectedItem = "CISPR32 Class B - 15 kHz to 30 MHz (3m)"; //QPeak
                    EMIChart.Series[1].Points.AddXY(0.15, 66);
                    EMIChart.Series[1].Points.AddXY(0.5, 58);
                    EMIChart.Series[1].Points.AddXY(0.5, 56);
                    EMIChart.Series[1].Points.AddXY(5, 56);
                    EMIChart.Series[1].Points.AddXY(5, 60);
                    EMIChart.Series[1].Points.AddXY(30, 60);

                    // -6dB Limits
                    EMIChart.Series[2].Points.AddXY(0.15, 60);
                    EMIChart.Series[2].Points.AddXY(0.5, 52);
                    EMIChart.Series[2].Points.AddXY(0.5, 50);
                    EMIChart.Series[2].Points.AddXY(5, 50);
                    EMIChart.Series[2].Points.AddXY(5, 54);
                    EMIChart.Series[2].Points.AddXY(30, 54);

                    // Set Axis
                    xs = new List<double>() { 0.15, 0.5, 1, 3, 10, 30 };
                    EMIChart.ChartAreas[0].AxisY.Minimum = 20;
                    EMIChart.ChartAreas[0].AxisY.Maximum = 70;
                    EMIChart.ChartAreas[0].AxisX.Minimum = 0.15;
                    EMIChart.ChartAreas[0].AxisX.Maximum = 30;
                    break;
            }


            // Draw X labels
            double spacer = 0.7d;

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
            
            // Draw Graph
            foreach (string X in EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.X)
            {
                // Draw Point
                PointX = Double.Parse(X) / 1000000;
                PointY = Double.Parse(EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.Y[pos]);
                if(dbuVm) { PointY += 107; }

                pos++;

                if(PointX > 10000 || PointY > 10000)
                    continue; 

                EMIChart.Series[0].Points.AddXY(PointX, PointY);

            }

            // Find Peaks
            List<Peaks> Pk = FindPeaks(EMIObj, EMIChart.Series[1].Points);

            // Draw Annotations
            EMIChart.Annotations.Clear();

            // Add Peak
            for (int pkloop = 0; pkloop < pkpoints; pkloop++)
            {
                pkstr += "Peak #" + (pkloop+1) + " : " + Pk[pkloop].value.ToString("F2") + " dBµV/m at " + (Pk[pkloop].freq / 1000000).ToString("F3") + " MHz (margin: " + Pk[pkloop].margin.ToString("F2") + ")" + Environment.NewLine;

                TextAnnotation TA_Peak = new TextAnnotation();

                if(pkloop == 0)
                    TA_Peak.Text = "Peak #1 (Max)";
                else
                    TA_Peak.Text = "Peak #" + (pkloop + 1);


                if(pkloop == 0 && Pk[pkloop].value > 50 && firstpoint == 30000000)
                {
                    EMIChart.ChartAreas[0].AxisY.Minimum += 10;
                    EMIChart.ChartAreas[0].AxisY.Maximum += 10;
                }

                if (Pk[pkloop].margin < 0)
                    status = 2;
                else if (Pk[pkloop].margin < 6 && status == 0)
                    status = 1;

                TA_Peak.ForeColor = Color.White;
                TA_Peak.SetAnchor(EMIChart.Series[0].Points[Pk[pkloop].pos]);
                EMIChart.Annotations.Add(TA_Peak);
            }

            // Add Peak lists
            TextAnnotation PkListAnnot = new TextAnnotation();
            PkListAnnot.ForeColor = Color.White;
            PkListAnnot.Text = pkstr;
            PkListAnnot.AnchorX = 85;
            PkListAnnot.AnchorY = 15;
            PkListAnnot.Alignment = ContentAlignment.TopLeft;
            EMIChart.Annotations.Add(PkListAnnot);

            // Add Status
            TextAnnotation TA_Status = new TextAnnotation();
            TA_Status.AnchorX = 13;
            TA_Status.AnchorY = 10;

            switch (status)
            {
                case 0:
                    TA_Status.Text = "PASS";
                    TA_Status.ForeColor = Color.LightGreen;
                    break;
                case 1:
                    TA_Status.Text = "FAIL?";
                    TA_Status.ForeColor = Color.Orange;
                    TA_Status.AnchorX += 1;
                    break;
                default:
                    TA_Status.Text = "FAIL";
                    TA_Status.ForeColor = Color.Red;
                    break;
            }

            TA_Status.Font = new Font("Consolas", 20F, FontStyle.Bold);

            EMIChart.Annotations.Add(TA_Status);

            // Add Norm
            TextAnnotation CISPRAnnot = new TextAnnotation();
            CISPRAnnot.ForeColor = Color.White;
            CISPRAnnot.Text = CISPR32_LimitBox.Text;
            CISPRAnnot.AnchorX = 88;
            CISPRAnnot.AnchorY = 92;
            CISPRAnnot.Alignment = ContentAlignment.TopRight;
            CISPRAnnot.AllowMoving = true;
            EMIChart.Annotations.Add(CISPRAnnot);

        }

        private void AddLog(string log, bool noCRLF = false)
        {
            if (noCRLF)
                textBox1.AppendText(log);
            else
                textBox1.AppendText(log + Environment.NewLine);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(RSAPersist));

            FileStream fs = new FileStream(currentfile, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            RSAPersist gpxObj = new RSAPersist();

            try
            {

                gpxObj = (RSAPersist)xmlSerializer.Deserialize(reader);

                //textBox1.Text = gpxObj.Internal.Composite.Items.Composite.Pid;
                //StatusLabel.Text = "File: " + actfile;
                //AddLog("File Read OK: " + actfile);

            }
            catch (Exception ex)
            {
                AddLog("Unable to read file: " + ex.Message);

            }

            fs.Close();

            ProcessGraph(gpxObj);
        }


        private void SaveGraphBtn_Click(object sender, EventArgs e)
        {
            string newfile = currentfile.Replace(".emcemi", " " + DateTime.Now.ToString("yyyyddMMHmmss") + ".png");

            EMIChart.SaveImage(newfile, ChartImageFormat.Png);
        }

        private void SetTitle_Btn_Click(object sender, EventArgs e)
        {
            TextAnnotation TitleAnnot = new TextAnnotation();
            TitleAnnot.ForeColor = Color.White;
            TitleAnnot.Text = GraphTitle.Text;
            TitleAnnot.AnchorX = 52;
            TitleAnnot.AnchorY = 4;
            TitleAnnot.Alignment = ContentAlignment.TopLeft;
            TitleAnnot.Font = new Font("Consolas", 14F, FontStyle.Bold);
            EMIChart.Annotations.Add(TitleAnnot);
        }
    }
}
