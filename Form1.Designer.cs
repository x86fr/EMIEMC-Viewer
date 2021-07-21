﻿namespace EMIEMC_Viewer
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.PolygonAnnotation polygonAnnotation1 = new System.Windows.Forms.DataVisualization.Charting.PolygonAnnotation();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.EMIChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Graph1PathBox = new System.Windows.Forms.TextBox();
            this.Graph2radioButton = new System.Windows.Forms.RadioButton();
            this.Graph2PathBox = new System.Windows.Forms.TextBox();
            this.Graph1radioButton = new System.Windows.Forms.RadioButton();
            this.OpenFileBtn = new System.Windows.Forms.Button();
            this.CISPR_DetectorBox = new System.Windows.Forms.ComboBox();
            this.SaveGraphBtn2 = new System.Windows.Forms.Button();
            this.SaveGraphBtn = new System.Windows.Forms.Button();
            this.SetTitle_Btn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.SetLegend1_Btn = new System.Windows.Forms.Button();
            this.SetLegend2_Btn = new System.Windows.Forms.Button();
            this.LegendBox = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EMIChart)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 890);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1601, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(102, 17);
            this.StatusLabel.Text = "Drag/Drop a File...";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.textBox1.Font = new System.Drawing.Font("Consolas", 8F);
            this.textBox1.Location = new System.Drawing.Point(1121, 8);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(468, 104);
            this.textBox1.TabIndex = 1;
            // 
            // EMIChart
            // 
            polygonAnnotation1.LineColor = System.Drawing.Color.DarkRed;
            polygonAnnotation1.Name = "PolygonAnnotation1";
            this.EMIChart.Annotations.Add(polygonAnnotation1);
            this.EMIChart.BackColor = System.Drawing.Color.Black;
            chartArea1.AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisX.LineColor = System.Drawing.Color.White;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartArea1.AxisX.MajorTickMark.LineColor = System.Drawing.Color.White;
            chartArea1.AxisX.MajorTickMark.Size = 0.75F;
            chartArea1.AxisX.MinorGrid.Enabled = true;
            chartArea1.AxisX.MinorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisX.MinorTickMark.LineColor = System.Drawing.Color.White;
            chartArea1.AxisX.TitleAlignment = System.Drawing.StringAlignment.Near;
            chartArea1.AxisX.TitleForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisY.MajorTickMark.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.MajorTickMark.Size = 0.5F;
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY.TitleForeColor = System.Drawing.Color.White;
            chartArea1.BackColor = System.Drawing.Color.Black;
            chartArea1.Name = "ChartArea1";
            this.EMIChart.ChartAreas.Add(chartArea1);
            this.EMIChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.BackColor = System.Drawing.Color.Black;
            legend1.BorderColor = System.Drawing.Color.Gainsboro;
            legend1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            legend1.DockedToChartArea = "ChartArea1";
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend1.ForeColor = System.Drawing.Color.White;
            legend1.InterlacedRowsColor = System.Drawing.Color.White;
            legend1.MaximumAutoSize = 75F;
            legend1.Name = "Legend1";
            legend1.TitleAlignment = System.Drawing.StringAlignment.Near;
            legend1.TitleForeColor = System.Drawing.Color.White;
            this.EMIChart.Legends.Add(legend1);
            this.EMIChart.Location = new System.Drawing.Point(0, 0);
            this.EMIChart.Name = "EMIChart";
            series1.BorderColor = System.Drawing.Color.White;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Yellow;
            series1.LabelForeColor = System.Drawing.Color.White;
            series1.Legend = "Legend1";
            series1.Name = "EMI1";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series2.Color = System.Drawing.Color.Aqua;
            series2.IsVisibleInLegend = false;
            series2.Legend = "Legend1";
            series2.Name = "EMI2";
            series3.BorderWidth = 2;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Color = System.Drawing.Color.Salmon;
            series3.IsVisibleInLegend = false;
            series3.Legend = "Legend1";
            series3.Name = "Limits1";
            series4.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            series4.BorderWidth = 2;
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Color = System.Drawing.Color.Salmon;
            series4.IsVisibleInLegend = false;
            series4.Legend = "Legend1";
            series4.Name = "Limits2";
            this.EMIChart.Series.Add(series1);
            this.EMIChart.Series.Add(series2);
            this.EMIChart.Series.Add(series3);
            this.EMIChart.Series.Add(series4);
            this.EMIChart.Size = new System.Drawing.Size(1601, 768);
            this.EMIChart.TabIndex = 2;
            this.EMIChart.Text = "EMIChart";
            this.EMIChart.Visible = false;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.EMIChart);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1601, 768);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.OpenFileBtn);
            this.panel2.Controls.Add(this.SaveGraphBtn2);
            this.panel2.Controls.Add(this.SaveGraphBtn);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 768);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1601, 122);
            this.panel2.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Graph1PathBox);
            this.groupBox1.Controls.Add(this.Graph2radioButton);
            this.groupBox1.Controls.Add(this.Graph2PathBox);
            this.groupBox1.Controls.Add(this.Graph1radioButton);
            this.groupBox1.Location = new System.Drawing.Point(132, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(335, 111);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Files";
            // 
            // Graph1PathBox
            // 
            this.Graph1PathBox.Location = new System.Drawing.Point(33, 19);
            this.Graph1PathBox.Name = "Graph1PathBox";
            this.Graph1PathBox.ReadOnly = true;
            this.Graph1PathBox.Size = new System.Drawing.Size(290, 20);
            this.Graph1PathBox.TabIndex = 15;
            this.Graph1PathBox.Text = "Graph #1";
            // 
            // Graph2radioButton
            // 
            this.Graph2radioButton.AutoSize = true;
            this.Graph2radioButton.Location = new System.Drawing.Point(13, 49);
            this.Graph2radioButton.Name = "Graph2radioButton";
            this.Graph2radioButton.Size = new System.Drawing.Size(14, 13);
            this.Graph2radioButton.TabIndex = 19;
            this.Graph2radioButton.UseVisualStyleBackColor = true;
            // 
            // Graph2PathBox
            // 
            this.Graph2PathBox.Location = new System.Drawing.Point(33, 46);
            this.Graph2PathBox.Name = "Graph2PathBox";
            this.Graph2PathBox.ReadOnly = true;
            this.Graph2PathBox.Size = new System.Drawing.Size(290, 20);
            this.Graph2PathBox.TabIndex = 16;
            this.Graph2PathBox.Text = "Graph #2";
            // 
            // Graph1radioButton
            // 
            this.Graph1radioButton.AutoSize = true;
            this.Graph1radioButton.Checked = true;
            this.Graph1radioButton.Location = new System.Drawing.Point(13, 22);
            this.Graph1radioButton.Name = "Graph1radioButton";
            this.Graph1radioButton.Size = new System.Drawing.Size(14, 13);
            this.Graph1radioButton.TabIndex = 18;
            this.Graph1radioButton.TabStop = true;
            this.Graph1radioButton.UseVisualStyleBackColor = true;
            // 
            // OpenFileBtn
            // 
            this.OpenFileBtn.Location = new System.Drawing.Point(12, 8);
            this.OpenFileBtn.Name = "OpenFileBtn";
            this.OpenFileBtn.Size = new System.Drawing.Size(103, 30);
            this.OpenFileBtn.TabIndex = 12;
            this.OpenFileBtn.Text = "Open File";
            this.OpenFileBtn.UseVisualStyleBackColor = true;
            this.OpenFileBtn.Click += new System.EventHandler(this.OpenFileBtn_Click);
            // 
            // CISPR_DetectorBox
            // 
            this.CISPR_DetectorBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CISPR_DetectorBox.Enabled = false;
            this.CISPR_DetectorBox.FormattingEnabled = true;
            this.CISPR_DetectorBox.Items.AddRange(new object[] {
            "Average",
            "Quasi-Peak"});
            this.CISPR_DetectorBox.Location = new System.Drawing.Point(63, 17);
            this.CISPR_DetectorBox.Name = "CISPR_DetectorBox";
            this.CISPR_DetectorBox.Size = new System.Drawing.Size(100, 21);
            this.CISPR_DetectorBox.TabIndex = 10;
            // 
            // SaveGraphBtn2
            // 
            this.SaveGraphBtn2.Location = new System.Drawing.Point(12, 80);
            this.SaveGraphBtn2.Name = "SaveGraphBtn2";
            this.SaveGraphBtn2.Size = new System.Drawing.Size(103, 30);
            this.SaveGraphBtn2.TabIndex = 8;
            this.SaveGraphBtn2.Text = "Save to Clipboard";
            this.SaveGraphBtn2.UseVisualStyleBackColor = true;
            this.SaveGraphBtn2.Click += new System.EventHandler(this.SaveGraphBtn2_Click);
            // 
            // SaveGraphBtn
            // 
            this.SaveGraphBtn.Location = new System.Drawing.Point(12, 44);
            this.SaveGraphBtn.Name = "SaveGraphBtn";
            this.SaveGraphBtn.Size = new System.Drawing.Size(103, 30);
            this.SaveGraphBtn.TabIndex = 7;
            this.SaveGraphBtn.Text = "Save to File";
            this.SaveGraphBtn.UseVisualStyleBackColor = true;
            this.SaveGraphBtn.Click += new System.EventHandler(this.SaveGraphBtn_Click);
            // 
            // SetTitle_Btn
            // 
            this.SetTitle_Btn.Location = new System.Drawing.Point(113, 75);
            this.SetTitle_Btn.Name = "SetTitle_Btn";
            this.SetTitle_Btn.Size = new System.Drawing.Size(98, 25);
            this.SetTitle_Btn.TabIndex = 5;
            this.SetTitle_Btn.Text = "Set Title";
            this.SetTitle_Btn.UseVisualStyleBackColor = true;
            this.SetTitle_Btn.Click += new System.EventHandler(this.SetTitle_Btn_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "emiemc";
            this.openFileDialog1.Filter = "EMCEMI files (*.emcemi)|*.emcemi";
            this.openFileDialog1.RestoreDirectory = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.CISPR_DetectorBox);
            this.groupBox2.Location = new System.Drawing.Point(473, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(249, 111);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Limits";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox1);
            this.groupBox3.Controls.Add(this.LegendBox);
            this.groupBox3.Controls.Add(this.SetTitle_Btn);
            this.groupBox3.Controls.Add(this.SetLegend2_Btn);
            this.groupBox3.Controls.Add(this.SetLegend1_Btn);
            this.groupBox3.Location = new System.Drawing.Point(728, 8);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(217, 111);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Legends";
            // 
            // SetLegend1_Btn
            // 
            this.SetLegend1_Btn.Location = new System.Drawing.Point(6, 44);
            this.SetLegend1_Btn.Name = "SetLegend1_Btn";
            this.SetLegend1_Btn.Size = new System.Drawing.Size(98, 25);
            this.SetLegend1_Btn.TabIndex = 6;
            this.SetLegend1_Btn.Text = "Set Legend #1";
            this.SetLegend1_Btn.UseVisualStyleBackColor = true;
            this.SetLegend1_Btn.Click += new System.EventHandler(this.SetLegend1_Btn_Click);
            // 
            // SetLegend2_Btn
            // 
            this.SetLegend2_Btn.Location = new System.Drawing.Point(113, 44);
            this.SetLegend2_Btn.Name = "SetLegend2_Btn";
            this.SetLegend2_Btn.Size = new System.Drawing.Size(98, 25);
            this.SetLegend2_Btn.TabIndex = 7;
            this.SetLegend2_Btn.Text = "Set Legend #2";
            this.SetLegend2_Btn.UseVisualStyleBackColor = true;
            this.SetLegend2_Btn.Click += new System.EventHandler(this.SetLegend2_Btn_Click);
            // 
            // LegendBox
            // 
            this.LegendBox.Location = new System.Drawing.Point(6, 18);
            this.LegendBox.Name = "LegendBox";
            this.LegendBox.Size = new System.Drawing.Size(205, 20);
            this.LegendBox.TabIndex = 8;
            this.LegendBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(7, 80);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(100, 17);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "Disable Legend";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Detector:";
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1601, 912);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EMCEMI Viewer";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EMIChart)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataVisualization.Charting.Chart EMIChart;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button SetTitle_Btn;
        private System.Windows.Forms.Button SaveGraphBtn;
        private System.Windows.Forms.Button SaveGraphBtn2;
        private System.Windows.Forms.ComboBox CISPR_DetectorBox;
        private System.Windows.Forms.Button OpenFileBtn;
        private System.Windows.Forms.RadioButton Graph2radioButton;
        private System.Windows.Forms.RadioButton Graph1radioButton;
        private System.Windows.Forms.TextBox Graph2PathBox;
        private System.Windows.Forms.TextBox Graph1PathBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox LegendBox;
        private System.Windows.Forms.Button SetLegend2_Btn;
        private System.Windows.Forms.Button SetLegend1_Btn;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label1;
    }
}

