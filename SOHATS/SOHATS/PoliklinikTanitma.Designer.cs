namespace SOHATS
{
    partial class PoliklinikTanitma
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
            this.label1 = new System.Windows.Forms.Label();
            this.chkGecerli = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAciklama = new System.Windows.Forms.TextBox();
            this.btnKaydet = new System.Windows.Forms.Button();
            this.btnSil = new System.Windows.Forms.Button();
            this.btnCikis = new System.Windows.Forms.Button();
            this.grpBilgiler = new System.Windows.Forms.GroupBox();
            this.cmbPoliklinikAdi = new System.Windows.Forms.ComboBox();
            this.grpBilgiler.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(183, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Poliklinik Adı";
            // 
            // chkGecerli
            // 
            this.chkGecerli.AutoSize = true;
            this.chkGecerli.Location = new System.Drawing.Point(400, 91);
            this.chkGecerli.Name = "chkGecerli";
            this.chkGecerli.Size = new System.Drawing.Size(91, 17);
            this.chkGecerli.TabIndex = 2;
            this.chkGecerli.Text = "Geçerli / Aktif";
            this.chkGecerli.UseVisualStyleBackColor = true;
            this.chkGecerli.CheckedChanged += new System.EventHandler(this.chkGecerli_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Açıklama";
            // 
            // txtAciklama
            // 
            this.txtAciklama.Location = new System.Drawing.Point(62, 19);
            this.txtAciklama.Multiline = true;
            this.txtAciklama.Name = "txtAciklama";
            this.txtAciklama.Size = new System.Drawing.Size(221, 89);
            this.txtAciklama.TabIndex = 4;
            // 
            // btnKaydet
            // 
            this.btnKaydet.Location = new System.Drawing.Point(6, 124);
            this.btnKaydet.Name = "btnKaydet";
            this.btnKaydet.Size = new System.Drawing.Size(115, 23);
            this.btnKaydet.TabIndex = 5;
            this.btnKaydet.Text = "Kaydet / Güncelle";
            this.btnKaydet.UseVisualStyleBackColor = true;
            this.btnKaydet.Click += new System.EventHandler(this.btnKaydet_Click);
            // 
            // btnSil
            // 
            this.btnSil.Location = new System.Drawing.Point(127, 124);
            this.btnSil.Name = "btnSil";
            this.btnSil.Size = new System.Drawing.Size(75, 23);
            this.btnSil.TabIndex = 6;
            this.btnSil.Text = "Sil";
            this.btnSil.UseVisualStyleBackColor = true;
            this.btnSil.Click += new System.EventHandler(this.btnSil_Click);
            // 
            // btnCikis
            // 
            this.btnCikis.Location = new System.Drawing.Point(208, 124);
            this.btnCikis.Name = "btnCikis";
            this.btnCikis.Size = new System.Drawing.Size(75, 23);
            this.btnCikis.TabIndex = 7;
            this.btnCikis.Text = "Kapat";
            this.btnCikis.UseVisualStyleBackColor = true;
            this.btnCikis.Click += new System.EventHandler(this.btnCikis_Click);
            // 
            // grpBilgiler
            // 
            this.grpBilgiler.Controls.Add(this.btnCikis);
            this.grpBilgiler.Controls.Add(this.label2);
            this.grpBilgiler.Controls.Add(this.btnSil);
            this.grpBilgiler.Controls.Add(this.txtAciklama);
            this.grpBilgiler.Controls.Add(this.btnKaydet);
            this.grpBilgiler.Location = new System.Drawing.Point(193, 131);
            this.grpBilgiler.Name = "grpBilgiler";
            this.grpBilgiler.Size = new System.Drawing.Size(305, 153);
            this.grpBilgiler.TabIndex = 8;
            this.grpBilgiler.TabStop = false;
            // 
            // cmbPoliklinikAdi
            // 
            this.cmbPoliklinikAdi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPoliklinikAdi.FormattingEnabled = true;
            this.cmbPoliklinikAdi.Location = new System.Drawing.Point(255, 89);
            this.cmbPoliklinikAdi.Name = "cmbPoliklinikAdi";
            this.cmbPoliklinikAdi.Size = new System.Drawing.Size(121, 21);
            this.cmbPoliklinikAdi.TabIndex = 9;
            // 
            // PoliklinikTanitma
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cmbPoliklinikAdi);
            this.Controls.Add(this.chkGecerli);
            this.Controls.Add(this.grpBilgiler);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PoliklinikTanitma";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PoliklinikTanitma";
            this.Load += new System.EventHandler(this.PoliklinikTanitma_Load);
            this.grpBilgiler.ResumeLayout(false);
            this.grpBilgiler.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkGecerli;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAciklama;
        private System.Windows.Forms.Button btnKaydet;
        private System.Windows.Forms.Button btnSil;
        private System.Windows.Forms.Button btnCikis;
        private System.Windows.Forms.GroupBox grpBilgiler;
        private System.Windows.Forms.ComboBox cmbPoliklinikAdi;
    }
}