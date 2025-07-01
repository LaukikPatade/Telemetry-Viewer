namespace Telemetry_demo
{
    partial class SettingsPage
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
            this.groupBoxConfigPath = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtConfigPath = new System.Windows.Forms.TextBox();
            this.lblConfigPath = new System.Windows.Forms.Label();
            this.groupBoxLogPath = new System.Windows.Forms.GroupBox();
            this.btnBrowseLog = new System.Windows.Forms.Button();
            this.txtLogPath = new System.Windows.Forms.TextBox();
            this.lblLogPath = new System.Windows.Forms.Label();
            this.groupBoxConfigPath.SuspendLayout();
            this.groupBoxLogPath.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxConfigPath
            // 
            this.groupBoxConfigPath.Controls.Add(this.btnBrowse);
            this.groupBoxConfigPath.Controls.Add(this.txtConfigPath);
            this.groupBoxConfigPath.Controls.Add(this.lblConfigPath);
            this.groupBoxConfigPath.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxConfigPath.Location = new System.Drawing.Point(30, 30);
            this.groupBoxConfigPath.Name = "groupBoxConfigPath";
            this.groupBoxConfigPath.Size = new System.Drawing.Size(600, 100);
            this.groupBoxConfigPath.TabIndex = 0;
            this.groupBoxConfigPath.TabStop = false;
            this.groupBoxConfigPath.Text = "Configuration Settings";
            // 
            // btnBrowse
            // 
            this.btnBrowse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnBrowse.FlatAppearance.BorderSize = 0;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnBrowse.ForeColor = System.Drawing.Color.White;
            this.btnBrowse.Location = new System.Drawing.Point(500, 40);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(80, 30);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtConfigPath
            // 
            this.txtConfigPath.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtConfigPath.Location = new System.Drawing.Point(120, 45);
            this.txtConfigPath.Name = "txtConfigPath";
            this.txtConfigPath.Size = new System.Drawing.Size(370, 23);
            this.txtConfigPath.TabIndex = 1;
            // 
            // lblConfigPath
            // 
            this.lblConfigPath.AutoSize = true;
            this.lblConfigPath.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblConfigPath.Location = new System.Drawing.Point(20, 48);
            this.lblConfigPath.Name = "lblConfigPath";
            this.lblConfigPath.Size = new System.Drawing.Size(94, 15);
            this.lblConfigPath.TabIndex = 0;
            this.lblConfigPath.Text = "Config File Path:";
            // 
            // groupBoxLogPath
            // 
            this.groupBoxLogPath.Controls.Add(this.btnBrowseLog);
            this.groupBoxLogPath.Controls.Add(this.txtLogPath);
            this.groupBoxLogPath.Controls.Add(this.lblLogPath);
            this.groupBoxLogPath.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxLogPath.Location = new System.Drawing.Point(30, 150);
            this.groupBoxLogPath.Name = "groupBoxLogPath";
            this.groupBoxLogPath.Size = new System.Drawing.Size(600, 100);
            this.groupBoxLogPath.TabIndex = 1;
            this.groupBoxLogPath.TabStop = false;
            this.groupBoxLogPath.Text = "Logs Folder";
            // 
            // btnBrowseLog
            // 
            this.btnBrowseLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnBrowseLog.FlatAppearance.BorderSize = 0;
            this.btnBrowseLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseLog.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnBrowseLog.ForeColor = System.Drawing.Color.White;
            this.btnBrowseLog.Location = new System.Drawing.Point(500, 40);
            this.btnBrowseLog.Name = "btnBrowseLog";
            this.btnBrowseLog.Size = new System.Drawing.Size(80, 30);
            this.btnBrowseLog.TabIndex = 2;
            this.btnBrowseLog.Text = "Browse";
            this.btnBrowseLog.UseVisualStyleBackColor = false;
            this.btnBrowseLog.Click += new System.EventHandler(this.btnBrowseLog_Click);
            // 
            // txtLogPath
            // 
            this.txtLogPath.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtLogPath.Location = new System.Drawing.Point(120, 45);
            this.txtLogPath.Name = "txtLogPath";
            this.txtLogPath.Size = new System.Drawing.Size(370, 23);
            this.txtLogPath.TabIndex = 1;
            // 
            // lblLogPath
            // 
            this.lblLogPath.AutoSize = true;
            this.lblLogPath.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLogPath.Location = new System.Drawing.Point(20, 48);
            this.lblLogPath.Name = "lblLogPath";
            this.lblLogPath.Size = new System.Drawing.Size(74, 15);
            this.lblLogPath.TabIndex = 0;
            this.lblLogPath.Text = "Logs Folder:";
            // 
            // SettingsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.groupBoxConfigPath);
            this.Controls.Add(this.groupBoxLogPath);
            this.Name = "SettingsPage";
            this.Size = new System.Drawing.Size(800, 600);
            this.groupBoxConfigPath.ResumeLayout(false);
            this.groupBoxConfigPath.PerformLayout();
            this.groupBoxLogPath.ResumeLayout(false);
            this.groupBoxLogPath.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxConfigPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtConfigPath;
        private System.Windows.Forms.Label lblConfigPath;
        private System.Windows.Forms.GroupBox groupBoxLogPath;
        private System.Windows.Forms.Button btnBrowseLog;
        private System.Windows.Forms.TextBox txtLogPath;
        private System.Windows.Forms.Label lblLogPath;
    }
} 