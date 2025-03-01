namespace Telemetry_demo
{
    partial class UserControl1
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.btnUARTConnect = new System.Windows.Forms.Button();
            this.chartUART = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chartUART)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(103, 45);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 24);
            this.comboBox1.TabIndex = 0;
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(308, 45);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 24);
            this.comboBox2.TabIndex = 1;
            // 
            // btnUARTConnect
            // 
            this.btnUARTConnect.Location = new System.Drawing.Point(519, 45);
            this.btnUARTConnect.Name = "btnUARTConnect";
            this.btnUARTConnect.Size = new System.Drawing.Size(75, 23);
            this.btnUARTConnect.TabIndex = 2;
            this.btnUARTConnect.Text = "Connect";
            this.btnUARTConnect.UseVisualStyleBackColor = true;
            this.btnUARTConnect.Click += new System.EventHandler(this.btnUARTConnect_Click);
            // 
            // chartUART
            // 
            chartArea1.Name = "ChartArea1";
            this.chartUART.ChartAreas.Add(chartArea1);
            this.chartUART.Dock = System.Windows.Forms.DockStyle.Bottom;
            legend1.Name = "Legend1";
            this.chartUART.Legends.Add(legend1);
            this.chartUART.Location = new System.Drawing.Point(0, 113);
            this.chartUART.Name = "chartUART";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartUART.Series.Add(series1);
            this.chartUART.Size = new System.Drawing.Size(745, 405);
            this.chartUART.TabIndex = 3;
            this.chartUART.Text = "chart1";
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chartUART);
            this.Controls.Add(this.btnUARTConnect);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(745, 518);
            ((System.ComponentModel.ISupportInitialize)(this.chartUART)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button btnUARTConnect;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartUART;
    }
}
