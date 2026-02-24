using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOHATS
{
    public partial class AnaForm : Form
    {
        public AnaForm()
        {
            InitializeComponent();
        }

        private void AnaForm_Load(object sender, EventArgs e)
        {

        }

        private void poliklinikTanıtmaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PoliklinikTanitma frm = new PoliklinikTanitma();
            frm.MdiParent = this; // Bu formu AnaFormun içinde açar
            frm.Show();
        }

        private void hastaKabulToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void kullanıcıTanıtmaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KullaniciTanitma frm = new KullaniciTanitma();
            frm.MdiParent = this; // Ana pencerenin içinde açılsın
            frm.Show();
        }

        private void hastaİşlemleriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Hasta İşlemleri Formunu Açma Kodu
            HastaIslemleri frm = new HastaIslemleri();
            frm.MdiParent = this; // Ana pencerenin içinde açılsın
            frm.Show();
        }
    }
}
