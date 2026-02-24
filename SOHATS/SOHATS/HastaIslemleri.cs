using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing; // Yazdırma çizimi için
using System.Drawing.Printing; // Yazdırma işlemi için
using System.Windows.Forms;

namespace SOHATS
{
    public partial class HastaIslemleri : Form
    {
        // Yazdırma Nesneleri
        PrintDocument pd = new PrintDocument();
        PrintPreviewDialog ppd = new PrintPreviewDialog();

        // Global Değişkenler
        SQLBaglantisi bgl = new SQLBaglantisi();
        string secilenDosyaNo = "";

        public HastaIslemleri()
        {
            InitializeComponent();
            // Yazdırma olayını bağla
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
        }

        private void HastaIslemleri_Load(object sender, EventArgs e)
        {
            PoliklinikGetir();
            DoktorGetir();
            IslemleriGetir();

            // Varsayılan ayarlar
            txtSiraNo.Text = "1";
            dateSevkTarihi.Value = DateTime.Now;

            // Eğer numMiktar numericUpDown ise varsayılan değer 1 olsun
            numMiktar.Value = 1;

            txtDosyaNo.Focus();
        }

        // --- BUTON: BUL (HASTA BİLGİLERİNİ GETİR) ---
        private void btnBul_Click(object sender, EventArgs e)
        {
            // --- DURUM 1: Kutu Boşsa (Arama Formunu Aç) ---
            if (txtDosyaNo.Text.Trim() == "")
            {
                DosyaBul frm = new DosyaBul();
                // Arama formu açılır ve kapanması beklenir
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // Formdan gelen seçili numarayı kutuya yaz
                    txtDosyaNo.Text = frm.secilenDosyaNo;

                    // Kutu artık dolu, bilgileri getirmek için butona tekrar kodla tıkla
                    btnBul_Click(sender, e);
                }
                return; // Buradan çık, aşağıdaki kodları çalıştırma
            }

            // --- DURUM 2: Kutu Doluysa (Bilgileri Getir) ---
            secilenDosyaNo = txtDosyaNo.Text;

            try
            {
                SqlConnection baglanti = bgl.Baglanti();

                // 1. Hasta Kimlik Bilgilerini Çek
                SqlCommand komut = new SqlCommand("Select * From hastalar where dosyano=@p1", baglanti);
                komut.Parameters.AddWithValue("@p1", secilenDosyaNo);
                SqlDataReader dr = komut.ExecuteReader();

                if (dr.Read())
                {
                    // Bilgileri doldur
                    txtHastaAd.Text = dr["ad"].ToString();
                    txtHastaSoyad.Text = dr["soyad"].ToString();
                    txtKurumAdi.Text = dr["kurumadi"].ToString(); // Eğer veritabanında 'kurumadi' yoksa hata verebilir, kontrol et.

                    // Önceki işlem tarihlerini doldur
                    OncekiIslemleriListele();

                    // Bugünün işlemlerini Grid'e getir
                    GridDoldur(DateTime.Now.ToString("yyyy-MM-dd"));

                    // Taburcu Kontrolü
                    TaburcuKontrol();
                }
                else
                {
                    MessageBox.Show("Bu numarada kayıtlı hasta bulunamadı.", "Uyarı");
                    // Bulunamadıysa temizle
                    Temizle();
                }
                baglanti.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        // --- TABURCU KONTROLÜ ---
        void TaburcuKontrol()
        {
            try
            {
                SqlConnection baglanti = bgl.Baglanti();
                // Hastanın bugünkü kaydına bak, taburcu olmuş mu?
                SqlCommand komut = new SqlCommand("Select top 1 taburcu From sevk where dosyano=@p1 and tarih=@p2 order by sevkid desc", baglanti);
                komut.Parameters.AddWithValue("@p1", secilenDosyaNo);
                komut.Parameters.AddWithValue("@p2", dateSevkTarihi.Value.ToString("yyyy-MM-dd"));

                object sonuc = komut.ExecuteScalar();
                baglanti.Close();

                if (sonuc != null && sonuc != DBNull.Value)
                {
                    bool taburcuMu = Convert.ToBoolean(sonuc);
                    if (taburcuMu)
                    {
                        MessageBox.Show("Bu hasta bugün taburcu edilmiştir! Yeni işlem yapılamaz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnEkle.Enabled = false;
                        btnTaburcu.Enabled = false;
                        lblToplamTutar.ForeColor = Color.Red;
                        lblToplamTutar.Text = "TABURCU";
                    }
                    else
                    {
                        btnEkle.Enabled = true;
                        btnTaburcu.Enabled = true;
                    }
                }
            }
            catch { }
        }

        // --- GRİD DOLDURMA (SEÇİLEN TARİHE GÖRE) ---
        void GridDoldur(string tarih)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlConnection baglanti = bgl.Baglanti();

                // 'sevkid' alanını silme işlemi için çekiyoruz ama gizleyeceğiz.
                SqlDataAdapter da = new SqlDataAdapter("Select sevkid, poliklinik as POLİKLİNİK, sirano as [SIRA NO], saat as SAAT, yapilanislem as [YAPILAN İŞLEM], drkod as [DR. KODU], miktar as MİKTAR, birimfiyat as [BİRİM FİYATI] From sevk where dosyano='" + secilenDosyaNo + "' and tarih='" + tarih + "'", baglanti);

                da.Fill(dt);
                gridIslemler.DataSource = dt;
                baglanti.Close();

                // sevkid sütununu gizle
                if (gridIslemler.Columns.Contains("sevkid"))
                {
                    gridIslemler.Columns["sevkid"].Visible = false;
                }

                ToplamTutarHesapla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Grid hatası: " + ex.Message);
            }
        }

