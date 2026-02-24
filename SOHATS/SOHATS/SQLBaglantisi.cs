using System;
using System.Data.SqlClient;

namespace SOHATS
{
    class SQLBaglantisi
    {
        public SqlConnection Baglanti()
        {
            // Veritabanı bağlantı adresi
            // Eğer .\SQLEXPRESS çalışmazsa, buraya kendi sunucu adını yazarsın.
            SqlConnection baglan = new SqlConnection(@"Data Source=Aybars\SQLEXPRESS;Initial Catalog=SaglikOcagiOtomasyonu;Integrated Security=True");

            baglan.Open();
            return baglan;
        }
    }
}