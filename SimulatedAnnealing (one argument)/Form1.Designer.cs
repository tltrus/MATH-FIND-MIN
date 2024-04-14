namespace Chart
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnStaticStart = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnDynamicStart = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbTemp = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea5.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea5);
            this.chart1.Location = new System.Drawing.Point(9, 10);
            this.chart1.Margin = new System.Windows.Forms.Padding(2);
            this.chart1.Name = "chart1";
            series9.ChartArea = "ChartArea1";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series9.Name = "Series1";
            series10.ChartArea = "ChartArea1";
            series10.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series10.Name = "Series2";
            this.chart1.Series.Add(series9);
            this.chart1.Series.Add(series10);
            this.chart1.Size = new System.Drawing.Size(560, 346);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnStaticStart
            // 
            this.btnStaticStart.Location = new System.Drawing.Point(573, 10);
            this.btnStaticStart.Margin = new System.Windows.Forms.Padding(2);
            this.btnStaticStart.Name = "btnStaticStart";
            this.btnStaticStart.Size = new System.Drawing.Size(74, 31);
            this.btnStaticStart.TabIndex = 1;
            this.btnStaticStart.Text = "Static Start";
            this.btnStaticStart.UseVisualStyleBackColor = true;
            this.btnStaticStart.Click += new System.EventHandler(this.btnStaticStart_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(573, 123);
            this.btnReset.Margin = new System.Windows.Forms.Padding(2);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(74, 30);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnDynamicStart
            // 
            this.btnDynamicStart.Location = new System.Drawing.Point(573, 45);
            this.btnDynamicStart.Margin = new System.Windows.Forms.Padding(2);
            this.btnDynamicStart.Name = "btnDynamicStart";
            this.btnDynamicStart.Size = new System.Drawing.Size(74, 47);
            this.btnDynamicStart.TabIndex = 3;
            this.btnDynamicStart.Text = "Dynamic start";
            this.btnDynamicStart.UseVisualStyleBackColor = true;
            this.btnDynamicStart.Click += new System.EventHandler(this.btnDynamicStart_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(573, 187);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Temperature:";
            // 
            // lbTemp
            // 
            this.lbTemp.AutoSize = true;
            this.lbTemp.Location = new System.Drawing.Point(588, 211);
            this.lbTemp.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbTemp.Name = "lbTemp";
            this.lbTemp.Size = new System.Drawing.Size(13, 13);
            this.lbTemp.TabIndex = 5;
            this.lbTemp.Text = "0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 367);
            this.Controls.Add(this.lbTemp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDynamicStart);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnStaticStart);
            this.Controls.Add(this.chart1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Find min - Simulated annealing";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnStaticStart;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnDynamicStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbTemp;
    }
}