        // --- TOPLAM TUTAR HESAPLAMA ---
        void ToplamTutarHesapla()
        {
            decimal toplam = 0;
            for (int i = 0; i < gridIslemler.Rows.Count; i++)
            {
                if (gridIslemler.Rows[i].Cells["MİKTAR"].Value != null && gridIslemler.Rows[i].Cells["BİRİM FİYATI"].Value != null)
                {
                    decimal miktar = Convert.ToDecimal(gridIslemler.Rows[i].Cells["MİKTAR"].Value);
                    decimal fiyat = Convert.ToDecimal(gridIslemler.Rows[i].Cells["BİRİM FİYATI"].Value);
                    toplam += (miktar * fiyat);
                }
            }
            lblToplamTutar.Text = toplam.ToString("C2");
        }

        // --- BUTON: EKLE (DÜZELTİLMİŞ HALİ) ---
        private void btnEkle_Click(object sender, EventArgs e)
        {
            // 1. GÜVENLİK KONTROLLERİ
            if (secilenDosyaNo == "")
            {
                MessageBox.Show("Lütfen önce bir hasta bulup seçiniz.", "Uyarı");
                return;
            }

            // NumericUpDown kullandığın için 'numMiktar.Text' kontrolüne gerek yok
            if (cmbPoliklinik.Text == "" || cmbYapilanIslem.Text == "" || cmbDoktor.Text == "")
            {
                MessageBox.Show("Lütfen Poliklinik, İşlem ve Doktor alanlarını doldurunuz.", "Eksik Bilgi");
                return;
            }

            try
            {
                // 2. HESAPLAMA
                decimal birimFiyat = 0;
                decimal.TryParse(txtBirimFiyat.Text, out birimFiyat);

                // *** ÖNEMLİ: NumericUpDown'dan değeri böyle alıyoruz ***
                int miktar = (int)numMiktar.Value;

                decimal satirToplami = birimFiyat * miktar;

                // 3. VERİTABANI İŞLEMİ
                SqlConnection baglanti = bgl.Baglanti();

                string sql = "insert into sevk (dosyano, poliklinik, sirano, saat, yapilanislem, drkod, miktar, birimfiyat, toplamtutar, tarih, taburcu) " +
                             "values (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, 0)";

                SqlCommand komut = new SqlCommand(sql, baglanti);

                komut.Parameters.AddWithValue("@p1", secilenDosyaNo);
                komut.Parameters.AddWithValue("@p2", cmbPoliklinik.Text);
                komut.Parameters.AddWithValue("@p3", txtSiraNo.Text);
                komut.Parameters.AddWithValue("@p4", DateTime.Now.ToShortTimeString());
                komut.Parameters.AddWithValue("@p5", cmbYapilanIslem.Text);

                // Doktor Kodu
                komut.Parameters.AddWithValue("@p6", cmbDoktor.SelectedValue != null ? cmbDoktor.SelectedValue.ToString() : "0");

                komut.Parameters.AddWithValue("@p7", miktar);
                komut.Parameters.AddWithValue("@p8", birimFiyat);
                komut.Parameters.AddWithValue("@p9", satirToplami);
                komut.Parameters.AddWithValue("@p10", dateSevkTarihi.Value.ToString("yyyy-MM-dd"));

                komut.ExecuteNonQuery();
                baglanti.Close();

                // 4. EKRANI GÜNCELLE
                GridDoldur(dateSevkTarihi.Value.ToString("yyyy-MM-dd"));

                // Sıra numarasını 1 arttır
                int sira = int.Parse(txtSiraNo.Text);
                txtSiraNo.Text = (sira + 1).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ekleme hatası: " + ex.Message, "Hata");
            }
        }

