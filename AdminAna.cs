using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;

namespace RafArasi2
{
    public partial class AdminAna : Form
    {
        private Timer idleTimer;
        private bool isWarningShown = false;
        private bool isSessionExpired = false;

        public AdminAna()
        {
            InitializeComponent();
            InitializeIdleTimer();
        }

        private void InitializeIdleTimer()
        {
            idleTimer = new Timer();
            idleTimer.Interval = 1170000;//1170000;//19:30 dakika 1000

            idleTimer.Tick += IdleTimer_Tick;
            idleTimer.Start();

            Application.Idle += Application_Idle;
        }

        private void ResetIdleTimer()
        {
            idleTimer.Stop();
            idleTimer.Start();
        }

        private void IdleTimer_Tick(object sender, EventArgs e)
        {
            if (!isWarningShown && !isSessionExpired)
            {
                isWarningShown = true;
                DialogResult result = MessageBox.Show("Oturum sürenizin bitmesine 30 saniye kaldı. Lütfen tekrar giriş yapınız!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (result == DialogResult.OK)
                {
                    idleTimer.Stop();
                    idleTimer.Interval = 30000;//30 saniye
                    idleTimer.Start();
                }
            }
            else if (isWarningShown && !isSessionExpired)
            {
                isSessionExpired = true;
                MessageBox.Show("Oturum süreniz doldu. Lütfen tekrar giriş yapınız!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            ResetIdleTimer();
        }
        //tarih ve saat göstermek için
        private void timer2_Tick(object sender, EventArgs e)
        {
            label7.Text = DateTime.Now.ToLongDateString();
            label8.Text = DateTime.Now.ToLongTimeString();
        }


        public void VerileriGetir()
        {
            dataGridView1.Rows.Clear();

            using (SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
            {
                baglanti.Open();

                string sql = "SELECT * FROM Kasiyer";
                SqlCommand command = new SqlCommand(sql, baglanti);

                // Verileri almak için SqlDataReader kullanın
                SqlDataReader reader = command.ExecuteReader();

                // Verileri oku ve işle
                while (reader.Read())
                {
                    // Kasiyer verilerini al
                    string kasiyerId = reader["KasiyerId"].ToString();
                    string kasiyer = reader["Kasiyer"].ToString();
                    string sifre = reader["Sifre"].ToString();
                    string ad = reader["Ad"].ToString();
                    string soyad = reader["Soyad"].ToString();
                    string adres = reader["Adres"].ToString();
                    string mail = reader["Mail"].ToString();
                    string cepNumarasi = reader["CepNumarasi"].ToString();

                    DateTime dtarihi = DateTime.MinValue;
                    if (reader["DTarihi"] != DBNull.Value)
                    {
                        dtarihi = DateTime.Parse(reader["DTarihi"].ToString());
                    }

                    DateTime kayitTarihi = DateTime.MinValue;
                    if (reader["KayitTarihi"] != DBNull.Value)
                    {
                        kayitTarihi = DateTime.Parse(reader["KayitTarihi"].ToString());
                    }

                    string kayitTarihiFormatli = kayitTarihi.ToString("dd/MM/yyyy");
                    string kayitSaatiFormatli = kayitTarihi.ToString("HH:mm");

                    dataGridView1.Rows.Add(kasiyerId, kasiyer, sifre, ad, soyad, adres, mail, cepNumarasi, dtarihi.ToString("yyyy-MM-dd"), kayitTarihiFormatli, kayitSaatiFormatli);
                }

                reader.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            KasiyerEkle KasiyerEkle = new KasiyerEkle(this);
            KasiyerEkle.Show();
        }


        private void AdminAna_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("KasiyerId", "Kasiyer ID");
            dataGridView1.Columns.Add("Kasiyer", "Kasiyer");
            dataGridView1.Columns.Add("Sifre", "Şifre");
            dataGridView1.Columns.Add("Ad", "Ad");
            dataGridView1.Columns.Add("Soyad", "Soyad");
            dataGridView1.Columns.Add("Adres", "Adres");
            dataGridView1.Columns.Add("Mail", "Mail");
            dataGridView1.Columns.Add("CepNumarasi", "Cep Numarası");
            dataGridView1.Columns.Add("DTarihi", "Doğum Tarihi");
            dataGridView1.Columns.Add("KayitTarihi", "Kayıt Tarihi");
            dataGridView1.Columns.Add("KayitSaati", "Kayıt Saati");
            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells; //kolun genişliğini hücrelerdekiler kadar olmasını sağlar.

            using (SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
            {
                baglanti.Open();

                string sql = "SELECT KasiyerId, Kasiyer, Sifre, Ad, Soyad, Adres, Mail, CepNumarasi, DTarihi, KayitTarihi FROM Kasiyer";
                SqlCommand command = new SqlCommand(sql, baglanti);

                // Verileri almak için SqlDataReader kullanın
                SqlDataReader reader = command.ExecuteReader();

                // Verileri oku ve işle
                while (reader.Read())
                {
                    // Kasiyer verilerini al
                    string kasiyerId = reader["KasiyerId"].ToString();
                    string kasiyer = reader["Kasiyer"].ToString();
                    string sifre = reader["Sifre"].ToString();
                    string ad = reader["Ad"].ToString();
                    string soyad = reader["Soyad"].ToString();
                    string adres = reader["Adres"].ToString();
                    string mail = reader["Mail"].ToString();
                    string cepNumarasi = reader["CepNumarasi"].ToString();

                    DateTime dtarihi = DateTime.MinValue;
                    if (reader["DTarihi"] != DBNull.Value)
                    {
                        dtarihi = DateTime.Parse(reader["DTarihi"].ToString());
                    }

                    DateTime kayitTarihi = DateTime.MinValue;
                    if (reader["KayitTarihi"] != DBNull.Value)
                    {
                        kayitTarihi = DateTime.Parse(reader["KayitTarihi"].ToString());
                    }

                    string kayitTarihiFormatli = kayitTarihi.ToString("dd/MM/yyyy");
                    string kayitSaatiFormatli = kayitTarihi.ToString("HH:mm");

                    dataGridView1.Rows.Add(kasiyerId, kasiyer, sifre, ad, soyad, adres, mail, cepNumarasi, dtarihi.ToString("yyyy-MM-dd"), kayitTarihiFormatli, kayitSaatiFormatli);

                    // Alınan verileri kullanarak işlemler yapabilirsiniz
                    // Örneğin, bir DataGridView'e bindirebilirsiniz
                }

                reader.Close();
            }
        }



        private void label1_Click(object sender, EventArgs e)
        {

        }
        // Kasiyer verilerini değiştirme
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            using (SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
            {
                baglanti.Open();
                string arananKasiyer = textBox1.Text;

                // DataGridView'i temizle
                dataGridView1.Rows.Clear();

                // Arama sorgusunu oluştur
                string sql = "SELECT * FROM Kasiyer WHERE Kasiyer LIKE @arananKasiyer";
                SqlCommand command = new SqlCommand(sql, baglanti);
                command.Parameters.AddWithValue("@arananKasiyer", "%" + arananKasiyer + "%");

                // Verileri almak için SqlDataReader kullan
                SqlDataReader reader = command.ExecuteReader();

                // Verileri oku ve işle
                while (reader.Read())
                {
                    string kasiyerId = reader["KasiyerId"].ToString();
                    string kasiyer = reader["Kasiyer"].ToString();
                    string sifre = reader["Sifre"].ToString();
                    string ad = reader["Ad"].ToString();
                    string soyad = reader["Soyad"].ToString();
                    string adres = reader["Adres"].ToString();
                    string mail = reader["Mail"].ToString();
                    string cepNumarasi = reader["CepNumarasi"].ToString();

                    DateTime dtarihi = DateTime.MinValue;
                    if (reader["DTarihi"] != DBNull.Value)
                    {
                        dtarihi = DateTime.Parse(reader["DTarihi"].ToString());
                    }

                    DateTime kayitTarihi = DateTime.MinValue;
                    if (reader["KayitTarihi"] != DBNull.Value)
                    {
                        kayitTarihi = DateTime.Parse(reader["KayitTarihi"].ToString());
                    }

                    string kayitTarihiFormatli = kayitTarihi.ToString("dd/MM/yyyy");
                    string kayitSaatiFormatli = kayitTarihi.ToString("HH:mm");

                    dataGridView1.Rows.Add(kasiyerId, kasiyer, sifre, ad, soyad, adres, mail, cepNumarasi, dtarihi.ToString("yyyy-MM-dd"), kayitTarihiFormatli, kayitSaatiFormatli);
                }

                reader.Close();
            }
        }
       private void UpdateKasiyerData()
{
    // Seçili satırı kontrol et
    if (dataGridView1.SelectedRows.Count > 0)
    {
        // Seçili satırın indeksini al
        int rowIndex = dataGridView1.SelectedRows[0].Index;

        // Seçili satırın KasiyerId değerini al
        string kasiyerId = dataGridView1.Rows[rowIndex].Cells["KasiyerId"].Value.ToString();

        // Güncellenecek verileri al
        string kasiyer = dataGridView1.Rows[rowIndex].Cells["Kasiyer"].Value.ToString();
        string sifre = dataGridView1.Rows[rowIndex].Cells["Sifre"].Value.ToString();
        string ad = dataGridView1.Rows[rowIndex].Cells["Ad"].Value.ToString();
        string soyad = dataGridView1.Rows[rowIndex].Cells["Soyad"].Value.ToString();
        string adres = dataGridView1.Rows[rowIndex].Cells["Adres"].Value.ToString();
        string mail = dataGridView1.Rows[rowIndex].Cells["Mail"].Value.ToString();
        string cepNumarasi = dataGridView1.Rows[rowIndex].Cells["CepNumarasi"].Value.ToString();
        string dtarihi = dataGridView1.Rows[rowIndex].Cells["DTarihi"].Value.ToString();

        // Veritabanında güncelleme işlemini gerçekleştir
        using (SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
        {
            baglanti.Open();

            // Kullanıcı adının benzersiz olup olmadığını kontrol et
            string checkDuplicateUsernameQuery = "SELECT COUNT(*) FROM Kasiyer WHERE KasiyerId <> @kasiyerId AND Kasiyer = @kasiyer";
            SqlCommand checkDuplicateUsernameCommand = new SqlCommand(checkDuplicateUsernameQuery, baglanti);
            checkDuplicateUsernameCommand.Parameters.AddWithValue("@kasiyerId", kasiyerId);
            checkDuplicateUsernameCommand.Parameters.AddWithValue("@kasiyer", kasiyer);
            int duplicateUsernameCount = (int)checkDuplicateUsernameCommand.ExecuteScalar();

            if (duplicateUsernameCount > 0)
            {
                // Kullanıcı adı çakışması var, hata mesajı göster
                MessageBox.Show("Bu kullanıcı adı zaten kullanılıyor. Lütfen farklı bir kullanıcı adı seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Kullanıcı adında çakışma yok, güncelleme işlemini devam ettir
                string updateQuery = "UPDATE Kasiyer SET Kasiyer = @kasiyer, Sifre = @sifre, Ad = @ad, Soyad = @soyad, Adres = @adres, Mail = @mail, CepNumarasi = @cepNumarasi, DTarihi = @dtarihi WHERE KasiyerId = @kasiyerId";
                SqlCommand updateCommand = new SqlCommand(updateQuery, baglanti);
                updateCommand.Parameters.AddWithValue("@kasiyer", kasiyer);
                updateCommand.Parameters.AddWithValue("@sifre", sifre);
                updateCommand.Parameters.AddWithValue("@ad", ad);
                updateCommand.Parameters.AddWithValue("@soyad", soyad);
                updateCommand.Parameters.AddWithValue("@adres", adres);
                updateCommand.Parameters.AddWithValue("@mail", mail);
                updateCommand.Parameters.AddWithValue("@cepNumarasi", cepNumarasi);
                updateCommand.Parameters.AddWithValue("@dtarihi", dtarihi);
                updateCommand.Parameters.AddWithValue("@kasiyerId", kasiyerId);
                updateCommand.ExecuteNonQuery();

                // Veriler güncellendi, kullanıcıya bilgi mesajı göster
                MessageBox.Show("Veriler güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Verileri yenile
              
            }
        }
            }
            else
    {
        MessageBox.Show("Güncellenecek bir satır seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}

        private void button3_Click(object sender, EventArgs e)
        {
            UpdateKasiyerData();
        }

        // Kasiyer verilerini silme
        private void DeleteKasiyerData()
        {
            // Seçili satırı kontrol et
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Silme işlemi için onay al
                DialogResult result = MessageBox.Show("Silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Seçili satırın indeksini al
                    int rowIndex = dataGridView1.SelectedRows[0].Index;

                    // Seçili satırın KasiyerId değerini al
                    string kasiyerId = dataGridView1.Rows[rowIndex].Cells["KasiyerId"].Value.ToString();

                    // Veritabanında silme işlemini gerçekleştir
                    using (SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
                    {
                        baglanti.Open();

                        string sql = "DELETE FROM Kasiyer WHERE KasiyerId = @kasiyerId";
                        SqlCommand command = new SqlCommand(sql, baglanti);
                        command.Parameters.AddWithValue("@kasiyerId", kasiyerId);

                        command.ExecuteNonQuery();

                        // Veritabanı güncellendikten sonra DataGridView'den satırı kaldır
                        dataGridView1.Rows.RemoveAt(rowIndex);
                    }
                }
            }
            else
            {
                MessageBox.Show("Silinecek veriyi seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            DeleteKasiyerData();
        }
        private void AdminAna_FormClosing(object sender, FormClosingEventArgs e)
        {
            KasiyerEkle kasiyerEkleForm = Application.OpenForms["KasiyerEkle"] as KasiyerEkle;
            if (kasiyerEkleForm != null && !kasiyerEkleForm.IsDisposed)
            {
                kasiyerEkleForm.Close();
            }
            AdminMüsteri adminMusteriForm = Application.OpenForms["AdminMüsteri"] as AdminMüsteri;
            if (adminMusteriForm != null && !adminMusteriForm.IsDisposed)
            {
                adminMusteriForm.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
           AdminMüsteri AdminMüsteri = new AdminMüsteri();
            AdminMüsteri.Show();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            AdminAna adminAnaForm = Application.OpenForms.OfType<AdminAna>().FirstOrDefault();
            if (adminAnaForm != null && !adminAnaForm.IsDisposed)
            {
                adminAnaForm.Close();
            }

            Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (form1 == null || form1.IsDisposed)
            {
                form1 = new Form1();
                form1.Show();
            }
        }
    }
}
