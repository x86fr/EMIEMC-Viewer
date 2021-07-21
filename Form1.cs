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

namespace EMIEMC_Viewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            Version appver = Assembly.GetExecutingAssembly().GetName().Version;
            this.Text += " " + appver.Major.ToString() + '.' + appver.Minor.ToString() + 'b';

            Graph1PathBox.Text = @"D:\temp\TD 500 MESH\H500.emcemi";
            Process_EMCEMI_File();
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

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(RSAPersist));
            FileStream fs = new FileStream(currentfile, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            RSAPersist RSAObj = new RSAPersist();

            try
            {

                RSAObj = (RSAPersist)xmlSerializer.Deserialize(reader);

                StatusLabel.Text = "File: " + currentfile;
                AddLog("File Read OK: " + currentfile);

            }
            catch (Exception ex)
            {
                AddLog("Unable to read file: " + ex.Message);

            }

            fs.Close();

            if (Graph1radioButton.Checked)
                ProcessMainGraph(RSAObj);
            else
                AddGraph(RSAObj);

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
                    curpeakvalue = 0;
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
            peaklist = peaklist.OrderBy(o => o.margin).ToList();

            return peaklist;

        }

        private void AddGraph(RSAPersist EMIObj, string seriename = "EMI2")
        {
            int pos = 0;
            double PointX, PointY;
            string YUnits = EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.YUnits;
            bool dbuVm = (YUnits == "dBm") ? true : false;

            // If Graph1 not set, cancel
            if (EMIChart.Series["EMI1"].Points.Count < 10)
                return;

            // If Graph1 doesn't have same scale than new Graph, cancel
            if((EMIChart.Series["EMI1"].Points[0].XValue * 1000000) != int.Parse(EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.X[0]))
            {
                MessageBox.Show("Both graph don't share the same range", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            EMIChart.Series[seriename].Points.Clear();

            // Draw Graph
            foreach (string X in EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.X)
            {
                // Draw Point
                PointX = Double.Parse(X) / 1000000;
                PointY = Double.Parse(EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.Y[pos]);
                if (dbuVm) { PointY += 107; }

                pos++;

                if (PointX > 10000 || PointY > 10000)
                    continue;

                EMIChart.Series[seriename].Points.AddXY(PointX, PointY);

            }

            // Add Legend
            EMIChart.Series["EMI2"].IsVisibleInLegend = true;
            EMIChart.Series["EMI2"].LegendText = Graph2PathBox.Text.Substring(Graph2PathBox.Text.LastIndexOf(@"\") + 1); ;

        }


        private void ProcessMainGraph(RSAPersist EMIObj, string seriename = "EMI1")
        {
            int pos = 0, firstpoint;
            double PointX, PointY, logMin;
            bool dbuVm = false;

            // 0 = PASS / 1 = FAIL WITHIN 6 dB / 2 = FAIL
            int status = 0;
            string pkstr = "";

            int pkpoints = 5;

            List<double> xs = new List<double>();

            EMILimits EMILimits = new EMILimits();

            EMIChart.Visible = true;

            EMIChart.Series["EMI1"].Points.Clear();
            EMIChart.Series["EMI2"].Points.Clear();
            EMIChart.Series["Limits1"].Points.Clear();
            EMIChart.Series["Limits2"].Points.Clear();
            Graph2PathBox.Text = "Graph #2";
            Graph1radioButton.Checked = true;

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

            // Find Limits
            IndvLimit CurLim = null;

            // Special case for Conducted EMI
            if (firstpoint == 150000)
            {
                if (Graph1PathBox.Text.Contains("QP", StringComparison.OrdinalIgnoreCase))
                {
                    CurLim = EMILimits.LimitList.Find(x => x.startfreq == firstpoint && x.detector == Detector.QPeak);
                    CISPR_DetectorBox.SelectedIndex = 1;
                }
                else
                {
                    CurLim = EMILimits.LimitList.Find(x => x.startfreq == firstpoint && x.detector == Detector.Average);
                    CISPR_DetectorBox.SelectedIndex = 0;
                }
                CISPR_DetectorBox.Enabled = true;
            }
            else
            {
                CurLim = EMILimits.LimitList.Find(x => x.startfreq == firstpoint);
                CISPR_DetectorBox.Enabled = false;
            }

            if (CurLim != null) {
                AddLog("Found Limit: " + CurLim.name);
            } else {
                AddLog("Unable to match any limit with Start Point  = " + firstpoint);
                return;
            }


            // Draw Limits
            foreach (PointF P in CurLim.freqLimits)
            {
                EMIChart.Series["Limits1"].Points.AddXY(P.X, P.Y);
                EMIChart.Series["Limits2"].Points.AddXY(P.X, P.Y - 6); // -6 dB Limit2
            }

            // Define Axis
            xs = CurLim.AxisFreq;
            EMIChart.ChartAreas[0].AxisX.Minimum = CurLim.AxisXMin;
            EMIChart.ChartAreas[0].AxisX.Maximum = CurLim.AxisXMax;
            EMIChart.ChartAreas[0].AxisY.Minimum = CurLim.AxisYMin;
            EMIChart.ChartAreas[0].AxisY.Maximum = CurLim.AxisYMax;

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

                EMIChart.Series[seriename].Points.AddXY(PointX, PointY);

            }

            // Find Peaks
            List<Peaks> Pk = FindPeaks(EMIObj, EMIChart.Series["Limits1"].Points);

            // Draw Annotations
            EMIChart.Annotations.Clear();

            // Add Peak
            for (int pkloop = 0; pkloop < pkpoints; pkloop++)
            {
                pkstr += "Peak #" + (pkloop+1) + " : " + Pk[pkloop].value.ToString("F2") + " dBµV/m @ " + (Pk[pkloop].freq / 1000000).ToString("F3") + " MHz (margin: " + Pk[pkloop].margin.ToString("F2") + ")" + Environment.NewLine;

                TextAnnotation TA_Peak = new TextAnnotation();
                TA_Peak.ForeColor = Color.White;

                if (pkloop == 0)
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

                TA_Peak.SetAnchor(EMIChart.Series[seriename].Points[Pk[pkloop].pos]);
                EMIChart.Annotations.Add(TA_Peak);

            }

            // Add Peak lists
            TextAnnotation PkListAnnot = new TextAnnotation();
            PkListAnnot.ForeColor = Color.White;
            PkListAnnot.Text = pkstr;
            PkListAnnot.AnchorX = 86;
            PkListAnnot.AnchorY = 92;
            PkListAnnot.Alignment = ContentAlignment.TopLeft;
            PkListAnnot.AllowMoving = true;

            EMIChart.Annotations.Add(PkListAnnot);

            // Add Status
            RectangleAnnotation TA_Status = new RectangleAnnotation();
            TA_Status.AnchorX = 5;
            TA_Status.AnchorY = 15;
            TA_Status.AllowMoving = true;
            TA_Status.ForeColor = Color.Black;

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
        }

        private void AddLog(string log, bool noCRLF = false)
        {
            if (noCRLF)
                textBox1.AppendText(log);
            else
                textBox1.AppendText(log + Environment.NewLine);
        }

        private void SaveGraphBtn_Click(object sender, EventArgs e)
        {
            string currentfile = (Graph1radioButton.Checked) ? Graph1PathBox.Text : Graph2PathBox.Text;

            string newfile = currentfile.Replace(".emcemi", " " + DateTime.Now.ToString("yyyyddMMHmmss") + ".png");

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
            }

            Process_EMCEMI_File();
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

    }
}
