namespace Telemetry_demo
{
    partial class UserControl1
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel mainTableLayout;

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
            this.ConnTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.UDPPortLbl = new System.Windows.Forms.Label();
            this.UDPPortTextBox = new System.Windows.Forms.TextBox();
            this.groupBoxConnection = new System.Windows.Forms.GroupBox();
            this.groupBoxChannels = new System.Windows.Forms.GroupBox();
            this.groupBoxConfigs = new System.Windows.Forms.GroupBox();
            this.dgvChannels = new System.Windows.Forms.DataGridView();
            this.toolTip1 = new System.Windows.Forms.ToolTip();
            this.panelChannelCard = new System.Windows.Forms.Panel();
            this.btnSaveChannels = new System.Windows.Forms.Button();
            this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // ComportBox
            // 
            this.ComportBox.FormattingEnabled = true;
            this.ComportBox.Location = new System.Drawing.Point(487, 240);
            this.ComportBox.Margin = new System.Windows.Forms.Padding(2);
            this.ComportBox.Name = "ComportBox";
            this.ComportBox.Size = new System.Drawing.Size(92, 21);
            this.ComportBox.TabIndex = 0;
            // 
            // BtnSaveConn
            // 
            this.BtnSaveConn.Location = new System.Drawing.Point(567, 340);
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
            this.ConnName.Location = new System.Drawing.Point(976, 241);
            this.ConnName.Margin = new System.Windows.Forms.Padding(2);
            this.ConnName.Name = "ConnName";
            this.ConnName.Size = new System.Drawing.Size(117, 20);
            this.ConnName.TabIndex = 3;
            // 
            // configList
            // 
            this.configList.FormattingEnabled = true;
            this.configList.Location = new System.Drawing.Point(802, 425);
            this.configList.Margin = new System.Windows.Forms.Padding(2);
            this.configList.Name = "configList";
            this.configList.Size = new System.Drawing.Size(399, 199);
            this.configList.TabIndex = 4;
            // 
            // ModeComboBox
            // 
            this.ModeComboBox.FormattingEnabled = true;
            this.ModeComboBox.Location = new System.Drawing.Point(828, 240);
            this.ModeComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.ModeComboBox.Name = "ModeComboBox";
            this.ModeComboBox.Size = new System.Drawing.Size(92, 21);
            this.ModeComboBox.TabIndex = 5;
            // 
            // ConnNameLabel
            // 
            this.ConnNameLabel.AutoSize = true;
            this.ConnNameLabel.Location = new System.Drawing.Point(976, 221);
            this.ConnNameLabel.Name = "ConnNameLabel";
            this.ConnNameLabel.Size = new System.Drawing.Size(117, 13);
            this.ConnNameLabel.TabIndex = 6;
            this.ConnNameLabel.Text = "Enter connection name";
            // 
            // ModeBoxLabel
            // 
            this.ModeBoxLabel.AutoSize = true;
            this.ModeBoxLabel.Location = new System.Drawing.Point(825, 221);
            this.ModeBoxLabel.Name = "ModeBoxLabel";
            this.ModeBoxLabel.Size = new System.Drawing.Size(73, 13);
            this.ModeBoxLabel.TabIndex = 7;
            this.ModeBoxLabel.Text = "Choose Mode";
            // 
            // BaudRateBoxLabel
            // 
            this.BaudRateBoxLabel.AutoSize = true;
            this.BaudRateBoxLabel.Location = new System.Drawing.Point(656, 221);
            this.BaudRateBoxLabel.Name = "BaudRateBoxLabel";
            this.BaudRateBoxLabel.Size = new System.Drawing.Size(97, 13);
            this.BaudRateBoxLabel.TabIndex = 8;
            this.BaudRateBoxLabel.Text = "Choose Baud Rate";
            // 
            // ComportBoxLabel
            // 
            this.ComportBoxLabel.AutoSize = true;
            this.ComportBoxLabel.Location = new System.Drawing.Point(484, 221);
            this.ComportBoxLabel.Name = "ComportBoxLabel";
            this.ComportBoxLabel.Size = new System.Drawing.Size(92, 13);
            this.ComportBoxLabel.TabIndex = 9;
            this.ComportBoxLabel.Text = "Choose COM Port";
            // 
            // BaudComboBox
            // 
            this.BaudComboBox.FormattingEnabled = true;
            this.BaudComboBox.Location = new System.Drawing.Point(659, 240);
            this.BaudComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.BaudComboBox.Name = "BaudComboBox";
            this.BaudComboBox.Size = new System.Drawing.Size(92, 21);
            this.BaudComboBox.TabIndex = 10;
            // 
            // ConfigListLabel
            // 
            this.ConfigListLabel.AutoSize = true;
            this.ConfigListLabel.Location = new System.Drawing.Point(799, 401);
            this.ConfigListLabel.Name = "ConfigListLabel";
            this.ConfigListLabel.Size = new System.Drawing.Size(88, 13);
            this.ConfigListLabel.TabIndex = 11;
            this.ConfigListLabel.Text = "Configuration List";
            // 
            // ConnTypeComboBox
            // 
            this.ConnTypeComboBox.FormattingEnabled = true;
            this.ConnTypeComboBox.Location = new System.Drawing.Point(298, 241);
            this.ConnTypeComboBox.Name = "ConnTypeComboBox";
            this.ConnTypeComboBox.Size = new System.Drawing.Size(121, 21);
            this.ConnTypeComboBox.TabIndex = 12;
            this.ConnTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.ConnTypeComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(298, 221);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Choose Connection Type";
            // 
            // UDPPortLbl
            // 
            this.UDPPortLbl.AutoSize = true;
            this.UDPPortLbl.Location = new System.Drawing.Point(485, 221);
            this.UDPPortLbl.Name = "UDPPortLbl";
            this.UDPPortLbl.Size = new System.Drawing.Size(80, 13);
            this.UDPPortLbl.TabIndex = 15;
            this.UDPPortLbl.Text = "Enter UDP Port";
            // 
            // UDPPortTextBox
            // 
            this.UDPPortTextBox.Location = new System.Drawing.Point(488, 240);
            this.UDPPortTextBox.Name = "UDPPortTextBox";
            this.UDPPortTextBox.Size = new System.Drawing.Size(91, 20);
            this.UDPPortTextBox.TabIndex = 16;
            // 
            // groupBoxConnection
            // 
            this.groupBoxConnection.Text = "Connection Settings";
            this.groupBoxConnection.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxConnection.Location = new System.Drawing.Point(30, 30);
            this.groupBoxConnection.Size = new System.Drawing.Size(400, 260);
            this.groupBoxConnection.Controls.Add(this.ConnNameLabel);
            this.groupBoxConnection.Controls.Add(this.ConnName);
            this.groupBoxConnection.Controls.Add(this.label1);
            this.groupBoxConnection.Controls.Add(this.ConnTypeComboBox);
            this.groupBoxConnection.Controls.Add(this.ComportBoxLabel);
            this.groupBoxConnection.Controls.Add(this.ComportBox);
            this.groupBoxConnection.Controls.Add(this.UDPPortLbl);
            this.groupBoxConnection.Controls.Add(this.UDPPortTextBox);
            this.groupBoxConnection.Controls.Add(this.BaudRateBoxLabel);
            this.groupBoxConnection.Controls.Add(this.BaudComboBox);
            this.groupBoxConnection.Controls.Add(this.ModeBoxLabel);
            this.groupBoxConnection.Controls.Add(this.ModeComboBox);
            this.groupBoxConnection.Controls.Add(this.BtnSaveConn);
            // 
            // groupBoxChannels
            // 
            this.groupBoxChannels.Text = "Channel Configuration";
            this.groupBoxChannels.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxChannels.Location = new System.Drawing.Point(30, 310);
            this.groupBoxChannels.Size = new System.Drawing.Size(400, 300);
            this.groupBoxChannels.Controls.Add(this.panelChannelCard);
            // 
            // panelChannelCard
            // 
            this.panelChannelCard.Location = new System.Drawing.Point(10, 25);
            this.panelChannelCard.Size = new System.Drawing.Size(380, 260);
            this.panelChannelCard.BackColor = System.Drawing.Color.White;
            this.panelChannelCard.Padding = new System.Windows.Forms.Padding(15);
            this.panelChannelCard.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.panelChannelCard.Controls.Add(this.dgvChannels);
            this.panelChannelCard.Controls.Add(this.btnSaveChannels);
            this.panelChannelCard.Paint += new System.Windows.Forms.PaintEventHandler(this.panelChannelCard_Paint);
            // 
            // dgvChannels
            // 
            this.dgvChannels.Location = new System.Drawing.Point(15, 15);
            this.dgvChannels.Size = new System.Drawing.Size(330, 150);
            this.dgvChannels.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.dgvChannels.AllowUserToAddRows = true;
            this.dgvChannels.AllowUserToDeleteRows = true;
            this.dgvChannels.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChannels.Columns.Add("ChannelName", "Channel Name");
            var colorCol = new System.Windows.Forms.DataGridViewButtonColumn();
            colorCol.Name = "ChannelColor";
            colorCol.HeaderText = "Color";
            colorCol.Text = "Pick Color";
            colorCol.UseColumnTextForButtonValue = true;
            this.dgvChannels.Columns.Add(colorCol);
            this.dgvChannels.Columns[0].Width = 200;
            this.dgvChannels.Columns[1].Width = 100;
            // 
            // btnSaveChannels
            // 
            this.btnSaveChannels.Text = "Save Channels";
            this.btnSaveChannels.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveChannels.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.btnSaveChannels.ForeColor = System.Drawing.Color.White;
            this.btnSaveChannels.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSaveChannels.Size = new System.Drawing.Size(120, 32);
            this.btnSaveChannels.Location = new System.Drawing.Point(155, 180);
            // 
            // groupBoxConfigs
            // 
            this.groupBoxConfigs.Text = "Saved Configurations";
            this.groupBoxConfigs.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxConfigs.Location = new System.Drawing.Point(460, 30);
            this.groupBoxConfigs.Size = new System.Drawing.Size(350, 580);
            this.groupBoxConfigs.Controls.Add(this.ConfigListLabel);
            this.groupBoxConfigs.Controls.Add(this.configList);
            // 
            // configList
            // 
            this.configList.Location = new System.Drawing.Point(15, 40);
            this.configList.Size = new System.Drawing.Size(320, 500);
            // 
            // ConfigListLabel
            // 
            this.ConfigListLabel.Location = new System.Drawing.Point(15, 20);
            this.ConfigListLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            // 
            // BtnSaveConn
            // 
            this.BtnSaveConn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnSaveConn.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.BtnSaveConn.ForeColor = System.Drawing.Color.White;
            this.BtnSaveConn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.BtnSaveConn.Size = new System.Drawing.Size(80, 32);
            this.BtnSaveConn.Location = new System.Drawing.Point(300, 210);
            // 
            // ConnName, ConnTypeComboBox, ComportBox, UDPPortTextBox, BaudComboBox, ModeComboBox
            // (Set consistent font, size, and spacing for all input controls)
            this.ConnName.Font = this.ConnTypeComboBox.Font = this.ComportBox.Font = this.UDPPortTextBox.Font = this.BaudComboBox.Font = this.ModeComboBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ConnName.Size = new System.Drawing.Size(200, 24);
            this.ConnName.Location = new System.Drawing.Point(160, 30);
            this.ConnTypeComboBox.Size = new System.Drawing.Size(120, 24);
            this.ConnTypeComboBox.Location = new System.Drawing.Point(160, 65);
            this.ComportBox.Size = new System.Drawing.Size(120, 24);
            this.ComportBox.Location = new System.Drawing.Point(160, 100);
            this.UDPPortTextBox.Size = new System.Drawing.Size(120, 24);
            this.UDPPortTextBox.Location = new System.Drawing.Point(160, 100);
            this.BaudComboBox.Size = new System.Drawing.Size(120, 24);
            this.BaudComboBox.Location = new System.Drawing.Point(160, 135);
            this.ModeComboBox.Size = new System.Drawing.Size(120, 24);
            this.ModeComboBox.Location = new System.Drawing.Point(160, 170);
            // 
            // Labels
            // 
            this.ConnNameLabel.Location = new System.Drawing.Point(20, 30);
            this.ConnNameLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.Location = new System.Drawing.Point(20, 65);
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ComportBoxLabel.Location = new System.Drawing.Point(20, 100);
            this.ComportBoxLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.UDPPortLbl.Location = new System.Drawing.Point(20, 100);
            this.UDPPortLbl.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.BaudRateBoxLabel.Location = new System.Drawing.Point(20, 135);
            this.BaudRateBoxLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ModeBoxLabel.Location = new System.Drawing.Point(20, 170);
            this.ModeBoxLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            // 
            // toolTip1
            // 
            this.toolTip1.SetToolTip(this.ConnName, "Unique name for this connection");
            this.toolTip1.SetToolTip(this.ConnTypeComboBox, "Select UART or UDP");
            this.toolTip1.SetToolTip(this.ComportBox, "Select COM port for UART");
            this.toolTip1.SetToolTip(this.UDPPortTextBox, "Enter UDP port number");
            this.toolTip1.SetToolTip(this.BaudComboBox, "Select baud rate");
            this.toolTip1.SetToolTip(this.ModeComboBox, "Select data mode");
            // 
            // mainTableLayout
            // 
            this.mainTableLayout.ColumnCount = 3;
            this.mainTableLayout.RowCount = 1;
            this.mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayout.Controls.Add(this.groupBoxConnection, 0, 0);
            this.mainTableLayout.Controls.Add(this.groupBoxChannels, 1, 0);
            this.mainTableLayout.Controls.Add(this.groupBoxConfigs, 2, 0);
            //
            // groupBoxConnection
            //
            this.groupBoxConnection.Dock = System.Windows.Forms.DockStyle.Fill;
            //
            // groupBoxChannels
            //
            this.groupBoxChannels.Dock = System.Windows.Forms.DockStyle.Fill;
            //
            // groupBoxConfigs
            //
            this.groupBoxConfigs.Dock = System.Windows.Forms.DockStyle.Fill;
            //
            // UserControl1
            //
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mainTableLayout);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(1100, 650);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChannels)).EndInit();
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
        private System.Windows.Forms.ComboBox ConnTypeComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label UDPPortLbl;
        private System.Windows.Forms.TextBox UDPPortTextBox;
        private System.Windows.Forms.GroupBox groupBoxConnection;
        private System.Windows.Forms.GroupBox groupBoxChannels;
        private System.Windows.Forms.GroupBox groupBoxConfigs;
        private System.Windows.Forms.DataGridView dgvChannels;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panelChannelCard;
        private System.Windows.Forms.Button btnSaveChannels;
    }
}
