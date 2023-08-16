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
using RafArasi2.Models;

namespace RafArasi2
{
    public partial class Kitap : Form
    {
        public Kitap()
        {
            InitializeComponent();
        }

        private void Kitap_Load(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string queryMusteriKitap = "SELECT Tc, Ad, Soyad, Adres, IlkTarih, SonTarih, CepNumarasi, KitapAd, KitapID FROM musterikitap";
                SqlCommand commandMusteriKitap = new SqlCommand(queryMusteriKitap, connection);

                // Kitap tablosunu al
                string queryKitap = "SELECT KitapID, KitapAd, Yazar, SayfaSayisi, Kira, KiraSayi, Kategori FROM Kitap";
                SqlCommand commandKitap = new SqlCommand(queryKitap, connection);

                // Verileri oku
                SqlDataAdapter adapterMusteriKitap = new SqlDataAdapter(commandMusteriKitap);
                SqlDataAdapter adapterKitap = new SqlDataAdapter(commandKitap);
                DataTable dataTableMusteriKitap = new DataTable();
                DataTable dataTableKitap = new DataTable();
                adapterMusteriKitap.Fill(dataTableMusteriKitap);
                adapterKitap.Fill(dataTableKitap);

                dataGridView1.DataSource = dataTableMusteriKitap;

                dataGridView2.DataSource = dataTableKitap;

                // Geçmiş veya son gün olan satırları kırmızı yap
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    DateTime sonTarih = Convert.ToDateTime(row.Cells["SonTarih"].Value);
                    if (sonTarih <= DateTime.Now)
                    {
                        row.DefaultCellStyle.BackColor = Color.Red;
                    }
                }

                dataGridView2.Columns["Kira"].ReadOnly = true;
            }
            comboBox1.Items.Add("Roman");
            comboBox1.Items.Add("Bilim Kurgu");
            comboBox1.Items.Add("Polisiye");
            comboBox1.Items.Add("Fantastik");
            comboBox1.Items.Add("Korku");
            comboBox1.Items.Add("Biyografi");
            comboBox1.Items.Add("Tarih");
            comboBox1.Items.Add("Klasik");
            comboBox1.Items.Add("Mizah");
            comboBox1.Items.Add("Çocuk Kitapları");
            comboBox1.Items.Add("Drama");
            comboBox1.Items.Add("Aksiyon");
            comboBox1.Items.Add("Felsefe");
            comboBox1.Items.Add("Dini");
            comboBox1.Items.Add("Kurgu Dışı");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string tc = textBox1.Text;
                string ad = textBox2.Text;
                string soyad = textBox3.Text;
                string adres = textBox4.Text;
                DateTime ilkTarih = dateTimePicker1.Value;
                DateTime sonTarih = dateTimePicker2.Value;
                string cepNumarasi = textBox5.Text;
                string kitapAd = textBox6.Text;
                string kitapID = textBox7.Text;

                string queryCheckKitap = "SELECT Kira, KitapAd FROM Kitap WHERE KitapID = @KitapID";
                SqlCommand commandCheckKitap = new SqlCommand(queryCheckKitap, connection);
                commandCheckKitap.Parameters.AddWithValue("@KitapID", kitapID);
                SqlDataReader reader = commandCheckKitap.ExecuteReader();

