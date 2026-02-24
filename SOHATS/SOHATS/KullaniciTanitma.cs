using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SOHATS
{
    public partial class KullaniciTanitma : Form
    {
        public KullaniciTanitma()
        {
            InitializeComponent();
        }

        SQLBaglantisi bgl = new SQLBaglantisi();
        bool yeniKayitModu = false;

        // KİLİT DEĞİŞKENİ: Form yüklenirken kodların çalışmasını engeller
        bool formYukleniyor = false;

        private void KullaniciTanitma_Load(object sender, EventArgs e)
        {
            // 1. KİLİDİ KAPAT (Yükleme başladı)
            formYukleniyor = true;

            grpDetay.Visible = false; // Alt taraf GİZLİ başlasın

            KullaniciListesiGetir(); // Listeyi doldur

            // ComboBox'ın seçili elemanını kaldır
            cmbKullaniciSec.SelectedIndex = -1;

            // 2. KİLİDİ AÇ (Yükleme bitti, artık tıklamaları dinle)
            formYukleniyor = false;
        }

        // --- COMBOBOX SEÇİMİ (ÇİFT TIKLAMA İLE OLUŞTURDUĞUN EVENT) ---
        private void cmbKullaniciSec_SelectedIndexChanged(object sender, EventArgs e)
        {
            // EĞER FORM YÜKLENİYORSA HİÇBİR ŞEY YAPMA (DUR!)
            if (formYukleniyor == true) return;

            // Eğer seçim -1 ise (boşsa) işlem yapma
            if (cmbKullaniciSec.SelectedIndex == -1) return;

            if (cmbKullaniciSec.SelectedValue != null)
            {
                try
                {
                    // Bazen ilk yüklemede değer DataRowView gelir, stringe çevirip kontrol edelim
                    string gelenDeger = cmbKullaniciSec.SelectedValue.ToString();

                    // Gelen değer sayısal bir ID mi? (Örn: "1", "2")
                    int kontrol;
                    if (int.TryParse(gelenDeger, out kontrol))
                    {
                        BilgileriGetir(gelenDeger);
                    }
                }
                catch { }
            }
        }

        // --- YENİ KULLANICI BUTONU ---
        private void btnYeniKullanici_Click(object sender, EventArgs e)
        {
            Temizle();
            grpDetay.Visible = true; // Detayları AÇ
            yeniKayitModu = true;

            btnKaydet.Text = "Kaydet";
            btnSil.Enabled = false;
            txtKullaniciKodu.Text = "Otomatik";
            txtTC.Focus();
        }

        // Helper: Verileri Getir
        void BilgileriGetir(string k_kodu)
        {
            try
            {
                SqlConnection baglanti = bgl.Baglanti();
                SqlCommand komut = new SqlCommand("Select * From kullanicilar where kodu=@p1", baglanti);
                komut.Parameters.AddWithValue("@p1", k_kodu);
                SqlDataReader dr = komut.ExecuteReader();

                if (dr.Read())
                {
                    grpDetay.Visible = true; // Detayları AÇ
                    yeniKayitModu = false;

                    txtKullaniciKodu.Text = dr["kodu"].ToString();
                    txtTC.Text = dr["tcno"].ToString();
                    txtDogumYeri.Text = dr["dogumyeri"].ToString();
                    txtEvTel.Text = dr["evtel"].ToString();
                    txtCepTel.Text = dr["ceptel"].ToString();
                    txtAdres.Text = dr["adres"].ToString();
                    txtAd.Text = dr["ad"].ToString();
                    txtSoyad.Text = dr["soyad"].ToString();
                    txtMaas.Text = dr["maas"].ToString();

                    cmbUnvan.Text = dr["unvan"].ToString();
                    cmbCinsiyet.Text = dr["cinsiyet"].ToString();

                    if (dr["isebaslama"] != DBNull.Value) dateIseBaslama.Value = Convert.ToDateTime(dr["isebaslama"]);
                    if (dr["dogumtarihi"] != DBNull.Value) dateDogum.Value = Convert.ToDateTime(dr["dogumtarihi"]);

                    txtKullaniciAdi.Text = dr["kodu"].ToString();
                    txtSifre.Text = dr["sifre"].ToString();

                    if (dr["yetki"].ToString() == "admin")
                        chkYetkili.Checked = true;
                    else
                        chkYetkili.Checked = false;

                    btnKaydet.Text = "Güncelle";
                    btnSil.Enabled = true;
                }
                baglanti.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        // Helper: ComboBox Doldur
        void KullaniciListesiGetir()
        {
            try
            {
                // DataSource atamadan önce olayları geçici olarak durduruyoruz (Flag ile)
                cmbKullaniciSec.DataSource = null;

                SqlConnection baglanti = bgl.Baglanti();
                SqlDataAdapter da = new SqlDataAdapter("Select kodu, (ad + ' ' + soyad) as AdSoyad From kullanicilar", baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbKullaniciSec.ValueMember = "kodu";
                cmbKullaniciSec.DisplayMember = "AdSoyad";
                cmbKullaniciSec.DataSource = dt;

                baglanti.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Liste hatası: " + ex.Message);
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection baglanti = bgl.Baglanti();
                string yetki = chkYetkili.Checked ? "admin" : "user";
                decimal maas = 0;
                decimal.TryParse(txtMaas.Text, out maas);

                if (yeniKayitModu)
                {
                    // INSERT
                    string sql = "insert into kullanicilar (tcno, dogumyeri, evtel, ceptel, adres, unvan, ad, soyad, maas, isebaslama, dogumtarihi, cinsiyet, sifre, yetki) " +
                                 "values (@p1,@p2,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p17,@p18)";

                    SqlCommand komut = new SqlCommand(sql, baglanti);
                    Parametrele(komut, yetki, maas);
                    komut.ExecuteNonQuery();
                    MessageBox.Show("Kullanıcı eklendi.");
                }
                else
                {
                    // UPDATE
                    string sql = "update kullanicilar set tcno=@p1, dogumyeri=@p2, evtel=@p5, ceptel=@p6, adres=@p7, unvan=@p8, ad=@p9, soyad=@p10, maas=@p11, isebaslama=@p12, dogumtarihi=@p13, cinsiyet=@p14, sifre=@p17, yetki=@p18 where kodu=@kID";

                    SqlCommand komut = new SqlCommand(sql, baglanti);
                    Parametrele(komut, yetki, maas);
                    komut.Parameters.AddWithValue("@kID", txtKullaniciKodu.Text);
                    komut.ExecuteNonQuery();
                    MessageBox.Show("Bilgiler güncellendi.");
                }
                baglanti.Close();

                // Listeyi yenilemek için kilidi tekrar yönet
                formYukleniyor = true;
                KullaniciListesiGetir();
                cmbKullaniciSec.SelectedIndex = -1;
                grpDetay.Visible = false; // İşlem bitince gizle
                formYukleniyor = false;

                Temizle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kaydetme hatası: " + ex.Message);
            }
        }

        void Parametrele(SqlCommand komut, string yetki, decimal maas)
        {
            komut.Parameters.AddWithValue("@p1", txtTC.Text);
            komut.Parameters.AddWithValue("@p2", txtDogumYeri.Text);
            komut.Parameters.AddWithValue("@p5", txtEvTel.Text);
            komut.Parameters.AddWithValue("@p6", txtCepTel.Text);
            komut.Parameters.AddWithValue("@p7", txtAdres.Text);
            komut.Parameters.AddWithValue("@p8", cmbUnvan.Text);
            komut.Parameters.AddWithValue("@p9", txtAd.Text);
            komut.Parameters.AddWithValue("@p10", txtSoyad.Text);
            komut.Parameters.AddWithValue("@p11", maas);
            komut.Parameters.AddWithValue("@p12", dateIseBaslama.Value);
            komut.Parameters.AddWithValue("@p13", dateDogum.Value);
            komut.Parameters.AddWithValue("@p14", cmbCinsiyet.Text);
            komut.Parameters.AddWithValue("@p17", txtSifre.Text);
            komut.Parameters.AddWithValue("@p18", yetki);
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            // Silme işlemi aynen kalacak
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void Temizle()
        {
            foreach (Control item in grpDetay.Controls)
            {
                if (item is TextBox) item.Text = "";
                if (item is ComboBox) (item as ComboBox).SelectedIndex = -1;
                if (item is CheckBox) (item as CheckBox).Checked = false;
                if (item is DateTimePicker) (item as DateTimePicker).Value = DateTime.Now;
            }
        }
    }
}