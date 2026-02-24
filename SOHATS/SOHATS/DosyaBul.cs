using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SOHATS
{
    public partial class DosyaBul : Form
    {
        public DosyaBul()
        {
            InitializeComponent();
        }

        SQLBaglantisi bgl = new SQLBaglantisi();
        public string secilenDosyaNo = "";

        private void DosyaBul_Load(object sender, EventArgs e)
        {
            // 1. ComboBox Seçeneklerini Doldur
            cmbKriter.Items.Add("Hasta Adı Soyadı");
            cmbKriter.Items.Add("Dosya No");
            cmbKriter.Items.Add("TC Kimlik No");

            // Varsayılan olarak ilkini seç
            cmbKriter.SelectedIndex = 0;

            // Form açılınca tüm listeyi getir
            Listele();
        }

        // --- ARAMA VE LİSTELEME FONKSİYONU ---
        void Listele()
        {
            try
            {
                DataTable dt = new DataTable();
                SqlConnection baglanti = bgl.Baglanti();
                string sql = "";

                // Kullanıcı boş bıraktıysa hepsini getir (veya sadece seçileni, mantık sana kalmış)
                // Burada "arama yapıyorsa filtrele, yapmıyorsa hepsini göster" mantığı kuruyoruz.

                string aranan = txtAranan.Text.Trim();

                if (cmbKriter.Text == "Hasta Adı Soyadı")
                {
                    // Ad ve Soyad sütunlarını birleştirip arama yapıyoruz
                    sql = "Select dosyano as [DOSYA NO], ad as [AD], soyad as [SOYAD], tcno as [TC NO], kurumadi as [KURUM] " +
                          "From hastalar where (ad + ' ' + soyad) LIKE '%" + aranan + "%'";
                }
                else if (cmbKriter.Text == "Dosya No")
                {
                    sql = "Select dosyano as [DOSYA NO], ad as [AD], soyad as [SOYAD], tcno as [TC NO], kurumadi as [KURUM] " +
                          "From hastalar where dosyano LIKE '%" + aranan + "%'";
                }
                else if (cmbKriter.Text == "TC Kimlik No")
                {
                    sql = "Select dosyano as [DOSYA NO], ad as [AD], soyad as [SOYAD], tcno as [TC NO], kurumadi as [KURUM] " +
                          "From hastalar where tcno LIKE '%" + aranan + "%'";
                }
                else
                {
                    // Hiçbiri seçili değilse varsayılan
                    sql = "Select dosyano as [DOSYA NO], ad as [AD], soyad as [SOYAD], tcno as [TC NO], kurumadi as [KURUM] From hastalar";
                }

                SqlDataAdapter da = new SqlDataAdapter(sql, baglanti);
                da.Fill(dt);
                gridBul.DataSource = dt;
                baglanti.Close();
            }
            catch (Exception ex)
            {
                // Boş aramalarda bazen SQL syntax hatası olabilir, kullanıcıya yansıtmayalım.
            }
        }

        // --- YAZI YAZILDIKÇA ARA ---
        private void txtAranan_TextChanged(object sender, EventArgs e)
        {
            Listele();
        }

        // --- KRİTER DEĞİŞİNCE LİSTEYİ YENİLE ---
        private void cmbKriter_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtAranan.Text = ""; // Kriter değişince kutuyu temizle
            Listele(); // Listeyi sıfırla
            txtAranan.Focus();
        }

        // --- ÇİFT TIKLAYINCA SEÇ VE KAPAT ---
        private void gridBul_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gridBul.CurrentRow != null)
            {
                // Seçilen satırdaki 'DOSYA NO' sütununu al
                // Not: SQL sorgusunda alias verdik (as [DOSYA NO]), o yüzden ismi bu.
                secilenDosyaNo = gridBul.CurrentRow.Cells["DOSYA NO"].Value.ToString();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}