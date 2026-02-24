namespace SOHATS
{
    partial class DosyaBul
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtAranan = new System.Windows.Forms.TextBox();
            this.cmbKriter = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gridBul = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridBul)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtAranan);
            this.panel1.Controls.Add(this.cmbKriter);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 34);
            this.panel1.TabIndex = 0;
            // 
            // txtAranan
            // 
            this.txtAranan.Location = new System.Drawing.Point(240, 7);
            this.txtAranan.Name = "txtAranan";
            this.txtAranan.Size = new System.Drawing.Size(100, 20);
            this.txtAranan.TabIndex = 2;
            this.txtAranan.TextChanged += new System.EventHandler(this.txtAranan_TextChanged);
            // 
            // cmbKriter
            // 
            this.cmbKriter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKriter.FormattingEnabled = true;
            this.cmbKriter.Location = new System.Drawing.Point(101, 6);
            this.cmbKriter.Name = "cmbKriter";
            this.cmbKriter.Size = new System.Drawing.Size(121, 21);
            this.cmbKriter.TabIndex = 1;
            this.cmbKriter.SelectedIndexChanged += new System.EventHandler(this.cmbKriter_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Arama Kriteri";
            // 
            // gridBul
            // 
            this.gridBul.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridBul.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridBul.Location = new System.Drawing.Point(0, 34);
            this.gridBul.Name = "gridBul";
            this.gridBul.ReadOnly = true;
            this.gridBul.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridBul.Size = new System.Drawing.Size(800, 416);
            this.gridBul.TabIndex = 1;
            this.gridBul.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridBul_CellDoubleClick);
            // 
            // DosyaBul
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.gridBul);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DosyaBul";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dosya Arama Yardımı";
            this.Load += new System.EventHandler(this.DosyaBul_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridBul)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtAranan;
        private System.Windows.Forms.ComboBox cmbKriter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView gridBul;
    }
}