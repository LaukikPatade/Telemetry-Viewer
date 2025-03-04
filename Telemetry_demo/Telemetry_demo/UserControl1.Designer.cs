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
            this.ComportBox = new System.Windows.Forms.ComboBox();
            this.BtnSaveConn = new System.Windows.Forms.Button();
            this.ConnName = new System.Windows.Forms.TextBox();
            this.configList = new System.Windows.Forms.ListBox();
            this.ModeComboBox = new System.Windows.Forms.ComboBox();
            this.ConnNameLabel = new System.Windows.Forms.Label();
            this.ModeBoxLabel = new System.Windows.Forms.Label();
            this.BaudRateBoxLabel = new System.Windows.Forms.Label();
            this.ComportBoxLabel = new System.Windows.Forms.Label();
            this.BaudComboBox = new System.Windows.Forms.ComboBox();
            this.ConfigListLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ComportBox
            // 
            this.ComportBox.FormattingEnabled = true;
            this.ComportBox.Location = new System.Drawing.Point(112, 117);
            this.ComportBox.Margin = new System.Windows.Forms.Padding(2);
            this.ComportBox.Name = "ComportBox";
            this.ComportBox.Size = new System.Drawing.Size(92, 21);
            this.ComportBox.TabIndex = 0;
            // 
            // BtnSaveConn
            // 
            this.BtnSaveConn.Location = new System.Drawing.Point(389, 228);
            this.BtnSaveConn.Margin = new System.Windows.Forms.Padding(2);
            this.BtnSaveConn.Name = "BtnSaveConn";
            this.BtnSaveConn.Size = new System.Drawing.Size(64, 20);
            this.BtnSaveConn.TabIndex = 2;
            this.BtnSaveConn.Text = "Save";
            this.BtnSaveConn.UseVisualStyleBackColor = true;
            this.BtnSaveConn.Click += new System.EventHandler(this.BtnSaveConn_Click);
            // 
            // ConnName
            // 
            this.ConnName.Location = new System.Drawing.Point(601, 118);
            this.ConnName.Margin = new System.Windows.Forms.Padding(2);
            this.ConnName.Name = "ConnName";
            this.ConnName.Size = new System.Drawing.Size(117, 20);
            this.ConnName.TabIndex = 3;
            // 
            // configList
            // 
            this.configList.FormattingEnabled = true;
            this.configList.Location = new System.Drawing.Point(802, 113);
            this.configList.Margin = new System.Windows.Forms.Padding(2);
            this.configList.Name = "configList";
            this.configList.Size = new System.Drawing.Size(419, 511);
            this.configList.TabIndex = 4;
            // 
            // ModeComboBox
            // 
            this.ModeComboBox.FormattingEnabled = true;
            this.ModeComboBox.Location = new System.Drawing.Point(453, 117);
            this.ModeComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.ModeComboBox.Name = "ModeComboBox";
            this.ModeComboBox.Size = new System.Drawing.Size(92, 21);
            this.ModeComboBox.TabIndex = 5;
            // 
            // ConnNameLabel
            // 
            this.ConnNameLabel.AutoSize = true;
            this.ConnNameLabel.Location = new System.Drawing.Point(601, 98);
            this.ConnNameLabel.Name = "ConnNameLabel";
            this.ConnNameLabel.Size = new System.Drawing.Size(117, 13);
            this.ConnNameLabel.TabIndex = 6;
            this.ConnNameLabel.Text = "Enter connection name";
            // 
            // ModeBoxLabel
            // 
            this.ModeBoxLabel.AutoSize = true;
            this.ModeBoxLabel.Location = new System.Drawing.Point(450, 98);
            this.ModeBoxLabel.Name = "ModeBoxLabel";
            this.ModeBoxLabel.Size = new System.Drawing.Size(73, 13);
            this.ModeBoxLabel.TabIndex = 7;
            this.ModeBoxLabel.Text = "Choose Mode";
            // 
            // BaudRateBoxLabel
            // 
            this.BaudRateBoxLabel.AutoSize = true;
            this.BaudRateBoxLabel.Location = new System.Drawing.Point(281, 98);
            this.BaudRateBoxLabel.Name = "BaudRateBoxLabel";
            this.BaudRateBoxLabel.Size = new System.Drawing.Size(97, 13);
            this.BaudRateBoxLabel.TabIndex = 8;
            this.BaudRateBoxLabel.Text = "Choose Baud Rate";
            // 
            // ComportBoxLabel
            // 
            this.ComportBoxLabel.AutoSize = true;
            this.ComportBoxLabel.Location = new System.Drawing.Point(109, 98);
            this.ComportBoxLabel.Name = "ComportBoxLabel";
            this.ComportBoxLabel.Size = new System.Drawing.Size(92, 13);
            this.ComportBoxLabel.TabIndex = 9;
            this.ComportBoxLabel.Text = "Choose COM Port";
            // 
            // BaudComboBox
            // 
            this.BaudComboBox.FormattingEnabled = true;
            this.BaudComboBox.Location = new System.Drawing.Point(284, 117);
            this.BaudComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.BaudComboBox.Name = "BaudComboBox";
            this.BaudComboBox.Size = new System.Drawing.Size(92, 21);
            this.BaudComboBox.TabIndex = 10;
            // 
            // ConfigListLabel
            // 
            this.ConfigListLabel.AutoSize = true;
            this.ConfigListLabel.Location = new System.Drawing.Point(799, 87);
            this.ConfigListLabel.Name = "ConfigListLabel";
            this.ConfigListLabel.Size = new System.Drawing.Size(88, 13);
            this.ConfigListLabel.TabIndex = 11;
            this.ConfigListLabel.Text = "Configuration List";
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ConfigListLabel);
            this.Controls.Add(this.BaudComboBox);
            this.Controls.Add(this.ComportBoxLabel);
            this.Controls.Add(this.BaudRateBoxLabel);
            this.Controls.Add(this.ModeBoxLabel);
            this.Controls.Add(this.ConnNameLabel);
            this.Controls.Add(this.ModeComboBox);
            this.Controls.Add(this.configList);
            this.Controls.Add(this.ConnName);
            this.Controls.Add(this.BtnSaveConn);
            this.Controls.Add(this.ComportBox);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(1279, 718);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ComportBox;
        private System.Windows.Forms.Button BtnSaveConn;
        private System.Windows.Forms.TextBox ConnName;
        private System.Windows.Forms.ListBox configList;
        private System.Windows.Forms.ComboBox ModeComboBox;
        private System.Windows.Forms.Label ConnNameLabel;
        private System.Windows.Forms.Label ModeBoxLabel;
        private System.Windows.Forms.Label BaudRateBoxLabel;
        private System.Windows.Forms.Label ComportBoxLabel;
        private System.Windows.Forms.ComboBox BaudComboBox;
        private System.Windows.Forms.Label ConfigListLabel;
    }
}
