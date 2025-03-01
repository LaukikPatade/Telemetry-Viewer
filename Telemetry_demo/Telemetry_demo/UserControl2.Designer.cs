namespace Telemetry_demo
{
    partial class UserControl2
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
            this.tbIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnWirelessConnect = new System.Windows.Forms.Button();
            this.tbSend = new System.Windows.Forms.TextBox();
            this.tbreceived = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnShowList = new System.Windows.Forms.Button();
            this.listBoxConfig = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // tbIP
            // 
            this.tbIP.Location = new System.Drawing.Point(125, 61);
            this.tbIP.Name = "tbIP";
            this.tbIP.Size = new System.Drawing.Size(162, 22);
            this.tbIP.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(125, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enter Port";
            // 
            // btnWirelessConnect
            // 
            this.btnWirelessConnect.Location = new System.Drawing.Point(408, 61);
            this.btnWirelessConnect.Name = "btnWirelessConnect";
            this.btnWirelessConnect.Size = new System.Drawing.Size(75, 23);
            this.btnWirelessConnect.TabIndex = 2;
            this.btnWirelessConnect.Text = "Connect";
            this.btnWirelessConnect.UseVisualStyleBackColor = true;
            this.btnWirelessConnect.Click += new System.EventHandler(this.btnWirelessConnect_Click);
            // 
            // tbSend
            // 
            this.tbSend.Location = new System.Drawing.Point(128, 173);
            this.tbSend.Name = "tbSend";
            this.tbSend.Size = new System.Drawing.Size(159, 22);
            this.tbSend.TabIndex = 3;
            // 
            // tbreceived
            // 
            this.tbreceived.Location = new System.Drawing.Point(128, 284);
            this.tbreceived.Name = "tbreceived";
            this.tbreceived.Size = new System.Drawing.Size(159, 22);
            this.tbreceived.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(128, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Send";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(125, 255);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Recieve";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(408, 173);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 7;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click_1);
            // 
            // btnShowList
            // 
            this.btnShowList.Location = new System.Drawing.Point(408, 371);
            this.btnShowList.Name = "btnShowList";
            this.btnShowList.Size = new System.Drawing.Size(75, 23);
            this.btnShowList.TabIndex = 8;
            this.btnShowList.Text = "SHOW";
            this.btnShowList.UseVisualStyleBackColor = true;
            this.btnShowList.Click += new System.EventHandler(this.btnShowList_Click);
            // 
            // listBoxConfig
            // 
            this.listBoxConfig.FormattingEnabled = true;
            this.listBoxConfig.ItemHeight = 16;
            this.listBoxConfig.Location = new System.Drawing.Point(131, 423);
            this.listBoxConfig.Name = "listBoxConfig";
            this.listBoxConfig.Size = new System.Drawing.Size(955, 212);
            this.listBoxConfig.TabIndex = 9;
            // 
            // UserControl2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listBoxConfig);
            this.Controls.Add(this.btnShowList);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbreceived);
            this.Controls.Add(this.tbSend);
            this.Controls.Add(this.btnWirelessConnect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbIP);
            this.Name = "UserControl2";
            this.Size = new System.Drawing.Size(1321, 636);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnWirelessConnect;
        private System.Windows.Forms.TextBox tbSend;
        private System.Windows.Forms.TextBox tbreceived;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnShowList;
        private System.Windows.Forms.ListBox listBoxConfig;
    }
}
