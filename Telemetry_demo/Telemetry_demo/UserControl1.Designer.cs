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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.btnUARTConnect = new System.Windows.Forms.Button();
            this.tbInputName = new System.Windows.Forms.TextBox();
            this.configList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(219, 210);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 24);
            this.comboBox1.TabIndex = 0;
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(444, 210);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 24);
            this.comboBox2.TabIndex = 1;
            // 
            // btnUARTConnect
            // 
            this.btnUARTConnect.Location = new System.Drawing.Point(857, 211);
            this.btnUARTConnect.Name = "btnUARTConnect";
            this.btnUARTConnect.Size = new System.Drawing.Size(85, 24);
            this.btnUARTConnect.TabIndex = 2;
            this.btnUARTConnect.Text = "Save";
            this.btnUARTConnect.UseVisualStyleBackColor = true;
            this.btnUARTConnect.Click += new System.EventHandler(this.btnUARTConnect_Click);
            // 
            // tbInputName
            // 
            this.tbInputName.Location = new System.Drawing.Point(669, 213);
            this.tbInputName.Name = "tbInputName";
            this.tbInputName.Size = new System.Drawing.Size(100, 22);
            this.tbInputName.TabIndex = 3;
            // 
            // configList
            // 
            this.configList.FormattingEnabled = true;
            this.configList.ItemHeight = 16;
            this.configList.Location = new System.Drawing.Point(219, 327);
            this.configList.Name = "configList";
            this.configList.Size = new System.Drawing.Size(747, 116);
            this.configList.TabIndex = 4;
            this.configList.SelectedIndexChanged += new System.EventHandler(this.configList_SelectedIndexChanged);
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.configList);
            this.Controls.Add(this.tbInputName);
            this.Controls.Add(this.btnUARTConnect);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(1294, 637);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button btnUARTConnect;
        private System.Windows.Forms.TextBox tbInputName;
        private System.Windows.Forms.ListBox configList;
    }
}
