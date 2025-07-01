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
            this.components = new System.ComponentModel.Container();
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
            this.SyncByteLabel = new System.Windows.Forms.Label();
            this.SyncByteTextBox = new System.Windows.Forms.TextBox();
            this.groupBoxConnection = new System.Windows.Forms.GroupBox();
            this.groupBoxChannels = new System.Windows.Forms.GroupBox();
            this.panelChannelCard = new System.Windows.Forms.Panel();
            this.dgvChannels = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChannelType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ChannelColor = new System.Windows.Forms.DataGridViewButtonColumn();
            this.btnSaveChannels = new System.Windows.Forms.Button();
            this.groupBoxConfigs = new System.Windows.Forms.GroupBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxConnection.SuspendLayout();
            this.groupBoxChannels.SuspendLayout();
            this.panelChannelCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChannels)).BeginInit();
            this.groupBoxConfigs.SuspendLayout();
            this.mainTableLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // ComportBox
            // 
            this.ComportBox.FormattingEnabled = true;
            this.ComportBox.Location = new System.Drawing.Point(160, 100);
            this.ComportBox.Margin = new System.Windows.Forms.Padding(2);
            this.ComportBox.Name = "ComportBox";
            this.ComportBox.Size = new System.Drawing.Size(120, 25);
            this.ComportBox.TabIndex = 0;
            this.toolTip1.SetToolTip(this.ComportBox, "Select COM port for UART");
            // 
            // BtnSaveConn
            // 
            this.BtnSaveConn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.BtnSaveConn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnSaveConn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.BtnSaveConn.ForeColor = System.Drawing.Color.White;
            this.BtnSaveConn.Location = new System.Drawing.Point(174, 270);
            this.BtnSaveConn.Margin = new System.Windows.Forms.Padding(2);
            this.BtnSaveConn.Name = "BtnSaveConn";
            this.BtnSaveConn.Size = new System.Drawing.Size(80, 32);
            this.BtnSaveConn.TabIndex = 2;
            this.BtnSaveConn.Text = "Save";
            this.BtnSaveConn.UseVisualStyleBackColor = false;
            this.BtnSaveConn.Click += new System.EventHandler(this.BtnSaveConn_Click);
            // 
            // ConnName
            // 
            this.ConnName.Location = new System.Drawing.Point(160, 30);
            this.ConnName.Margin = new System.Windows.Forms.Padding(2);
            this.ConnName.Name = "ConnName";
            this.ConnName.Size = new System.Drawing.Size(200, 25);
            this.ConnName.TabIndex = 3;
            this.toolTip1.SetToolTip(this.ConnName, "Unique name for this connection");
            // 
            // configList
            // 
            this.configList.FormattingEnabled = true;
            this.configList.ItemHeight = 17;
            this.configList.Location = new System.Drawing.Point(15, 40);
            this.configList.Margin = new System.Windows.Forms.Padding(2);
            this.configList.Name = "configList";
            this.configList.Size = new System.Drawing.Size(320, 497);
            this.configList.TabIndex = 4;
            // 
            // ModeComboBox
            // 
            this.ModeComboBox.FormattingEnabled = true;
            this.ModeComboBox.Location = new System.Drawing.Point(160, 170);
            this.ModeComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.ModeComboBox.Name = "ModeComboBox";
            this.ModeComboBox.Size = new System.Drawing.Size(120, 25);
            this.ModeComboBox.TabIndex = 5;
            this.toolTip1.SetToolTip(this.ModeComboBox, "Select data mode");
            // 
            // ConnNameLabel
            // 
            this.ConnNameLabel.AutoSize = true;
            this.ConnNameLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ConnNameLabel.Location = new System.Drawing.Point(20, 30);
            this.ConnNameLabel.Name = "ConnNameLabel";
            this.ConnNameLabel.Size = new System.Drawing.Size(130, 15);
            this.ConnNameLabel.TabIndex = 6;
            this.ConnNameLabel.Text = "Enter connection name";
            // 
            // ModeBoxLabel
            // 
            this.ModeBoxLabel.AutoSize = true;
            this.ModeBoxLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ModeBoxLabel.Location = new System.Drawing.Point(20, 170);
            this.ModeBoxLabel.Name = "ModeBoxLabel";
            this.ModeBoxLabel.Size = new System.Drawing.Size(81, 15);
            this.ModeBoxLabel.TabIndex = 7;
            this.ModeBoxLabel.Text = "Choose Mode";
            // 
            // BaudRateBoxLabel
            // 
            this.BaudRateBoxLabel.AutoSize = true;
            this.BaudRateBoxLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.BaudRateBoxLabel.Location = new System.Drawing.Point(20, 135);
            this.BaudRateBoxLabel.Name = "BaudRateBoxLabel";
            this.BaudRateBoxLabel.Size = new System.Drawing.Size(103, 15);
            this.BaudRateBoxLabel.TabIndex = 8;
            this.BaudRateBoxLabel.Text = "Choose Baud Rate";
            // 
            // ComportBoxLabel
            // 
            this.ComportBoxLabel.AutoSize = true;
            this.ComportBoxLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ComportBoxLabel.Location = new System.Drawing.Point(20, 100);
            this.ComportBoxLabel.Name = "ComportBoxLabel";
            this.ComportBoxLabel.Size = new System.Drawing.Size(103, 15);
            this.ComportBoxLabel.TabIndex = 9;
            this.ComportBoxLabel.Text = "Choose COM Port";
            // 
            // BaudComboBox
            // 
            this.BaudComboBox.FormattingEnabled = true;
            this.BaudComboBox.Location = new System.Drawing.Point(160, 135);
            this.BaudComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.BaudComboBox.Name = "BaudComboBox";
            this.BaudComboBox.Size = new System.Drawing.Size(120, 25);
            this.BaudComboBox.TabIndex = 10;
            this.toolTip1.SetToolTip(this.BaudComboBox, "Select baud rate");
            // 
            // ConfigListLabel
            // 
            this.ConfigListLabel.AutoSize = true;
            this.ConfigListLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.ConfigListLabel.Location = new System.Drawing.Point(15, 20);
            this.ConfigListLabel.Name = "ConfigListLabel";
            this.ConfigListLabel.Size = new System.Drawing.Size(105, 15);
            this.ConfigListLabel.TabIndex = 11;
            this.ConfigListLabel.Text = "Configuration List";
            // 
            // ConnTypeComboBox
            // 
            this.ConnTypeComboBox.FormattingEnabled = true;
            this.ConnTypeComboBox.Location = new System.Drawing.Point(160, 65);
            this.ConnTypeComboBox.Name = "ConnTypeComboBox";
            this.ConnTypeComboBox.Size = new System.Drawing.Size(120, 25);
            this.ConnTypeComboBox.TabIndex = 12;
            this.toolTip1.SetToolTip(this.ConnTypeComboBox, "Select UART or UDP");
            this.ConnTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.ConnTypeComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.Location = new System.Drawing.Point(20, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 15);
            this.label1.TabIndex = 13;
            this.label1.Text = "Choose Connection Type";
            // 
            // UDPPortLbl
            // 
            this.UDPPortLbl.AutoSize = true;
            this.UDPPortLbl.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.UDPPortLbl.Location = new System.Drawing.Point(20, 100);
            this.UDPPortLbl.Name = "UDPPortLbl";
            this.UDPPortLbl.Size = new System.Drawing.Size(85, 15);
            this.UDPPortLbl.TabIndex = 15;
            this.UDPPortLbl.Text = "Enter UDP Port";
            // 
            // UDPPortTextBox
            // 
            this.UDPPortTextBox.Location = new System.Drawing.Point(160, 100);
            this.UDPPortTextBox.Name = "UDPPortTextBox";
            this.UDPPortTextBox.Size = new System.Drawing.Size(120, 25);
            this.UDPPortTextBox.TabIndex = 16;
            this.toolTip1.SetToolTip(this.UDPPortTextBox, "Enter UDP port number");
            // 
            // SyncByteLabel
            // 
            this.SyncByteLabel.AutoSize = true;
            this.SyncByteLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SyncByteLabel.Location = new System.Drawing.Point(20, 214);
            this.SyncByteLabel.Name = "SyncByteLabel";
            this.SyncByteLabel.Size = new System.Drawing.Size(88, 15);
            this.SyncByteLabel.TabIndex = 17;
            this.SyncByteLabel.Text = "Enter Sync Byte";
            // 
            // SyncByteTextBox
            // 
            this.SyncByteTextBox.Location = new System.Drawing.Point(160, 210);
            this.SyncByteTextBox.Name = "SyncByteTextBox";
            this.SyncByteTextBox.Size = new System.Drawing.Size(120, 25);
            this.SyncByteTextBox.TabIndex = 18;
            // 
            // groupBoxConnection
            // 
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
            this.groupBoxConnection.Controls.Add(this.SyncByteLabel);
            this.groupBoxConnection.Controls.Add(this.SyncByteTextBox);
            this.groupBoxConnection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxConnection.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxConnection.Location = new System.Drawing.Point(3, 3);
            this.groupBoxConnection.Name = "groupBoxConnection";
            this.groupBoxConnection.Size = new System.Drawing.Size(357, 644);
            this.groupBoxConnection.TabIndex = 0;
            this.groupBoxConnection.TabStop = false;
            this.groupBoxConnection.Text = "Connection Settings";
            // 
            // groupBoxChannels
            // 
            this.groupBoxChannels.Controls.Add(this.panelChannelCard);
            this.groupBoxChannels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxChannels.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxChannels.Location = new System.Drawing.Point(366, 3);
            this.groupBoxChannels.Name = "groupBoxChannels";
            this.groupBoxChannels.Size = new System.Drawing.Size(368, 644);
            this.groupBoxChannels.TabIndex = 1;
            this.groupBoxChannels.TabStop = false;
            this.groupBoxChannels.Text = "Channel Configuration";
            // 
            // panelChannelCard
            // 
            this.panelChannelCard.BackColor = System.Drawing.Color.White;
            this.panelChannelCard.Controls.Add(this.dgvChannels);
            this.panelChannelCard.Controls.Add(this.btnSaveChannels);
            this.panelChannelCard.Location = new System.Drawing.Point(10, 25);
            this.panelChannelCard.Name = "panelChannelCard";
            this.panelChannelCard.Padding = new System.Windows.Forms.Padding(15);
            this.panelChannelCard.Size = new System.Drawing.Size(380, 260);
            this.panelChannelCard.TabIndex = 0;
            this.panelChannelCard.Paint += new System.Windows.Forms.PaintEventHandler(this.panelChannelCard_Paint);
            // 
            // dgvChannels
            // 
            this.dgvChannels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvChannels.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChannels.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.ChannelType,
            this.ChannelColor});
            this.dgvChannels.Location = new System.Drawing.Point(15, 15);
            this.dgvChannels.Name = "dgvChannels";
            this.dgvChannels.Size = new System.Drawing.Size(330, 150);
            this.dgvChannels.TabIndex = 0;
            //
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Channel Name";
            this.dataGridViewTextBoxColumn1.Name = "ChannelName";
            // 
            // ChannelType
            // 
            this.ChannelType.HeaderText = "Channel Type";
            this.ChannelType.Name = "ChannelType";
            this.ChannelType.Items.AddRange(new object[] {
                "int8",
                "int16",
                "int32",
                "int64",
                "uint8",
                "uint16",
                "uint32",
                "uint64"
            });
            // 
            // ChannelColor
            // 
            this.ChannelColor.HeaderText = "Color";
            this.ChannelColor.Name = "ChannelColor";
            this.ChannelColor.Text = "Pick Color";
            this.ChannelColor.UseColumnTextForButtonValue = true;
            // 
            // btnSaveChannels
            // 
            this.btnSaveChannels.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnSaveChannels.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveChannels.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSaveChannels.ForeColor = System.Drawing.Color.White;
            this.btnSaveChannels.Location = new System.Drawing.Point(155, 180);
            this.btnSaveChannels.Name = "btnSaveChannels";
            this.btnSaveChannels.Size = new System.Drawing.Size(120, 32);
            this.btnSaveChannels.TabIndex = 1;
            this.btnSaveChannels.Text = "Save Channels";
            this.btnSaveChannels.UseVisualStyleBackColor = false;
            // 
            // groupBoxConfigs
            // 
            this.groupBoxConfigs.Controls.Add(this.ConfigListLabel);
            this.groupBoxConfigs.Controls.Add(this.configList);
            this.groupBoxConfigs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxConfigs.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxConfigs.Location = new System.Drawing.Point(740, 3);
            this.groupBoxConfigs.Name = "groupBoxConfigs";
            this.groupBoxConfigs.Size = new System.Drawing.Size(357, 644);
            this.groupBoxConfigs.TabIndex = 2;
            this.groupBoxConfigs.TabStop = false;
            this.groupBoxConfigs.Text = "Saved Configurations";
            // 
            // mainTableLayout
            // 
            this.mainTableLayout.ColumnCount = 3;
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.mainTableLayout.Controls.Add(this.groupBoxConnection, 0, 0);
            this.mainTableLayout.Controls.Add(this.groupBoxChannels, 1, 0);
            this.mainTableLayout.Controls.Add(this.groupBoxConfigs, 2, 0);
            this.mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayout.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayout.Name = "mainTableLayout";
            this.mainTableLayout.RowCount = 1;
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayout.Size = new System.Drawing.Size(1100, 650);
            this.mainTableLayout.TabIndex = 0;
            // 
            // UserControl1
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mainTableLayout);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(1100, 650);
            this.groupBoxConnection.ResumeLayout(false);
            this.groupBoxConnection.PerformLayout();
            this.groupBoxChannels.ResumeLayout(false);
            this.panelChannelCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChannels)).EndInit();
            this.groupBoxConfigs.ResumeLayout(false);
            this.groupBoxConfigs.PerformLayout();
            this.mainTableLayout.ResumeLayout(false);
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Label SyncByteLabel;
        private System.Windows.Forms.TextBox SyncByteTextBox;
        private System.Windows.Forms.GroupBox groupBoxConnection;
        private System.Windows.Forms.GroupBox groupBoxChannels;
        private System.Windows.Forms.GroupBox groupBoxConfigs;
        private System.Windows.Forms.DataGridView dgvChannels;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panelChannelCard;
        private System.Windows.Forms.Button btnSaveChannels;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewComboBoxColumn ChannelType;
        private System.Windows.Forms.DataGridViewButtonColumn ChannelColor;
    }
}
