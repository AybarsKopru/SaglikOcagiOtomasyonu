using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SOHATS
{
    public partial class PoliklinikTanitma : Form
    {
        public PoliklinikTanitma()
        {
            InitializeComponent();
        }

        SQLBaglantisi bgl = new SQLBaglantisi();

        // 1. FORM YÜKLENİRKEN
        private void PoliklinikTanitma_Load(object sender, EventArgs e)
        {
            // Başlangıç ayarları
            chkGecerli.Visible = true;    // Checkbox hep görünsün
            chkGecerli.Checked = false;   // Başta işaretli olmasın
            grpBilgiler.Visible = false;  // Alt taraf gizli olsun

            PoliklinikListesiGetir();     // Listeyi doldur
        }

        // --- EN ÖNEMLİ KISIM BURASI ---
        // Checkbox işaretlendiği AN alt tarafı açan kod
        private void chkGecerli_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGecerli.Checked == true)
            {
                grpBilgiler.Visible = true; // İşaretliyse AÇ
            }
            else
            {
                grpBilgiler.Visible = false; // İşaretli değilse GİZLE
            }
        }

        // Listeyi Doldurma Metodu
        void PoliklinikListesiGetir()
        {
            cmbPoliklinikAdi.Items.Clear();
            try
            {
                SqlConnection baglanti = bgl.Baglanti();
                SqlCommand komut = new SqlCommand("Select poliklinikadi From poliklinik", baglanti);
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    cmbPoliklinikAdi.Items.Add(dr[0].ToString());
                }
                baglanti.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        // 2. COMBOBOX SEÇİMİ DEĞİŞİNCE
        private void cmbPoliklinikAdi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPoliklinikAdi.Text == "") return;
            BilgileriGetir(cmbPoliklinikAdi.Text);
        }

        // 3. ENTER TUŞUNA BASILINCA
        private void cmbPoliklinikAdi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BilgileriGetir(cmbPoliklinikAdi.Text);
            }
        }

        // Ortak Veri Çekme Fonksiyonu
        void BilgileriGetir(string poliklinikAdi)
        {
            try
            {
                // Önce temiz bir sayfa açalım
                chkGecerli.Checked = false; // Bu işlem otomatik olarak alt tarafı gizler (Event sayesinde)
                txtAciklama.Text = "";

                SqlConnection baglanti = bgl.Baglanti();
                SqlCommand komut = new SqlCommand("Select * From poliklinik where poliklinikadi=@p1", baglanti);
                komut.Parameters.AddWithValue("@p1", poliklinikAdi);
                SqlDataReader dr = komut.ExecuteReader();

                if (dr.Read())
                {
                    // --- KAYIT VARSA ---
                    txtAciklama.Text = dr["aciklama"].ToString();

                    // Veritabanındaki duruma göre Checkbox'ı ayarla
                    if (dr["durum"].ToString() == "Geçerli")
                    {
                        chkGecerli.Checked = true;
                        // DİKKAT: Burası 'true' olduğu an yukarıdaki 'CheckedChanged' olayı tetiklenir 
                        // ve alt taraf (grpBilgiler) OTOMATİK OLARAK açılır.
                    }
                    else
                    {
                        chkGecerli.Checked = false;
                        // Burası false ise alt taraf kapalı kalır. Kullanıcı elle açmak zorundadır.
                    }

                    btnKaydet.Text = "Güncelle";
                    btnSil.Enabled = true;
                }
                else
                {
                    // --- KAYIT YOKSA ---
                    DialogResult cevap = MessageBox.Show("Böyle bir poliklinik bulunamadı. Yeni kayıt oluşturmak ister misiniz?",
                        "Kayıt Yok", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (cevap == DialogResult.Yes)
                    {
                        // Evet derse, kutuyu işaretle (Bu da alt tarafı açar)
                        chkGecerli.Checked = true;

                        btnKaydet.Text = "Kaydet";
                        btnSil.Enabled = false;
                        txtAciklama.Focus();
                    }
                }
                baglanti.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        // KAYDET / GÜNCELLE
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbPoliklinikAdi.Text)) return;

            try
            {
                SqlConnection baglanti = bgl.Baglanti();
                // Checkbox işaretliyse Geçerli, değilse Geçersiz
                string durum = chkGecerli.Checked ? "Geçerli" : "Geçersiz";

                if (btnKaydet.Text == "Kaydet")
                {
                    SqlCommand komut = new SqlCommand("insert into poliklinik (poliklinikadi,durum,aciklama) values (@p1,@p2,@p3)", baglanti);
                    komut.Parameters.AddWithValue("@p1", cmbPoliklinikAdi.Text);
                    komut.Parameters.AddWithValue("@p2", durum);
                    komut.Parameters.AddWithValue("@p3", txtAciklama.Text);
                    komut.ExecuteNonQuery();
                    MessageBox.Show("Eklendi.");
                }
                else
                {
                    SqlCommand komut = new SqlCommand("update poliklinik set durum=@p2, aciklama=@p3 where poliklinikadi=@p1", baglanti);
                    komut.Parameters.AddWithValue("@p1", cmbPoliklinikAdi.Text);
                    komut.Parameters.AddWithValue("@p2", durum);
                    komut.Parameters.AddWithValue("@p3", txtAciklama.Text);
                    komut.ExecuteNonQuery();
                    MessageBox.Show("Güncellendi.");
                }
                baglanti.Close();

                // İşlem bitince listeyi yenile ve temizle
                PoliklinikListesiGetir();
                TemizleVeGizle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("İşlem Hatası: " + ex.Message);
            }
        }

        // SİL
        private void btnSil_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbPoliklinikAdi.Text)) return;
            DialogResult cevap = MessageBox.Show("Silinsin mi?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (cevap == DialogResult.Yes)
            {
                try
                {
                    SqlConnection baglanti = bgl.Baglanti();
                    SqlCommand komut = new SqlCommand("delete from poliklinik where poliklinikadi=@p1", baglanti);
                    komut.Parameters.AddWithValue("@p1", cmbPoliklinikAdi.Text);
                    komut.ExecuteNonQuery();
                    baglanti.Close();
                    MessageBox.Show("Silindi.");
                    PoliklinikListesiGetir();
                    TemizleVeGizle();
                }
                catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
            }
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void TemizleVeGizle()
        {
            cmbPoliklinikAdi.Text = "";
            txtAciklama.Text = "";
            chkGecerli.Checked = false; // Bu otomatik olarak gizleyecek
        }
    }
}