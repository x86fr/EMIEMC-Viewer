namespace EMIEMC_Viewer
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.EMIChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SaveGraphBtn = new System.Windows.Forms.Button();
            this.CISPR32_LimitBox = new System.Windows.Forms.ComboBox();
            this.SetTitle_Btn = new System.Windows.Forms.Button();
            this.GraphTitle = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EMIChart)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 874);
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
            this.textBox1.Font = new System.Drawing.Font("Consolas", 10F);
            this.textBox1.Location = new System.Drawing.Point(12, 6);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(711, 98);
            this.textBox1.TabIndex = 1;
            // 
            // EMIChart
            // 
            this.EMIChart.BackColor = System.Drawing.Color.Black;
            chartArea1.AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisX.LineColor = System.Drawing.Color.White;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartArea1.AxisX.MajorTickMark.LineColor = System.Drawing.Color.White;
            chartArea1.AxisX.MajorTickMark.Size = 0.75F;
            chartArea1.AxisX.MinorGrid.Enabled = true;
            chartArea1.AxisX.MinorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisX.MinorTickMark.LineColor = System.Drawing.Color.White;
            chartArea1.AxisX.TitleAlignment = System.Drawing.StringAlignment.Far;
            chartArea1.AxisX.TitleForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisY.MajorTickMark.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.MajorTickMark.Size = 0.5F;
            chartArea1.AxisY.TitleAlignment = System.Drawing.StringAlignment.Far;
            chartArea1.AxisY.TitleForeColor = System.Drawing.Color.White;
            chartArea1.BackColor = System.Drawing.Color.Black;
            chartArea1.Name = "ChartArea1";
            this.EMIChart.ChartAreas.Add(chartArea1);
            this.EMIChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EMIChart.Location = new System.Drawing.Point(0, 0);
            this.EMIChart.Name = "EMIChart";
            series1.BorderColor = System.Drawing.Color.White;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Yellow;
            series1.LabelForeColor = System.Drawing.Color.White;
            series1.Legend = "Legend1";
            series1.Name = "EMCValue";
            series2.BorderWidth = 2;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.Salmon;
            series2.Name = "Limits";
            series3.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            series3.BorderWidth = 2;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Color = System.Drawing.Color.Salmon;
            series3.Name = "Limits2";
            this.EMIChart.Series.Add(series1);
            this.EMIChart.Series.Add(series2);
            this.EMIChart.Series.Add(series3);
            this.EMIChart.Size = new System.Drawing.Size(1601, 767);
            this.EMIChart.TabIndex = 2;
            this.EMIChart.Text = "EMIChart";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.EMIChart);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1601, 767);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.SaveGraphBtn);
            this.panel2.Controls.Add(this.CISPR32_LimitBox);
            this.panel2.Controls.Add(this.SetTitle_Btn);
            this.panel2.Controls.Add(this.GraphTitle);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 767);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1601, 107);
            this.panel2.TabIndex = 5;
            // 
            // SaveGraphBtn
            // 
            this.SaveGraphBtn.Location = new System.Drawing.Point(1467, 60);
            this.SaveGraphBtn.Name = "SaveGraphBtn";
            this.SaveGraphBtn.Size = new System.Drawing.Size(122, 37);
            this.SaveGraphBtn.TabIndex = 7;
            this.SaveGraphBtn.Text = "Save Graph";
            this.SaveGraphBtn.UseVisualStyleBackColor = true;
            this.SaveGraphBtn.Click += new System.EventHandler(this.SaveGraphBtn_Click);
            // 
            // CISPR32_LimitBox
            // 
            this.CISPR32_LimitBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CISPR32_LimitBox.FormattingEnabled = true;
            this.CISPR32_LimitBox.Items.AddRange(new object[] {
            "CISPR32 Class B - 15 kHz to 30 MHz (3m)",
            "CISPR32 Class B - 30 MHz to 1 GHz (3m)",
            "CISPR32 Class B - 1 GHz to 6 GHz (3m)"});
            this.CISPR32_LimitBox.Location = new System.Drawing.Point(1361, 7);
            this.CISPR32_LimitBox.Name = "CISPR32_LimitBox";
            this.CISPR32_LimitBox.Size = new System.Drawing.Size(228, 21);
            this.CISPR32_LimitBox.TabIndex = 6;
            // 
            // SetTitle_Btn
            // 
            this.SetTitle_Btn.Location = new System.Drawing.Point(1131, 8);
            this.SetTitle_Btn.Name = "SetTitle_Btn";
            this.SetTitle_Btn.Size = new System.Drawing.Size(75, 20);
            this.SetTitle_Btn.TabIndex = 5;
            this.SetTitle_Btn.Text = "SET";
            this.SetTitle_Btn.UseVisualStyleBackColor = true;
            this.SetTitle_Btn.Click += new System.EventHandler(this.SetTitle_Btn_Click);
            // 
            // GraphTitle
            // 
            this.GraphTitle.Location = new System.Drawing.Point(762, 8);
            this.GraphTitle.Name = "GraphTitle";
            this.GraphTitle.Size = new System.Drawing.Size(345, 20);
            this.GraphTitle.TabIndex = 3;
            this.GraphTitle.Text = "Title";
            this.GraphTitle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1601, 896);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusStrip1);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EMIEMC Viewer";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EMIChart)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
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
        private System.Windows.Forms.TextBox GraphTitle;
        private System.Windows.Forms.ComboBox CISPR32_LimitBox;
        private System.Windows.Forms.Button SaveGraphBtn;
    }
}

