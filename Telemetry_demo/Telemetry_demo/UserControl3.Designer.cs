namespace Telemetry_demo
{
    partial class UserControl3
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
            this.txtRows = new System.Windows.Forms.TextBox();
            this.txtColumns = new System.Windows.Forms.TextBox();
            this.BtnAddGrid = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtRows
            // 
            this.txtRows.Location = new System.Drawing.Point(134, 56);
            this.txtRows.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtRows.Name = "txtRows";
            this.txtRows.Size = new System.Drawing.Size(76, 20);
            this.txtRows.TabIndex = 0;
            // 
            // txtColumns
            // 
            this.txtColumns.Location = new System.Drawing.Point(250, 56);
            this.txtColumns.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtColumns.Name = "txtColumns";
            this.txtColumns.Size = new System.Drawing.Size(76, 20);
            this.txtColumns.TabIndex = 1;
            // 
            // BtnAddGrid
            // 
            this.BtnAddGrid.Location = new System.Drawing.Point(395, 54);
            this.BtnAddGrid.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.BtnAddGrid.Name = "BtnAddGrid";
            this.BtnAddGrid.Size = new System.Drawing.Size(56, 19);
            this.BtnAddGrid.TabIndex = 2;
            this.BtnAddGrid.Text = "Add grid";
            this.BtnAddGrid.UseVisualStyleBackColor = true;
            this.BtnAddGrid.Click += new System.EventHandler(this.BtnAddGrid_Click);
            // 
            // UserControl3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BtnAddGrid);
            this.Controls.Add(this.txtColumns);
            this.Controls.Add(this.txtRows);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "UserControl3";
            this.Size = new System.Drawing.Size(525, 325);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtRows;
        private System.Windows.Forms.TextBox txtColumns;
        private System.Windows.Forms.Button BtnAddGrid;
    }
}
