using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SOHATS
{
    public partial class HastaBilgileri : Form
    {
        public HastaBilgileri()
        {
            InitializeComponent();
        }

        SQLBaglantisi bgl = new SQLBaglantisi();

        private void HastaBilgileri_Load(object sender, EventArgs e)
        {
            // Form açılır açılmaz Yeni Dosya Numarasını hesapla
            YeniDosyaNoHesapla();

            // txtKullaniciKodu (Dosya No) elle değiştirilmesin
            txtKullaniciKodu.Enabled = false;
        }

        // --- YENİ DOSYA NO HESAPLAMA ---
        void YeniDosyaNoHesapla()
        {
            try
            {
                SqlConnection baglanti = bgl.Baglanti();
                // En büyük dosya numarasını buluyoruz
                SqlCommand komut = new SqlCommand("Select MAX(CAST(dosyano AS INT)) From hastalar", baglanti);

                object sonuc = komut.ExecuteScalar();
                baglanti.Close();

                if (sonuc != DBNull.Value && sonuc != null)
                {
                    // Varsa 1 arttır
                    int yeniNo = Convert.ToInt32(sonuc) + 1;
                    txtKullaniciKodu.Text = yeniNo.ToString();
                }
                else
                {
                    // Hiç kayıt yoksa 1 yap
                    txtKullaniciKodu.Text = "1";
                }
            }
            catch
            {
                txtKullaniciKodu.Text = "1";
            }
        }

        // --- KAYDET BUTONU ---
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            // Zorunlu alan kontrolü
            if (txtAd.Text == "" || txtSoyad.Text == "")
            {
                MessageBox.Show("Lütfen en azından İsim ve Soyisim giriniz.");
                return;
            }

            try
            {
                SqlConnection baglanti = bgl.Baglanti();

                // Tasarımındaki TÜM alanları kapsayan SQL sorgusu
                string sql = "insert into hastalar (dosyano, tcno, ad, soyad, dogumyeri, babaadi, anneadi, telefon, dogumtarihi, cinsiyet, medeni_hal, kan_grubu, adres, yakin_adi, yakin_tel) " +
                             "values (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15)";

                SqlCommand komut = new SqlCommand(sql, baglanti);

                // Designer isimlerine göre atamalar:
                komut.Parameters.AddWithValue("@p1", txtKullaniciKodu.Text); // Dosya No
                komut.Parameters.AddWithValue("@p2", txtTC.Text);
                komut.Parameters.AddWithValue("@p3", txtAd.Text);
                komut.Parameters.AddWithValue("@p4", txtSoyad.Text);
                komut.Parameters.AddWithValue("@p5", txtDogumYeri.Text);
                komut.Parameters.AddWithValue("@p6", txtBabaAdi.Text);
                komut.Parameters.AddWithValue("@p7", txtAnneAdi.Text);
                komut.Parameters.AddWithValue("@p8", txtEvTel.Text);
                komut.Parameters.AddWithValue("@p9", dateDogum.Value.ToString("yyyy-MM-dd"));
                komut.Parameters.AddWithValue("@p10", cmbCinsiyet.Text);
                komut.Parameters.AddWithValue("@p11", cmbMedeniHal.Text);
                komut.Parameters.AddWithValue("@p12", cmbKanGrubu.Text);
                komut.Parameters.AddWithValue("@p13", txtAdres.Text);
                komut.Parameters.AddWithValue("@p14", txtYakin.Text);
                komut.Parameters.AddWithValue("@p15", txtYakinTel.Text);

                komut.ExecuteNonQuery();
                baglanti.Close();

                MessageBox.Show("Hasta kaydı başarıyla oluşturuldu.\nDosya No: " + txtKullaniciKodu.Text, "Bilgi");

                this.Close(); // İşlem bitince formu kapat
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kaydetme hatası: " + ex.Message);
            }
        }

        // --- SİL BUTONU ---
        private void btnSil_Click(object sender, EventArgs e)
        {
            DialogResult cevap = MessageBox.Show("Bu hasta kaydını silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (cevap == DialogResult.Yes)
            {
                try
                {
                    SqlConnection baglanti = bgl.Baglanti();
                    SqlCommand komut = new SqlCommand("Delete From hastalar where dosyano=@p1", baglanti);
                    komut.Parameters.AddWithValue("@p1", txtKullaniciKodu.Text);
                    komut.ExecuteNonQuery();
                    baglanti.Close();

                    MessageBox.Show("Kayıt silindi.");
                    Temizle(); // Formu temizle ve yeni numara ver
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Silme hatası: " + ex.Message);
                }
            }
        }

        // --- TEMİZLE BUTONU ---
        private void btnTemizle_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        void Temizle()
        {
            // GroupBox içindeki tüm Textboxları temizle
            foreach (Control item in grpDetay.Controls)
            {
                if (item is TextBox) item.Text = "";
                if (item is ComboBox) (item as ComboBox).SelectedIndex = -1;
                if (item is DateTimePicker) (item as DateTimePicker).Value = DateTime.Now;
            }

            // Yeni numara hesapla
            YeniDosyaNoHesapla();
        }

        // --- ÇIKIŞ BUTONU ---
        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}