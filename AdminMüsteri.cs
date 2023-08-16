using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RafArasi2
{
    public partial class AdminMüsteri : Form
    {
        public AdminMüsteri()
        {
            InitializeComponent();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        //private bool saveTable = false;

        private void AdminMüsteri_Load(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
                {
                    baglanti.Open();

                    string sql = "SELECT ID, Ad, Soyad, CepNumarasi, Ikram, Tarih, Kod, MasaNo, GirisSaati, CikisSaati FROM Musteri";
                    SqlCommand command = new SqlCommand(sql, baglanti);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string tarih = DateTime.Now.ToString("dd-MM-yyyy");

                SaveFileDialog saveDialog = new SaveFileDialog();
                string dosyaAdi = tarih + ".xlsx";
                saveDialog.FileName = dosyaAdi;
                saveDialog.Filter = "Excel Dosyası|*.xlsx|Metin Dosyası|*.txt"; // Kaydedilecek dosya formatları (Excel dosyası ve metin dosyası)

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    if (saveDialog.FilterIndex == 1) // Seçilen dosya türü Excel dosyası (*.xlsx)
                    {
                        // Excel uygulamasını oluştur
                        Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                        excelApp.Visible = false;

                        // Yeni bir Excel çalışma kitabı oluştur
                        Microsoft.Office.Interop.Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);

                        // Aktif sayfayı seç
                        Microsoft.Office.Interop.Excel.Worksheet worksheet = workbook.ActiveSheet;

                        // Sütun başlıklarını ekle
                        string[] sütunBasliklari = { "Id", "Ad", "Soyad", "CepNumarası", "Ikram", "Tarih", "Kod", "MasaNo", "GirisSaati", "CikisSaati" };
                        for (int j = 0; j < sütunBasliklari.Length; j++)
                        {
                            worksheet.Cells[1, j + 1] = sütunBasliklari[j];
                        }

                        // DataGridView verilerini Excel sayfasına kopyala
                        for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                        {
                            for (int j = 0; j < dataGridView1.Columns.Count; j++)
                            {
                                worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                            }
                        }

                        // Excel dosyasını kaydet
                        workbook.SaveAs(saveDialog.FileName);
                        workbook.Close();
                        excelApp.Quit();

                        MessageBox.Show("Veriler Excel dosyasına başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (saveDialog.FilterIndex == 2) // Seçilen dosya türü Metin dosyası (*.txt)
                    {
                        // Metin dosyasını oluştur ve verileri yaz
                        using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                        {
                            // Sütun başlıklarını yaz
                            string[] sütunBasliklari = { "Id", "Ad", "Soyad", "CepNumarası", "Ikram", "Tarih",  "Kod", "MasaNo", "GirisSaati", "CikisSaati" };
                            writer.WriteLine(string.Join("\t", sütunBasliklari));

                            // DataGridView verilerini metin dosyasına yaz
                            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                            {
                                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                                {
                                    writer.Write(dataGridView1.Rows[i].Cells[j].Value.ToString() + "\t");
                                }
                                writer.WriteLine();
                            }
                        }

                        MessageBox.Show("Veriler Metin dosyasına başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Tabloyu silmeden önce kaydetmek ister misiniz?", "Onay", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Tabloyu kaydet
                button3_Click(sender, e); // Button 3'e tıklandığında yapılacak işlemler
            }
            else if (result == DialogResult.No)
            {
                try
                {
                    using (SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
                    {
                        baglanti.Open();

                        // Musteri tablosunu sil
                        string dropTableQuery = "DROP TABLE IF EXISTS Musteri";
                        SqlCommand dropTableCommand = new SqlCommand(dropTableQuery, baglanti);
                        dropTableCommand.ExecuteNonQuery();

                        // Yeni Musteri tablosunu oluştur
                        string createTableQuery = "CREATE TABLE Musteri (ID INT IDENTITY(1,1) PRIMARY KEY, Ad NVARCHAR(50), Soyad NVARCHAR(50), CepNumarasi NVARCHAR(20), Ikram BIT, Tarih DATETIME, Kod NVARCHAR(50), MasaNo NVARCHAR(50), GirisSaati TIME, CikisSaati TIME)";
                        SqlCommand createTableCommand = new SqlCommand(createTableQuery, baglanti);
                        createTableCommand.ExecuteNonQuery();

                        MessageBox.Show("Musteri tablosu silindi ve yeni tablo oluşturuldu.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // DataGridView'i temizle
                        dataGridView1.Rows.Clear();
                        dataGridView1.Columns.Clear();

                        // Yeni tabloyu yeniden yükle
                        string selectQuery = "SELECT * FROM Musteri";
                        SqlCommand selectCommand = new SqlCommand(selectQuery, baglanti);
                        SqlDataReader reader = selectCommand.ExecuteReader();

                        // Sütunları yeniden oluştur
                        dataGridView1.Columns.Add("ID", "ID");
                        dataGridView1.Columns.Add("Ad", "Ad");
                        dataGridView1.Columns.Add("Soyad", "Soyad");
                        dataGridView1.Columns.Add("CepNumarasi", "Cep Numarası");
                        dataGridView1.Columns.Add("Ikram", "Ikram");
                        dataGridView1.Columns.Add("Tarih", "Tarih");
                        dataGridView1.Columns.Add("Kod", "Kod");
                        dataGridView1.Columns.Add("MasaNo", "Masa No");
                        dataGridView1.Columns.Add("GirisSaati", "Giriş Saati");
                        dataGridView1.Columns.Add("CikisSaati", "Çıkış Saati");

                        // Verileri doldur
                        while (reader.Read())
                        {
                            string ID = reader["ID"].ToString();
                            string musteriAd = reader["Ad"].ToString();
                            string soyad = reader["Soyad"].ToString();
                            string cepNumarasi = reader["CepNumarasi"].ToString();
                            string ikram = reader["Ikram"].ToString();
                            string tarih = reader["Tarih"].ToString();
                            string kod = reader["Kod"].ToString();
                            string masaNo = reader["MasaNo"].ToString();
                            string girisSaati = reader["GirisSaati"].ToString();
                            string cikisSaati = reader["CikisSaati"].ToString();

                            dataGridView1.Rows.Add(ID, musteriAd, soyad, cepNumarasi, ikram, tarih, kod, masaNo, girisSaati, cikisSaati);
                        }

                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata oluştu: " + ex.Message);
                }
            }
            else if (result == DialogResult.Cancel)
            {
                MessageBox.Show("Tablo silme işlemi iptal edildi.");
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Seçili satırın indeksini al
                int selectedRowIndex = dataGridView1.SelectedRows[0].Index;

                // Seçili satırdan ID ve Ad değerlerini al
                string selectedID = dataGridView1.Rows[selectedRowIndex].Cells["ID"].Value.ToString();
                string selectedName = dataGridView1.Rows[selectedRowIndex].Cells["Ad"].Value.ToString();

                // Silme işleminden önce onay al
                DialogResult result = MessageBox.Show(selectedID + " numaralı " + selectedName + " isimli müşteriyi silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
                        {
                            baglanti.Open();

                            string sql = "DELETE FROM Musteri WHERE ID = @ID";
                            SqlCommand command = new SqlCommand(sql, baglanti);
                            command.Parameters.AddWithValue("@ID", selectedID);
                            command.ExecuteNonQuery();

                            // DataGridView'den seçili satırı sil
                            dataGridView1.Rows.RemoveAt(selectedRowIndex);

                            MessageBox.Show("Müşteri başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata oluştu: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silinecek bir müşteri seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Seçili satırın indeksini al
                int selectedRowIndex = dataGridView1.SelectedRows[0].Index;

                // Seçili satırdan ID, Ad ve Soyad değerlerini al
                string selectedID = dataGridView1.Rows[selectedRowIndex].Cells["ID"].Value.ToString();
                string selectedAd = dataGridView1.Rows[selectedRowIndex].Cells["Ad"].Value.ToString();
                string selectedSoyad = dataGridView1.Rows[selectedRowIndex].Cells["Soyad"].Value.ToString();

                // Kullanıcıdan güncelleme işlemini onaylamasını iste
                DialogResult result = MessageBox.Show("ID: " + selectedID + " olan müşterinin verilerini güncellemek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
                        {
                            baglanti.Open();

                            // Ad ve Soyad daha önce kaydedilmişse uyarı mesajı göster
                            string checkIfExistsQuery = "SELECT COUNT(*) FROM Musteri WHERE Ad = @Ad AND Soyad = @Soyad AND ID <> @ID";
                            SqlCommand checkIfExistsCommand = new SqlCommand(checkIfExistsQuery, baglanti);
                            checkIfExistsCommand.Parameters.AddWithValue("@Ad", selectedAd);
                            checkIfExistsCommand.Parameters.AddWithValue("@Soyad", selectedSoyad);
                            checkIfExistsCommand.Parameters.AddWithValue("@ID", selectedID);
                            int existingCount = (int)checkIfExistsCommand.ExecuteScalar();

                            if (existingCount > 0)
                            {
                                DialogResult overwriteResult = MessageBox.Show("Bu isim ve soyada sahip bir müşteri zaten kaydedilmiş. Güncellemeyi devam ettirmek istediğinizden emin misiniz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (overwriteResult == DialogResult.No)
                                {
                                    return; // Güncelleme işlemini iptal et
                                }
                            }

                            // Diğer verileri güncelle
                            string updateQuery = "UPDATE Musteri SET Ad = @Ad, Soyad = @Soyad, CepNumarasi = @CepNumarasi, Ikram = @Ikram, Tarih = @Tarih, Kod = @Kod, MasaNo = @MasaNo, GirisSaati = @GirisSaati, CikisSaati = @CikisSaati WHERE ID = @ID";
                            SqlCommand updateCommand = new SqlCommand(updateQuery, baglanti);
                            updateCommand.Parameters.AddWithValue("@Ad", selectedAd);
                            updateCommand.Parameters.AddWithValue("@Soyad", selectedSoyad);
                            updateCommand.Parameters.AddWithValue("@CepNumarasi", dataGridView1.Rows[selectedRowIndex].Cells["CepNumarasi"].Value.ToString());
                            updateCommand.Parameters.AddWithValue("@Ikram", dataGridView1.Rows[selectedRowIndex].Cells["Ikram"].Value.ToString());
                            updateCommand.Parameters.AddWithValue("@Tarih", dataGridView1.Rows[selectedRowIndex].Cells["Tarih"].Value.ToString());
                            updateCommand.Parameters.AddWithValue("@Kod", dataGridView1.Rows[selectedRowIndex].Cells["Kod"].Value.ToString());
                            updateCommand.Parameters.AddWithValue("@MasaNo", dataGridView1.Rows[selectedRowIndex].Cells["MasaNo"].Value.ToString());
                            updateCommand.Parameters.AddWithValue("@GirisSaati", dataGridView1.Rows[selectedRowIndex].Cells["GirisSaati"].Value.ToString());
                            updateCommand.Parameters.AddWithValue("@CikisSaati", dataGridView1.Rows[selectedRowIndex].Cells["CikisSaati"].Value.ToString());
                            updateCommand.Parameters.AddWithValue("@ID", selectedID);
                            updateCommand.ExecuteNonQuery();

                            MessageBox.Show("Müşteri verileri başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata oluştu: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellenecek bir müşteri seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
}