        // --- BUTON: SEÇ - SİL ---
        private void btnSecSil_Click(object sender, EventArgs e)
        {
            if (gridIslemler.CurrentRow == null)
            {
                MessageBox.Show("Lütfen silinecek işlemi seçiniz.");
                return;
            }

            // Gizli sevkid sütunundan ID'yi al
            string id = gridIslemler.CurrentRow.Cells["sevkid"].Value.ToString();
            string islemAdi = gridIslemler.CurrentRow.Cells["YAPILAN İŞLEM"].Value.ToString();

            DialogResult cevap = MessageBox.Show("'" + islemAdi + "' işlemini silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (cevap == DialogResult.Yes)
            {
                try
                {
                    SqlConnection baglanti = bgl.Baglanti();
                    SqlCommand komut = new SqlCommand("Delete From sevk where sevkid=@p1", baglanti);
                    komut.Parameters.AddWithValue("@p1", id);
                    komut.ExecuteNonQuery();
                    baglanti.Close();

                    MessageBox.Show("İşlem silindi.");
                    GridDoldur(dateSevkTarihi.Value.ToString("yyyy-MM-dd"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Silme hatası: " + ex.Message);
                }
            }
        }

        // --- BUTON: TABURCU ---
        private void btnTaburcu_Click(object sender, EventArgs e)
        {
            if (secilenDosyaNo == "") return;

            DialogResult cevap = MessageBox.Show("Hasta taburcu edilecek. Onaylıyor musunuz?", "Taburcu", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (cevap == DialogResult.Yes)
            {
                try
                {
                    SqlConnection baglanti = bgl.Baglanti();
                    SqlCommand komut = new SqlCommand("update sevk set taburcu=1 where dosyano=@p1 and tarih=@p2", baglanti);
                    komut.Parameters.AddWithValue("@p1", secilenDosyaNo);
                    komut.Parameters.AddWithValue("@p2", dateSevkTarihi.Value.ToString("yyyy-MM-dd"));
                    komut.ExecuteNonQuery();
                    baglanti.Close();

                    MessageBox.Show("Hasta taburcu edildi.");
                    btnEkle.Enabled = false;
                    btnTaburcu.Enabled = false;
                    lblToplamTutar.ForeColor = Color.Red;
                    lblToplamTutar.Text = "TABURCU EDİLDİ";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        // --- BUTON: YENİ (Hasta Kayıt Formu Aç) ---
        private void btnYeni_Click(object sender, EventArgs e)
        {
            HastaBilgileri frm = new HastaBilgileri();
            frm.ShowDialog(); // Formu aç
            Temizle(); // Form kapanınca ekranı temizle
        }

        // --- BUTON: ÖNCEKİ İŞLEMLER GİT ---
        private void btnGit_Click(object sender, EventArgs e)
        {
            if (cmbOncekiIslemler.Text != "")
            {
                DateTime tarih = Convert.ToDateTime(cmbOncekiIslemler.Text);
                GridDoldur(tarih.ToString("yyyy-MM-dd"));
            }
        }

        // --- BUTON: BASKI ÖNİZLEME ---
        private void btnOnizleme_Click(object sender, EventArgs e)
        {
            ppd.Document = pd;
            ppd.ShowDialog();
        }

        // --- BUTON: YAZDIR ---
        private void btnYazdir_Click(object sender, EventArgs e)
        {
            PrintDialog pDialog = new PrintDialog();
            pDialog.Document = pd;
            if (pDialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        // --- YAZDIRMA ÇİZİM KODU ---
        private void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font baslikFont = new Font("Arial", 20, FontStyle.Bold);
            Font altBaslikFont = new Font("Arial", 12, FontStyle.Bold);
            Font icerikFont = new Font("Arial", 10);
            SolidBrush firca = new SolidBrush(Color.Black);
            Pen kalem = new Pen(Color.Black);

            e.Graphics.DrawString("SAĞLIK OCAĞI HASTA TAKİP SİSTEMİ", baslikFont, firca, 150, 50);
            e.Graphics.DrawString("Hasta İşlem Dökümü", altBaslikFont, firca, 350, 90);

            e.Graphics.DrawString("Dosya No: " + txtDosyaNo.Text, icerikFont, firca, 50, 150);
            e.Graphics.DrawString("Adı Soyadı: " + txtHastaAd.Text + " " + txtHastaSoyad.Text, icerikFont, firca, 50, 175);
            e.Graphics.DrawString("Kurum: " + txtKurumAdi.Text, icerikFont, firca, 50, 200);
            e.Graphics.DrawString("Tarih: " + DateTime.Now.ToShortDateString(), icerikFont, firca, 600, 150);

            e.Graphics.DrawLine(kalem, 50, 230, 750, 230);

            int y = 250;
            e.Graphics.DrawString("POLİKLİNİK", altBaslikFont, firca, 50, y);
            e.Graphics.DrawString("İŞLEM", altBaslikFont, firca, 200, y);
            e.Graphics.DrawString("DR. KODU", altBaslikFont, firca, 400, y);
            e.Graphics.DrawString("MİKTAR", altBaslikFont, firca, 500, y);
            e.Graphics.DrawString("FİYAT", altBaslikFont, firca, 600, y);

            y += 30;
            e.Graphics.DrawLine(kalem, 50, y, 750, y);
            y += 10;

            for (int i = 0; i < gridIslemler.Rows.Count; i++)
            {
                if (gridIslemler.Rows[i].Cells["POLİKLİNİK"].Value == null) continue;

                e.Graphics.DrawString(gridIslemler.Rows[i].Cells["POLİKLİNİK"].Value.ToString(), icerikFont, firca, 50, y);
                e.Graphics.DrawString(gridIslemler.Rows[i].Cells["YAPILAN İŞLEM"].Value.ToString(), icerikFont, firca, 200, y);
                e.Graphics.DrawString(gridIslemler.Rows[i].Cells["DR. KODU"].Value.ToString(), icerikFont, firca, 400, y);
                e.Graphics.DrawString(gridIslemler.Rows[i].Cells["MİKTAR"].Value.ToString(), icerikFont, firca, 520, y);
                e.Graphics.DrawString(gridIslemler.Rows[i].Cells["BİRİM FİYATI"].Value.ToString(), icerikFont, firca, 600, y);
                y += 25;
            }

            e.Graphics.DrawLine(kalem, 50, y + 10, 750, y + 10);
            e.Graphics.DrawString("TOPLAM TUTAR: " + lblToplamTutar.Text, baslikFont, firca, 450, y + 30);
        }

        // --- YARDIMCI METOTLAR ---
        void PoliklinikGetir()
        {
            try
            {
                cmbPoliklinik.Items.Clear();
                SqlConnection baglanti = bgl.Baglanti();
                SqlCommand komut = new SqlCommand("Select poliklinikadi From poliklinik where durum='Geçerli'", baglanti);
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read()) { cmbPoliklinik.Items.Add(dr[0].ToString()); }
                baglanti.Close();
            }
            catch { }
        }

        void DoktorGetir()
        {
            try
            {
                cmbDoktor.DataSource = null;
                SqlConnection baglanti = bgl.Baglanti();
                SqlDataAdapter da = new SqlDataAdapter("Select kodu, (ad + ' ' + soyad) as TamAd From kullanicilar where unvan='Doktor'", baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbDoktor.ValueMember = "kodu";
                cmbDoktor.DisplayMember = "TamAd";
                cmbDoktor.DataSource = dt;
                baglanti.Close();
            }
            catch { }
        }

        void IslemleriGetir()
        {
            try
            {
                cmbYapilanIslem.Items.Clear();
                SqlConnection baglanti = bgl.Baglanti();
                SqlCommand komut = new SqlCommand("Select islemadi From islemler", baglanti);
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read()) { cmbYapilanIslem.Items.Add(dr[0].ToString()); }
                baglanti.Close();
            }
            catch { }
        }

        void OncekiIslemleriListele()
        {
            cmbOncekiIslemler.Items.Clear();
            try
            {
                SqlConnection baglanti = bgl.Baglanti();
                SqlCommand komut = new SqlCommand("Select distinct tarih From sevk where dosyano=@p1 order by tarih desc", baglanti);
                komut.Parameters.AddWithValue("@p1", secilenDosyaNo);
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    cmbOncekiIslemler.Items.Add(Convert.ToDateTime(dr[0]).ToShortDateString());
                }
                baglanti.Close();
            }
            catch { }
        }

        void Temizle()
        {
            secilenDosyaNo = "";
            txtDosyaNo.Text = "";
            txtHastaAd.Text = "";
            txtHastaSoyad.Text = "";
            txtKurumAdi.Text = "";
            gridIslemler.DataSource = null;
            lblToplamTutar.Text = "0 TL";
            btnEkle.Enabled = true;
            btnTaburcu.Enabled = true;
            lblToplamTutar.ForeColor = Color.Black;
            txtDosyaNo.Focus();
        }

        // --- OTOMATİK FİYAT GETİRME ---
        private void cmbYapilanIslem_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SqlConnection baglanti = bgl.Baglanti();
                SqlCommand komut = new SqlCommand("Select birimfiyat From islemler where islemadi=@p1", baglanti);
                komut.Parameters.AddWithValue("@p1", cmbYapilanIslem.Text);
                SqlDataReader dr = komut.ExecuteReader();
                if (dr.Read()) { txtBirimFiyat.Text = dr[0].ToString(); }
                baglanti.Close();
            }
            catch { }
        }

        // --- BOŞ OLUŞAN EVENTLER (SİLME!) ---
        private void gridIslemler_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void btnSecSil_Click_1(object sender, EventArgs e) { }
    }
}