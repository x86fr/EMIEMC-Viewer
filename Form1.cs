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
using ObjectDumping.Internal;

namespace EMIEMC_Viewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string actfile = "";
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string file in files)
            {
                actfile = file;
            }


            XmlSerializer xmlSerializer = new XmlSerializer(typeof(RSAPersist));

            FileStream fs = new FileStream(actfile, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            RSAPersist gpxObj = new RSAPersist();

            try
            {

                gpxObj = (RSAPersist)xmlSerializer.Deserialize(reader);

                //textBox1.Text = gpxObj.Internal.Composite.Items.Composite.Pid;
                StatusLabel.Text = "File OK: " + actfile;
                //textBox1.Text = ObjectDumper.Dump(gpxObj);

            } catch(Exception ex)
            {
                StatusLabel.Text = "Unable to read file: " + ex.Message;

            }

            fs.Close();


            ProcessGraph(gpxObj);     

        }

        private void ProcessGraph(RSAPersist EMIObj)
        {
            int pos = 0, firstpoint = 0;
            double PointX, PointY;
            bool dbuVm = false;

            chart1.Series[0].Points.Clear();

            string YUnits = EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.YUnits;
            string IntYUnits = EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.InternalYUnits;

            textBox1.Text = "YUnits: " + YUnits + Environment.NewLine;
            AddLog("InternalYUnits: " + IntYUnits);

            if (YUnits == "dBuVPerMeter") {
                AddLog(" - dBuV/m converted to dBm");
                dbuVm = true;
            }  else if (YUnits != "dBm") {
                AddLog(" Raw Unit not supported!");
                return;
            }

            firstpoint = int.Parse(EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.X[0]);

            AddLog("First Point: " + firstpoint.ToString() + " Hz");

            chart1.ChartAreas[0].AxisX.IsLogarithmic = true;

            if (firstpoint == 30000000)
            {
                chart1.ChartAreas[0].AxisY.Minimum = -90;
                chart1.ChartAreas[0].AxisY.Maximum = -40;
                chart1.ChartAreas[0].AxisX.Minimum = 30;
                chart1.ChartAreas[0].AxisX.Maximum = 1000;
                chart1.ChartAreas[0].AxisX.Interval = 0.1;
            }

            foreach (string X in EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.X)
            {
                PointX = Double.Parse(X) / 1000000;
                PointY = Double.Parse(EMIObj.Internal.Composite.Items.Composite[1].Items.Waveform.Y[pos]);
                if(dbuVm) { PointY -= 95.2; }
                chart1.Series[0].Points.AddXY(PointX, PointY);
                pos++;
            }



            //textBox1.AppendText(Bigstring);

        }

        private void AddLog(string log, bool noCRLF = false)
        {
            if (noCRLF)
                textBox1.AppendText(log);
            else
                textBox1.AppendText(log + Environment.NewLine);
        }

    }
}
