using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SOHATS
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        SQLBaglantisi bgl = new SQLBaglantisi();

        // TEMİZLE BUTONU
        private void btnTemizle_Click(object sender, EventArgs e)
        {
            txtKullaniciAdi.Text = "";
            txtSifre.Text = "";
            txtKullaniciAdi.Focus();
        }

        // ÇIKIŞ BUTONU
        private void btnCikis_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // GİRİŞ BUTONU
        // GİRİŞ BUTONU
        private void btnGiris_Click(object sender, EventArgs e)
        {
            // 1. Boş Alan Kontrolü
            if (string.IsNullOrEmpty(txtKullaniciAdi.Text) || string.IsNullOrEmpty(txtSifre.Text))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifreyi giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SqlConnection baglanti = bgl.Baglanti();

                // Kullanıcı adı ve şifreyi veritabanında ara
                SqlCommand komut = new SqlCommand("Select * From kullanicilar where kodu=@p1 and sifre=@p2", baglanti);
                komut.Parameters.AddWithValue("@p1", txtKullaniciAdi.Text);
                komut.Parameters.AddWithValue("@p2", txtSifre.Text);

                SqlDataReader dr = komut.ExecuteReader();

                if (dr.Read())
                {
                    // --- GİRİŞ BAŞARILI ---
                    AnaForm frm = new AnaForm();
                    frm.Show();
                    this.Hide();
                }
                else
                {
                    // --- GİRİŞ HATALI --- 
                    // PDF'teki mesajın birebir aynısı:
                    MessageBox.Show("Yanlış kullanıcı adı ve/veya şifre", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Kullanıcı kolayca tekrar yazabilsin diye şifreyi temizleyip odağı oraya verelim
                    txtSifre.Text = "";
                    txtSifre.Focus();
                }

                baglanti.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı hatası: " + ex.Message);
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
        }
    }
}