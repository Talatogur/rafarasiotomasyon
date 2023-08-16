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


namespace RafArasi2
{
    public partial class KasiyerEkle : Form
    {
        private AdminAna AdminAna;

        public KasiyerEkle(AdminAna form)
        {
            InitializeComponent();
            AdminAna = form;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
                {
                    baglanti.Open();

                    string kontrolSql = "SELECT COUNT(*) FROM Kasiyer WHERE Kasiyer = @Kasiyer";
                    SqlCommand kontrolCommand = new SqlCommand(kontrolSql, baglanti);
                    kontrolCommand.Parameters.AddWithValue("@Kasiyer", textBox2.Text);

                    int kasiyerSayisi = Convert.ToInt32(kontrolCommand.ExecuteScalar());

                    if (kasiyerSayisi > 0)
                    {
                       
                        MessageBox.Show("Bu kullanıcı adı zaten mevcut. Lütfen farklı bir kullanıcı adı seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        string kontrolSql2 = "SELECT COUNT(*) FROM Kasiyer WHERE Ad = @Ad AND Soyad = @Soyad";
                        SqlCommand kontrolCommand2 = new SqlCommand(kontrolSql2, baglanti);
                        kontrolCommand2.Parameters.AddWithValue("@Ad", textBox4.Text);
                        kontrolCommand2.Parameters.AddWithValue("@Soyad", textBox5.Text);

                        int duplicateCount = Convert.ToInt32(kontrolCommand2.ExecuteScalar());

                        if (duplicateCount > 0)
                        {
                            
                            DialogResult result = MessageBox.Show("Bu ad ve soyad zaten mevcut. Kaydetmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                               
                                string sql = "INSERT INTO Kasiyer (Kasiyer, Sifre, Ad, Soyad, Adres, Mail, DTarihi, CepNumarasi, KayitTarihi) VALUES (@Kasiyer, @Sifre, @Ad, @Soyad, @Adres, @Mail, @DTarihi, @CepNumarasi, GETDATE())";
                                SqlCommand command = new SqlCommand(sql, baglanti);
                                command.Parameters.AddWithValue("@Kasiyer", textBox2.Text);
                                command.Parameters.AddWithValue("@Sifre", textBox3.Text);
                                command.Parameters.AddWithValue("@Ad", textBox4.Text);
                                command.Parameters.AddWithValue("@Soyad", textBox5.Text);
                                command.Parameters.AddWithValue("@Adres", textBox6.Text);
                                command.Parameters.AddWithValue("@Mail", textBox7.Text);
                                command.Parameters.AddWithValue("@CepNumarasi", textBox8.Text);
                                command.Parameters.AddWithValue("@DTarihi", dateTimePicker1.Value);

                                int affectedRows = command.ExecuteNonQuery();

                                if (affectedRows > 0)
                                {
                                    MessageBox.Show("Veri başarıyla eklendi.");
                                    AdminAna.VerileriGetir();
                                }
                                else
                                {
                                    MessageBox.Show("Veri eklenirken bir hata oluştu.");
                                }
                            }
                            else if (result == DialogResult.No)
                            {
                             
                                MessageBox.Show("Kaydetme işlemi iptal edildi.");
                            }
                        }
                        else
                        {
                            
                            string sql = "INSERT INTO Kasiyer (Kasiyer, Sifre, Ad, Soyad, Adres, Mail, DTarihi, CepNumarasi, KayitTarihi) VALUES (@Kasiyer, @Sifre, @Ad, @Soyad, @Adres, @Mail, @DTarihi, @CepNumarasi, GETDATE())";
                            SqlCommand command = new SqlCommand(sql, baglanti);
                            command.Parameters.AddWithValue("@Kasiyer", textBox2.Text);
                            command.Parameters.AddWithValue("@Sifre", textBox3.Text);
                            command.Parameters.AddWithValue("@Ad", textBox4.Text);
                            command.Parameters.AddWithValue("@Soyad", textBox5.Text);
                            command.Parameters.AddWithValue("@Adres", textBox6.Text);
                            command.Parameters.AddWithValue("@Mail", textBox7.Text);
                            command.Parameters.AddWithValue("@CepNumarasi", textBox8.Text);
                            command.Parameters.AddWithValue("@DTarihi", dateTimePicker1.Value);

                            int affectedRows = command.ExecuteNonQuery();

                            if (affectedRows > 0)
                            {
                                MessageBox.Show("Veri başarıyla eklendi.");
                                AdminAna.VerileriGetir();
                            }
                            else
                            {
                                MessageBox.Show("Veri eklenirken bir hata oluştu.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }
    }
}