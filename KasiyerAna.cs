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
using QRCoder;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Printing;
using RafArasi2.Models;
  
namespace RafArasi2
{
    public partial class KasiyerAna : Form
    {
        public KasiyerAna()
        {
            InitializeComponent();
        }
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice captureDevice;
        private void KasiyerAna_Load(object sender, EventArgs e)
        {
            textBox6.ReadOnly = false;
            textBox6.Visible = false;
            MaskedTextBox maskedTextBox1 = new MaskedTextBox();
            maskedTextBox1.Mask = "00:00";

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

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            for (int i = 0; i < filterInfoCollection.Count; i++)
            {
                comboBox1.Items.Add(filterInfoCollection[i].Name);
            }

            try
            {
                using (SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
                {
                    baglanti.Open();

                    string sql = "SELECT * FROM Musteri";
                    SqlCommand command = new SqlCommand(sql, baglanti);
                    SqlDataReader reader = command.ExecuteReader();

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
                        string cikisSaati = reader["CikisSaati"].ToString();
                        string girisSaati = reader["GirisSaati"].ToString();
                        DateTime dateTime = DateTime.Parse(tarih);
                        string Tarih = dateTime.ToString("yyyy-MM-dd");

                        dataGridView1.Rows.Add(ID, musteriAd, soyad, cepNumarasi, ikram, Tarih, kod, masaNo, girisSaati, cikisSaati);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            label7.Text = DateTime.Now.ToLongDateString();
            label8.Text = DateTime.Now.ToLongTimeString();
        }

        private void MusteriArama(string ad)
        {
           
            using (SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
            {
                baglanti.Open();

                string sql = "SELECT * FROM Musteri WHERE Ad LIKE @ad";
                SqlCommand command = new SqlCommand(sql, baglanti);
                command.Parameters.AddWithValue("@ad", "%" + ad + "%");

               
                SqlDataReader reader = command.ExecuteReader();

               
                dataGridView1.Rows.Clear();

               
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
                    string cikisSaati = reader["CikisSaati"].ToString();
                    string girisSaati = DateTime.Now.ToString("HH:mm");
                    DateTime dateTime = DateTime.Parse(tarih);
                    string Tarih = dateTime.ToString("yyyy-MM-dd");

                    dataGridView1.Rows.Add(ID, musteriAd, soyad, cepNumarasi, ikram, Tarih, kod, masaNo, girisSaati, cikisSaati);

                }

                reader.Close();
                baglanti.Close();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (!checkBox1.Checked && string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Kod alanı boş bırakılamaz.");
                return;
            }
            if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Ad ve Soyad boş bırakılamaz.");
                return;
            }

            try
            {
                using (SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
                {
                    baglanti.Open();

                    string selectSql = "SELECT COUNT(*) FROM Musteri WHERE Kod = @Kod";
                    SqlCommand selectCommand = new SqlCommand(selectSql, baglanti);
                    selectCommand.Parameters.AddWithValue("@Kod", textBox1.Text);
                    int kodsayisi = (int)selectCommand.ExecuteScalar();

                    if (kodsayisi > 0)
                    {
                        MessageBox.Show("Bu kod zaten kullanılıyor. Başka bir kod deneyin.");
                        return;
                    }

                    string selectNameSql = "SELECT COUNT(*) FROM Musteri WHERE Ad = @Ad AND Soyad = @Soyad";
                    SqlCommand selectNameCommand = new SqlCommand(selectNameSql, baglanti);
                    selectNameCommand.Parameters.AddWithValue("@Ad", textBox2.Text);
                    selectNameCommand.Parameters.AddWithValue("@Soyad", textBox3.Text);
                    int existingNameCount = (int)selectNameCommand.ExecuteScalar();

                    if (existingNameCount > 0)
                    {
                        DialogResult result = MessageBox.Show("Aynı isim ve soy isimli biri var. Kaydetmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }

                    string selectMasaDurumSql = "SELECT KulDurum FROM Masa WHERE MasaNo = @MasaNo";
                    SqlCommand selectMasaDurumCommand = new SqlCommand(selectMasaDurumSql, baglanti);
                    selectMasaDurumCommand.Parameters.AddWithValue("@MasaNo", textBox7.Text);
                    object masaDurumResult = selectMasaDurumCommand.ExecuteScalar();
                    bool masaDolu = masaDurumResult != null && (bool)masaDurumResult;

                    if (masaDolu)
                    {
                        MessageBox.Show("Seçilen masa dolu. Başka bir masa seçin.");
                        return;
                    }

                    string selectMasaSql = "SELECT COUNT(*) FROM Masa WHERE MasaNo = @MasaNo";
                    SqlCommand selectMasaCommand = new SqlCommand(selectMasaSql, baglanti);
                    selectMasaCommand.Parameters.AddWithValue("@MasaNo", textBox7.Text);
                    int masaCount = (int)selectMasaCommand.ExecuteScalar();

                    if (masaCount == 0)
                    {
                        MessageBox.Show("Seçilen masa bulunamadı. Lütfen masa numarasını kontrol edin.");
                        return;
                    }

                    string sql = "INSERT INTO Musteri (Ad, Soyad, CepNumarasi, Ikram, Tarih, Kod, MasaNo, GirisSaati, CikisSaati) " +
                                 "VALUES (@Ad, @Soyad, @CepNumarasi, @Ikram, GETDATE(), @Kod, @MasaNo, @GirisSaati, @CikisSaati)";
                    SqlCommand command = new SqlCommand(sql, baglanti);
                    command.Parameters.AddWithValue("@Ad", textBox2.Text);
                    command.Parameters.AddWithValue("@Soyad", textBox3.Text);
                    command.Parameters.AddWithValue("@CepNumarasi", textBox4.Text);
                    command.Parameters.AddWithValue("@Ikram", checkBox1.Checked);
                    command.Parameters.AddWithValue("@Kod", (checkBox1.Checked && string.IsNullOrEmpty(textBox1.Text)) ? DBNull.Value : (object)textBox1.Text);
                    command.Parameters.AddWithValue("@MasaNo", textBox7.Text);
                    command.Parameters.AddWithValue("@GirisSaati", DateTime.Now.ToString("HH:mm"));
                    command.Parameters.AddWithValue("@CikisSaati", dateTimePicker1.Value);

                    int affectedRows = command.ExecuteNonQuery();

                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Veri başarıyla eklendi.");

                        // MasaNo'ya göre Masa tablosundaki KulDurum değerini true olarak güncelle
                        UpdateMasaDurum(baglanti, textBox7.Text, true);

                        dataGridView1.Rows.Clear();

                        string selectAllSql = "SELECT * FROM Musteri";
                        SqlCommand selectAllCommand = new SqlCommand(selectAllSql, baglanti);
                        SqlDataReader reader = selectAllCommand.ExecuteReader();

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
                            string cikisSaati = reader["CikisSaati"].ToString();
                            string girisSaati = reader["GirisSaati"].ToString();
                            DateTime dateTime = DateTime.Parse(tarih);
                            string Tarih = dateTime.ToString("yyyy-MM-dd");

                            dataGridView1.Rows.Add(ID, musteriAd, soyad, cepNumarasi, ikram, Tarih, kod, masaNo, girisSaati, cikisSaati);
                        }

                        reader.Close();
                    }
                    else
                    {
                        MessageBox.Show("Veri eklenirken bir hata oluştu.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }
        private void UpdateMasaDurum(SqlConnection baglanti, string masaNo, bool durum)
        {
            try
            {
                string updateSql = "UPDATE Masa SET KulDurum = @Durum WHERE MasaNo = @MasaNo";
                SqlCommand updateCommand = new SqlCommand(updateSql, baglanti);
                updateCommand.Parameters.AddWithValue("@Durum", durum);
                updateCommand.Parameters.AddWithValue("@MasaNo", masaNo);

                updateCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }



        private void textBox5_MouseClick(object sender, MouseEventArgs e)
        {
            textBox5.Clear();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            string musteriAdBulma = textBox5.Text;
            MusteriArama(musteriAdBulma);
        }

        private void button2_Click(object sender, EventArgs e) 
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string randomCode = new string(Enumerable.Repeat(chars, 11)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            textBox1.Text = randomCode;

            string metin = textBox1.Text;

            // QR kod oluştur
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(metin, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(10);

            // PictureBox1'e QR kodu yükle
            pictureBox1.Image = qrCodeImage;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
            
        }

        private void KasiyerAna_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (captureDevice != null && captureDevice.IsRunning)
            {
                captureDevice.Stop();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            captureDevice = new VideoCaptureDevice(filterInfoCollection[comboBox1.SelectedIndex].MonikerString);
            captureDevice.NewFrame += CaptureDevice_NewFrame; // tab tab
            captureDevice.Start();
            timer1.Start();

        }

        private void CaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox2.Image=(Bitmap)eventArgs.Frame.Clone();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null)
            {
                BarcodeReader barcodeReader = new BarcodeReader();
                Result result = barcodeReader.Decode((Bitmap)pictureBox2.Image);
                if (result != null)
                {
                    textBox6.Text = result.ToString();
                    timer1.Stop();
                    if (captureDevice.IsRunning)
                    {
                        captureDevice.Stop();
                        captureDevice = null; // captureDevice'ı sıfırla
                        pictureBox2.Image = default;
                        comboBox1.Visible = false;
                        pictureBox2.Visible = false;
                        button3.Visible = false;
                        textBox6.Visible = true;
                        button5.Visible = true;
                        button6.Visible = true;
                        listBox1.Visible = true;
                    }
                }
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.Visible)
            {
                comboBox1.Visible = false;
                pictureBox2.Visible = false;
                button3.Visible = false;

                if (captureDevice != null && captureDevice.IsRunning)
                {
                    captureDevice.Stop();
                }
            }
            else
            {
                comboBox1.Visible = true;
                pictureBox2.Visible = true;
                button3.Visible = true;

                if (captureDevice != null && !captureDevice.IsRunning)
                {
                    captureDevice.Start();
                }
            }
        }


        private void button5_Click(object sender, EventArgs e)
        {
            string arananKod = textBox6.Text;

            listBox1.Items.Clear();

            using (SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
            {
                baglanti.Open();

                string sql = "SELECT * FROM Musteri WHERE Kod = @kod";
                SqlCommand command = new SqlCommand(sql, baglanti);
                command.Parameters.AddWithValue("@kod", arananKod);

              
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    // Verileri oku ve işle
                    while (reader.Read())
                    {
                       
                        string musteriID = reader["ID"].ToString();
                        string musteriAd = reader["Ad"].ToString();
                        string soyad = reader["Soyad"].ToString();
                        bool ikram = (bool)reader["Ikram"];

                        string ikramDurumu = ikram ? "Alındı" : "Alınmadı"; // True ise "Aldı", False ise "Almadı" olarak atama yap

                        string itemText = $"ID: {musteriID}, Ad: {musteriAd}, Soyad: {soyad}, Ikram: {ikramDurumu}";
                        listBox1.Items.Add(itemText);
                    }
                }
                else
                {
                    MessageBox.Show("Böyle bir müşteri bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                reader.Close();
                baglanti.Close();
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            string arananKod = textBox6.Text;

            if (string.IsNullOrEmpty(arananKod))
            {
                MessageBox.Show("Lütfen bir kod girin.");
                return;
            }

            using (SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
            {
                baglanti.Open();

                string selectSql = "SELECT * FROM Musteri WHERE Kod = @kod";
                SqlCommand selectCommand = new SqlCommand(selectSql, baglanti);
                selectCommand.Parameters.AddWithValue("@kod", arananKod);

                SqlDataReader reader = selectCommand.ExecuteReader();

                if (reader.Read())
                {
                    string musteriID = reader["ID"].ToString();
                    string musteriAd = reader["Ad"].ToString();
                    string ikramDurumu = reader["Ikram"].ToString();

                    if (ikramDurumu.ToLower() == "false")
                    {
                        reader.Close();

                        string updateSql = "UPDATE Musteri SET Ikram = @ikram WHERE ID = @musteriID";
                        SqlCommand updateCommand = new SqlCommand(updateSql, baglanti);
                        updateCommand.Parameters.AddWithValue("@ikram", true);
                        updateCommand.Parameters.AddWithValue("@musteriID", musteriID);
                        int affectedRows = updateCommand.ExecuteNonQuery();

                        if (affectedRows > 0)
                        {
                            MessageBox.Show(musteriID + " numaralı " + musteriAd + " ikram aldı.");

                            textBox6.Visible = false;
                            button5.Visible = false;
                            button6.Visible = false;
                            listBox1.Visible = false;
                            listBox1.Items.Clear();

                            // DataGridView'i güncelle
                            dataGridView1.Rows.Clear();
                            string gridSelectSql = "SELECT * FROM Musteri";
                            SqlCommand gridSelectCommand = new SqlCommand(gridSelectSql, baglanti);
                            SqlDataReader gridReader = gridSelectCommand.ExecuteReader();
                            while (gridReader.Read())
                            {
                                string ID = reader["ID"].ToString();
                                string musteriAdi = reader["Ad"].ToString();
                                string soyad = reader["Soyad"].ToString();
                                string cepNumarasi = reader["CepNumarasi"].ToString();
                                string ikram = reader["Ikram"].ToString();
                                string tarih = reader["Tarih"].ToString();
                                string kod = reader["Kod"].ToString();
                                string masaNo = reader["MasaNo"].ToString();
                                string cikisSaati = reader["CikisSaati"].ToString();
                                string girisSaati = reader["GirisSaati"].ToString();
                                DateTime dateTime = DateTime.Parse(tarih);
                                string Tarih = dateTime.ToString("yyyy-MM-dd");

                                dataGridView1.Rows.Add(ID, musteriAdi, soyad, cepNumarasi, ikram, Tarih, kod, masaNo, girisSaati, cikisSaati);
                            }
                            gridReader.Close();
                        }
                        else
                        {
                            MessageBox.Show("Ikram verilirken bir hata oluştu.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Seçili kişiye zaten ikram yapılmış.");
                        textBox6.Visible = false;
                        button5.Visible = false;
                        button6.Visible = false;
                        listBox1.Visible = false;
                        listBox1.Items.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("Belirtilen kod ile eşleşen bir kayıt bulunamadı.");
                    textBox6.Visible = false;
                    button5.Visible = false;
                    button6.Visible = false;
                    listBox1.Visible = false;
                    listBox1.Items.Clear();
                }

                reader.Close();
                baglanti.Close();
            }
        }



        private void button7_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JPEG Image|*.jpg|PNG Image|*.png|BMP Image|*.bmp";
                saveFileDialog.Title = "Resmi Kaydet";
                saveFileDialog.FileName = "resim";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string dosyaYolu = saveFileDialog.FileName;
                    ImageFormat format;

                    switch (Path.GetExtension(dosyaYolu).ToLower())
                    {
                        case ".jpg":
                        case ".jpeg":
                            format = ImageFormat.Jpeg;
                            break;
                        case ".png":
                            format = ImageFormat.Png;
                            break;
                        case ".bmp":
                            format = ImageFormat.Bmp;
                            break;
                        default:
                            MessageBox.Show("Desteklenmeyen dosya formatı.");
                            return;
                    }

                    try
                    {
                        pictureBox1.Image.Save(dosyaYolu, format);
                        MessageBox.Show("Resim başarıyla kaydedildi.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Kaydetme hatası: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Kaydedilecek bir resim yok.");
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                PrintDocument printDocument = new PrintDocument();
                printDocument.PrintPage += PrintDocument_PrintPage;

                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDocument;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
            else
            {
                MessageBox.Show("Yazdırılacak bir resim yok.");
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Image resim = pictureBox1.Image;
            Point baslangicNoktasi = new Point(100, 100); // Yazdırma başlangıç noktası

            // Kağıt boyutları
            float sayfaGenislik = e.PageBounds.Width;
            float sayfaYukseklik = e.PageBounds.Height;

            // Resmin boyutları
            float resimGenislik = resim.Width;
            float resimYukseklik = resim.Height;

            // Resmi ölçeklendirme oranları
            float oranGenislik = sayfaGenislik / resimGenislik;
            float oranYukseklik = sayfaYukseklik / resimYukseklik;

            // Ölçeklendirme faktörü
            float olcek = Math.Min(oranGenislik, oranYukseklik);

            // Yeni resim boyutları
            float yeniGenislik = resimGenislik * olcek;
            float yeniYukseklik = resimYukseklik * olcek;

            // Resmi ölçeklendirerek çizdir
            RectangleF hedefRect = new RectangleF(baslangicNoktasi.X, baslangicNoktasi.Y, yeniGenislik, yeniYukseklik);
            e.Graphics.DrawImage(resim, hedefRect);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            KasiyerAna kasiyerAnaForm = Application.OpenForms["KasiyerAna"] as KasiyerAna;
            if (kasiyerAnaForm != null && !kasiyerAnaForm.IsDisposed)
            {
                kasiyerAnaForm.Close();
            }

        
            form1.Show();
        }

        private void KasiyerAna_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (form1 != null && form1.Visible == false)
            {
                form1.Show();
            }
        }
        private void button10_Click(object sender, EventArgs e)
        {
            SmsApiService smsApi = new SmsApiService();
            smsApi.SmsSender(textBox4.Text,textBox1.Text);
            MessageBox.Show("SMS gönderildi.", "Durum", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void button11_Click(object sender, EventArgs e)
        {
            Kitap kitapForm = new Kitap();
            kitapForm.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Masa masaForm = new Masa();
            masaForm.Show();
        }
    }
}