                if (reader.Read())
                {
                    bool Kira = (bool)reader["Kira"];
                    string kitapAdVeritabanı = reader["KitapAd"].ToString();

                    if (Kira)
                    {
                        MessageBox.Show("Bu kitap zaten kiralandı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        reader.Close();
                        return;
                    }
                    else if (kitapAd != kitapAdVeritabanı)
                    {
                        MessageBox.Show("Seçtiğiniz kitabı kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        reader.Close();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Böyle bir kitap bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    reader.Close();
                    return;
                }

                reader.Close();

                string queryInsertMusteriKitap = "INSERT INTO musterikitap (Tc, Ad, Soyad, Adres, IlkTarih, SonTarih, CepNumarasi, KitapAd, KitapID) VALUES (@Tc, @Ad, @Soyad, @Adres, @IlkTarih, @SonTarih, @CepNumarasi, @KitapAd, @KitapID)";
                SqlCommand commandInsertMusteriKitap = new SqlCommand(queryInsertMusteriKitap, connection);
                commandInsertMusteriKitap.Parameters.AddWithValue("@Tc", tc);
                commandInsertMusteriKitap.Parameters.AddWithValue("@Ad", ad);
                commandInsertMusteriKitap.Parameters.AddWithValue("@Soyad", soyad);
                commandInsertMusteriKitap.Parameters.AddWithValue("@Adres", adres);
                commandInsertMusteriKitap.Parameters.AddWithValue("@IlkTarih", ilkTarih);
                commandInsertMusteriKitap.Parameters.AddWithValue("@SonTarih", sonTarih);
                commandInsertMusteriKitap.Parameters.AddWithValue("@CepNumarasi", cepNumarasi);
                commandInsertMusteriKitap.Parameters.AddWithValue("@KitapAd", kitapAd);
                commandInsertMusteriKitap.Parameters.AddWithValue("@KitapID", kitapID);
                commandInsertMusteriKitap.ExecuteNonQuery();

                string queryUpdateKitap = "UPDATE Kitap SET Kira = 1 WHERE KitapID = @KitapID";
                SqlCommand commandUpdateKitap = new SqlCommand(queryUpdateKitap, connection);
                commandUpdateKitap.Parameters.AddWithValue("@KitapID", kitapID);
                commandUpdateKitap.ExecuteNonQuery();

                DataTable dataTableMusteriKitap = new DataTable();
                DataTable dataTableKitap = new DataTable();
                SqlDataAdapter adapterMusteriKitap = new SqlDataAdapter("SELECT Tc, Ad, Soyad, Adres, IlkTarih, SonTarih, CepNumarasi, KitapAd, KitapID FROM musterikitap", connection);
                SqlDataAdapter adapterKitap = new SqlDataAdapter("SELECT KitapID, KitapAd, Yazar, SayfaSayisi, Kira, KiraSayi, Kategori FROM Kitap", connection);
                adapterMusteriKitap.Fill(dataTableMusteriKitap);
                adapterKitap.Fill(dataTableKitap);
                dataGridView1.DataSource = dataTableMusteriKitap;
                dataGridView2.DataSource = dataTableKitap;


                MessageBox.Show("Kitap kiraya verildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DateTime sonTarih = Convert.ToDateTime(row.Cells["SonTarih"].Value);
                if (sonTarih <= DateTime.Now)
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                }
            }

            dataGridView2.Columns["Kira"].ReadOnly = true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string kitapAd = textBox8.Text;
                string yazar = textBox9.Text;
                int sayfaSayisi;
                if (!int.TryParse(textBox10.Text, out sayfaSayisi))
                {
                    MessageBox.Show("Sayfa sayısı geçerli bir tamsayı olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string kategori = comboBox1.Text;
                string queryCheckDuplicate = "SELECT COUNT(*) FROM Kitap WHERE KitapAd = @KitapAd";
                SqlCommand commandCheckDuplicate = new SqlCommand(queryCheckDuplicate, connection);
                commandCheckDuplicate.Parameters.AddWithValue("@KitapAd", kitapAd);
                int duplicateCount = (int)commandCheckDuplicate.ExecuteScalar();

                if (duplicateCount > 0)
                {
                    
                    DialogResult result = MessageBox.Show("Bu kitap daha önce kaydedilmiş. Kaydetmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.No)
                    {
                        return; 
                    }
                }


                string queryInsertKitap = "INSERT INTO Kitap (KitapAd, Yazar, SayfaSayisi, Kira, KiraSayi, Kategori) VALUES (@KitapAd, @Yazar, @SayfaSayisi, 0, 0, @Kategori)";
                SqlCommand commandInsertKitap = new SqlCommand(queryInsertKitap, connection);
                commandInsertKitap.Parameters.AddWithValue("@KitapAd", kitapAd);
                commandInsertKitap.Parameters.AddWithValue("@Yazar", yazar);
                commandInsertKitap.Parameters.AddWithValue("@SayfaSayisi", sayfaSayisi);
                commandInsertKitap.Parameters.AddWithValue("@Kategori", kategori);
                commandInsertKitap.ExecuteNonQuery();

                DataTable dataTableKitap = new DataTable();
                SqlDataAdapter adapterKitap = new SqlDataAdapter("SELECT KitapID, KitapAd, Yazar, SayfaSayisi, Kira, KiraSayi, Kategori FROM Kitap", connection);
                adapterKitap.Fill(dataTableKitap);
                dataGridView2.DataSource = dataTableKitap;

                MessageBox.Show("Kitap kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
          
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Seçili satırı silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    int selectedRowIndex = dataGridView1.SelectedRows[0].Index;
                    string tc = dataGridView1.Rows[selectedRowIndex].Cells["Tc"].Value.ToString();
                    string kitapID = dataGridView1.Rows[selectedRowIndex].Cells["KitapID"].Value.ToString();

                    string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True";

                 
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                       
                        string queryDelete = "DELETE FROM musterikitap WHERE Tc = @Tc";
                        SqlCommand commandDelete = new SqlCommand(queryDelete, connection);
                        commandDelete.Parameters.AddWithValue("@Tc", tc);
                        commandDelete.ExecuteNonQuery();

                       
                        string queryUpdateKitap = "UPDATE Kitap SET Kira = 0 WHERE KitapID = @KitapID";
                        SqlCommand commandUpdateKitap = new SqlCommand(queryUpdateKitap, connection);
                        commandUpdateKitap.Parameters.AddWithValue("@KitapID", kitapID);
                        commandUpdateKitap.ExecuteNonQuery();

                       
                        DataTable dataTableMusteriKitap = new DataTable();
                        SqlDataAdapter adapterMusteriKitap = new SqlDataAdapter("SELECT Tc, Ad, Soyad, Adres, IlkTarih, SonTarih, CepNumarasi, KitapAd, KitapID FROM musterikitap", connection);
                        adapterMusteriKitap.Fill(dataTableMusteriKitap);
                        dataGridView1.DataSource = dataTableMusteriKitap;

                        MessageBox.Show("Satır silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                       
                        DataTable dataTableKitap = new DataTable();
                        SqlDataAdapter adapterKitap = new SqlDataAdapter("SELECT KitapID, KitapAd, Yazar, SayfaSayisi, Kira, KiraSayi, Kategori, FROM Kitap", connection);
                        adapterKitap.Fill(dataTableKitap);
                        dataGridView2.DataSource = dataTableKitap;
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            DateTime sonTarih = Convert.ToDateTime(row.Cells["SonTarih"].Value);
                            if (sonTarih <= DateTime.Now)
                            {
                                row.DefaultCellStyle.BackColor = Color.Red;
                            }
                        }

                        dataGridView2.Columns["Kira"].ReadOnly = true;
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Seçili satırı güncellemek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    int selectedRowIndex = dataGridView1.SelectedRows[0].Index;
                    string tc = dataGridView1.Rows[selectedRowIndex].Cells["Tc"].Value.ToString();
                    string ad = dataGridView1.Rows[selectedRowIndex].Cells["Ad"].Value.ToString();
                    string soyad = dataGridView1.Rows[selectedRowIndex].Cells["Soyad"].Value.ToString();
                    string adres = dataGridView1.Rows[selectedRowIndex].Cells["Adres"].Value.ToString();
                    string ilkTarih = dataGridView1.Rows[selectedRowIndex].Cells["IlkTarih"].Value.ToString();
                    string sonTarih = dataGridView1.Rows[selectedRowIndex].Cells["SonTarih"].Value.ToString();
                    string cepNumarasi = dataGridView1.Rows[selectedRowIndex].Cells["CepNumarasi"].Value.ToString();
                    string kitapAd = dataGridView1.Rows[selectedRowIndex].Cells["KitapAd"].Value.ToString();
                    string kitapID = dataGridView1.Rows[selectedRowIndex].Cells["KitapID"].Value.ToString();


                    DateTime ilkTarihValue;
                    DateTime sonTarihValue;

                    if (!DateTime.TryParse(ilkTarih, out ilkTarihValue) || !DateTime.TryParse(sonTarih, out sonTarihValue))
                    {
                        MessageBox.Show("Geçersiz tarih formatı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    ilkTarihValue = ilkTarihValue.Date;
                    sonTarihValue = sonTarihValue.Date;

                    string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string queryUpdate = "UPDATE musterikitap SET Ad = @Ad, Soyad = @Soyad, Adres = @Adres, IlkTarih = @IlkTarih, SonTarih = @SonTarih, CepNumarasi = @CepNumarasi, KitapAd = @KitapAd, KitapID = @KitapID WHERE Tc = @Tc";
                        SqlCommand commandUpdate = new SqlCommand(queryUpdate, connection);
                        commandUpdate.Parameters.AddWithValue("@Ad", ad);
                        commandUpdate.Parameters.AddWithValue("@Soyad", soyad);
                        commandUpdate.Parameters.AddWithValue("@Adres", adres);
                        commandUpdate.Parameters.AddWithValue("@IlkTarih", ilkTarihValue);
                        commandUpdate.Parameters.AddWithValue("@SonTarih", sonTarihValue);
                        commandUpdate.Parameters.AddWithValue("@CepNumarasi", cepNumarasi);
                        commandUpdate.Parameters.AddWithValue("@KitapAd", kitapAd);
                        commandUpdate.Parameters.AddWithValue("@KitapID", kitapID);
                        commandUpdate.Parameters.AddWithValue("@Tc", tc);
                        commandUpdate.ExecuteNonQuery();


                        DataTable dataTableMusteriKitap = new DataTable();
                        SqlDataAdapter adapterMusteriKitap = new SqlDataAdapter("SELECT Tc, Ad, Soyad, Adres, IlkTarih, SonTarih, CepNumarasi, KitapAd, KitapID FROM musterikitap", connection);
                        adapterMusteriKitap.Fill(dataTableMusteriKitap);
                        dataGridView1.DataSource = dataTableMusteriKitap;

                        MessageBox.Show("Satır güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);


                        string queryUpdateKira = "UPDATE kitap SET Kira = CASE WHEN KitapID = @KitapID THEN 1 ELSE 0 END";
                        SqlCommand commandUpdateKira = new SqlCommand(queryUpdateKira, connection);
                        commandUpdateKira.Parameters.AddWithValue("@KitapID", kitapID);
                        commandUpdateKira.ExecuteNonQuery();

                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellemek için bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DateTime sonTarih = Convert.ToDateTime(row.Cells["SonTarih"].Value);
                if (sonTarih <= DateTime.Now)
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                }
            }

            dataGridView2.Columns["Kira"].ReadOnly = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                int selectedRowIndex = dataGridView2.SelectedRows[0].Index;
                string kitapID = dataGridView2.Rows[selectedRowIndex].Cells["KitapID"].Value.ToString();
                bool Kira = Convert.ToBoolean(dataGridView2.Rows[selectedRowIndex].Cells["Kira"].Value);

                DialogResult result = MessageBox.Show("Seçili satırı silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (Kira)
                    {
                        DialogResult rentResult = MessageBox.Show("Bu kitap şu anda kirada. Silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (rentResult == DialogResult.No)
                        {
                            return; 
                        }
                    }

                    string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        
                        string queryDeleteMusteriKitap = "DELETE FROM musterikitap WHERE KitapID = @KitapID";
                        SqlCommand commandDeleteMusteriKitap = new SqlCommand(queryDeleteMusteriKitap, connection);
                        commandDeleteMusteriKitap.Parameters.AddWithValue("@KitapID", kitapID);
                        commandDeleteMusteriKitap.ExecuteNonQuery();

                       
                        string queryDeleteKitap = "DELETE FROM kitap WHERE KitapID = @KitapID";
                        SqlCommand commandDeleteKitap = new SqlCommand(queryDeleteKitap, connection);
                        commandDeleteKitap.Parameters.AddWithValue("@KitapID", kitapID);
                        commandDeleteKitap.ExecuteNonQuery();

                        MessageBox.Show("Satır silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                      
                        DataTable dataTableKitap = new DataTable();
                        SqlDataAdapter adapterKitap = new SqlDataAdapter("SELECT KitapID, KitapAd, Yazar, Kira, KiraSayi, Kategori FROM kitap", connection);
                        adapterKitap.Fill(dataTableKitap);
                        dataGridView2.DataSource = dataTableKitap;
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                int selectedRowIndex = dataGridView2.SelectedRows[0].Index;
                string kitapID = dataGridView2.Rows[selectedRowIndex].Cells["KitapID"].Value.ToString();
                string kitapAd = dataGridView2.Rows[selectedRowIndex].Cells["KitapAd"].Value.ToString();
                string yazar = dataGridView2.Rows[selectedRowIndex].Cells["Yazar"].Value.ToString();
                string sayfasayisi = dataGridView2.Rows[selectedRowIndex].Cells["Sayfasayisi"].Value.ToString();
                bool kira = Convert.ToBoolean(dataGridView2.Rows[selectedRowIndex].Cells["Kira"].Value);
                string kiraSayi = dataGridView2.Rows[selectedRowIndex].Cells["KiraSayi"].Value.ToString();
                string kategori = dataGridView2.Rows[selectedRowIndex].Cells["Kategori"].Value.ToString();

                DialogResult result = MessageBox.Show("Seçili satırı güncellemek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string queryUpdateKitap = "UPDATE kitap SET KitapAd = @KitapAd, Yazar = @Yazar, Sayfasayisi = @Sayfasayisi, Kira =@Kira, KiraSayi=@KiraSayi, Kategori=@Kategori WHERE KitapID = @KitapID";
                        SqlCommand commandUpdateKitap = new SqlCommand(queryUpdateKitap, connection);
                        commandUpdateKitap.Parameters.AddWithValue("@KitapAd", kitapAd);
                        commandUpdateKitap.Parameters.AddWithValue("@Yazar", yazar);
                        commandUpdateKitap.Parameters.AddWithValue("@Sayfasayisi", sayfasayisi);
                        commandUpdateKitap.Parameters.AddWithValue("@KitapID", kitapID);
                        commandUpdateKitap.Parameters.AddWithValue("@Kira", kira);
                        commandUpdateKitap.Parameters.AddWithValue("@KiraSayi", kiraSayi);
                        commandUpdateKitap.Parameters.AddWithValue("@Kategori", kategori);
                        int affectedRows = commandUpdateKitap.ExecuteNonQuery();

                        if (affectedRows > 0)
                        {
                            MessageBox.Show("Satır güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                          
                            DataTable dataTableKitap = new DataTable();
                            SqlDataAdapter adapterKitap = new SqlDataAdapter("SELECT KitapID, KitapAd, Yazar, Kira, KiraSayi, Kategori FROM kitap", connection);
                            adapterKitap.Fill(dataTableKitap);
                            dataGridView2.DataSource = dataTableKitap;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellemek için bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string tc = textBox11.Text;
            string kitapID = textBox12.Text;

            
            string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string queryMusteri = "SELECT * FROM musterikitap WHERE Tc = @Tc";
                SqlCommand commandMusteri = new SqlCommand(queryMusteri, connection);
                commandMusteri.Parameters.AddWithValue("@Tc", tc);

                SqlDataReader readerMusteri = commandMusteri.ExecuteReader();
                if (!readerMusteri.HasRows)
                {
                    MessageBox.Show("Belirtilen Tc numarasına sahip bir müşteri bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    readerMusteri.Close();
                    return;
                }
                readerMusteri.Close();

                string queryKitap = "SELECT * FROM kitap WHERE KitapID = @KitapID";
                SqlCommand commandKitap = new SqlCommand(queryKitap, connection);
                commandKitap.Parameters.AddWithValue("@KitapID", kitapID);

                SqlDataReader readerKitap = commandKitap.ExecuteReader();
                if (!readerKitap.HasRows)
                {
                    MessageBox.Show("Belirtilen Kitap ID'ye sahip bir kitap bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    readerKitap.Close();
                    return;
                }
                readerKitap.Close();

               
                string confirmMessage = $"Seçilen Tc: {tc}\nSeçilen Kitap ID: {kitapID}\n\nBu Seçilen veriler doğru mu?";
                DialogResult result = MessageBox.Show(confirmMessage, "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                  
                    string queryUpdateKitap = "UPDATE kitap SET Kira = 0 WHERE KitapID = @KitapID";
                    SqlCommand commandUpdateKitap = new SqlCommand(queryUpdateKitap, connection);
                    commandUpdateKitap.Parameters.AddWithValue("@KitapID", kitapID);
                    commandUpdateKitap.ExecuteNonQuery();

                   
                    string queryDeleteMusteri = "DELETE FROM musterikitap WHERE Tc = @Tc";
                    SqlCommand commandDeleteMusteri = new SqlCommand(queryDeleteMusteri, connection);
                    commandDeleteMusteri.Parameters.AddWithValue("@Tc", tc);
                    commandDeleteMusteri.ExecuteNonQuery();

                    MessageBox.Show("Kitap başarıyla teslim edildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                   
                    DataTable dataTableMusteriKitap = new DataTable();
                    SqlDataAdapter adapterMusteriKitap = new SqlDataAdapter("SELECT Tc, Ad, Soyad, Adres, IlkTarih, SonTarih, CepNumarasi, KitapAd, KitapID FROM musterikitap", connection);
                    adapterMusteriKitap.Fill(dataTableMusteriKitap);
                    dataGridView1.DataSource = dataTableMusteriKitap;

                    
                    DataTable dataTableKitap = new DataTable();
                    SqlDataAdapter adapterKitap = new SqlDataAdapter("SELECT KitapID, KitapAd, Yazar, Kira, KiraSayi, Kategori FROM kitap", connection);
                    adapterKitap.Fill(dataTableKitap);
                    dataGridView2.DataSource = dataTableKitap;
                }
                else
                {
                    MessageBox.Show("Silme işlemi iptal edildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            bool isVisible = !(label1.Visible && label2.Visible && label3.Visible && label4.Visible && label5.Visible &&
                              label6.Visible && label7.Visible && label8.Visible && label9.Visible && label18.Visible);

            label1.Visible = isVisible;
            label2.Visible = isVisible;
            label3.Visible = isVisible;
            label4.Visible = isVisible;
            label5.Visible = isVisible;
            label6.Visible = isVisible;
            label7.Visible = isVisible;
            label8.Visible = isVisible;
            label9.Visible = isVisible;
            label18.Visible = isVisible;

            textBox1.Visible = isVisible;
            textBox2.Visible = isVisible;
            textBox3.Visible = isVisible;
            textBox4.Visible = isVisible;
            textBox5.Visible = isVisible;
            textBox6.Visible = isVisible;
            textBox7.Visible = isVisible;

            dateTimePicker1.Visible = isVisible;
            dateTimePicker2.Visible = isVisible;

            button1.Visible = isVisible;
            button4.Visible = isVisible;
            button3.Visible = isVisible;
            if (isVisible)
            {
                button8.Text = " ";
                button8.ImageKey = "back.png";
                button8.ImageAlign = ContentAlignment.MiddleCenter;

            }
            else
            {
                button8.Text = "Kitap Kirala";
                button8.ImageKey = "book.png";
                button8.ImageAlign = ContentAlignment.MiddleRight;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            bool isVisible = !(label13.Visible && label14.Visible && label19.Visible);
            label13.Visible = isVisible;
            label14.Visible = isVisible;
            label19.Visible = isVisible;

            textBox11.Visible = isVisible;
            textBox12.Visible = isVisible;

            button7.Visible = isVisible;
            if (isVisible)
            {
                button9.Text = " ";
                button9.ImageKey = "back.png";
                button9.ImageAlign = ContentAlignment.MiddleCenter;

            }
            else
            {
                button9.Text = "Kitap Teslim";
                button9.ImageKey = "book2.png";
                button9.ImageAlign = ContentAlignment.MiddleRight;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            bool isVisible = !(label12.Visible && label11.Visible && label10.Visible && label20.Visible && label22.Visible);
            label10.Visible = isVisible;
            label11.Visible = isVisible;
            label12.Visible = isVisible;
            label20.Visible = isVisible;
            label22.Visible = isVisible;

            textBox10.Visible = isVisible;
            textBox9.Visible = isVisible;
            textBox8.Visible = isVisible;
            comboBox1.Visible = isVisible;

            button2.Visible = isVisible;
            button5.Visible = isVisible;
            button6.Visible = isVisible;
            if (isVisible)
            {
                button10.Text = " ";
                button10.ImageKey = "back.png";
                button10.ImageAlign = ContentAlignment.MiddleCenter;

            }
            else
            {
                button10.Text = "Kitap Ekle";
                button10.ImageKey = "book3.png";
                button10.ImageAlign = ContentAlignment.MiddleRight;
            }

        }

        private void button11_Click(object sender, EventArgs e)
        {
            string searchValue = textBox13.Text;

            if (string.IsNullOrEmpty(searchValue))
            {
                MessageBox.Show("Lütfen kitap adı girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool found = false;

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Cells["KitapAd"].Value != null && row.Cells["KitapAd"].Value.ToString().Contains(searchValue))
                {
                    row.Selected = true;
                    dataGridView2.CurrentCell = row.Cells["KitapAd"];
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                MessageBox.Show("Bu kitap kayıtlı değil.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void button12_Click(object sender, EventArgs e)
        {
            string searchValue = textBox14.Text;

            if (string.IsNullOrEmpty(searchValue))
            {
                MessageBox.Show("Lütfen müşteri adı girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool found = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Ad"].Value != null && row.Cells["Ad"].Value.ToString().Contains(searchValue))
                {
                    row.Selected = true;
                    dataGridView1.CurrentCell = row.Cells["Ad"];
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                MessageBox.Show("Bu kişi kayıtlı değil.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            SmsApiService smsApi = new SmsApiService();
            smsApi.SmsSender(textBox15.Text, "Kitap teslim süreniz geçmiş bulunmaktadır 3 gün içerisinde kitabı getirmezseniz yasal işlem uygulanacaktır!!");
            MessageBox.Show("SMS gönderildi.", "Durum", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

