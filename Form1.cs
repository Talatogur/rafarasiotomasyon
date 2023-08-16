using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;//sql server kullanmak için  

namespace RafArasi2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox2.Visible = false;
            pictureBox3.Visible = false;
            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection baglanti = null;
            try
            {
                baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True");
                baglanti.Open();
                string kullaniciAdi = textBox1.Text;
                string sifre = textBox2.Text;
                string query = "SELECT COUNT(*) FROM Admin WHERE Admin=@kullaniciAdi AND Sifre=@sifre";

                using (SqlCommand command = new SqlCommand(query, baglanti))
                {
                    command.Parameters.AddWithValue("@kullaniciAdi", kullaniciAdi);
                    command.Parameters.AddWithValue("@sifre", sifre);

                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Giriş başarılı! " + kullaniciAdi);
                        textBox1.Clear();
                        textBox2.Clear();
                        Form1 form1 = Application.OpenForms["Form1"] as Form1;
                        if (form1 != null && !form1.IsDisposed)
                        {
                            form1.FormClosing += Form1_FormClosing;
                            form1.Close(); 
                        }
                        AdminAna adminAnaForm = new AdminAna();
                        adminAnaForm.FormClosed += AdminAna_FormClosed;
                        adminAnaForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı adı veya şifre yanlış!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlanılamadı!" + ex.ToString());
            }
            finally
            {
                if (baglanti != null)
                    baglanti.Close();
            }
        }

        private void AdminAna_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1 form1 = Application.OpenForms["Form1"] as Form1;
            if (form1 != null && form1.Visible == false)
            {
                form1.Show();
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection baglanti = null;
            try
            {
                baglanti = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True");
                baglanti.Open();
                string kullaniciAdi = textBox3.Text;
                string sifre = textBox4.Text;
                string query = "SELECT COUNT(*) FROM Kasiyer WHERE Kasiyer=@kullaniciAdi AND Sifre=@sifre";

                using (SqlCommand command = new SqlCommand(query, baglanti))
                {
                    command.Parameters.AddWithValue("@kullaniciAdi", kullaniciAdi);
                    command.Parameters.AddWithValue("@sifre", sifre);

                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Giriş başarılı! " + kullaniciAdi);

                        textBox3.Clear();
                        textBox4.Clear();
                        Form1 form1 = Application.OpenForms["Form1"] as Form1;
                        if (form1 != null && !form1.IsDisposed)
                        {
                            form1.FormClosing += Form1_FormClosing;
                            form1.Close(); 
                        }

                        KasiyerAna kasiyerAnaForm = new KasiyerAna();
                        kasiyerAnaForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı adı veya şifre yanlış!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlanılamadı!" + ex.ToString());
            }
            finally
            {
                if (baglanti != null)
                    baglanti.Close();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; // Form1 kapatılmasını iptal et
            ((Form)sender).Hide(); 
        }


        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox2.Visible = true;
            pictureBox3.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            button1.Visible = true;

            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            button2.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pictureBox2.Visible = false;
            pictureBox3.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            button1.Visible = false;

            pictureBox4.Visible = true;
            pictureBox5.Visible = true;
            textBox3.Visible = true;
            textBox4.Visible = true;
            button2.Visible = true;
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            pictureBox2.Visible = false;
            pictureBox3.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            button1.Visible = false;

            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            button2.Visible = false;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Tüm açık formları kapat
            foreach (Form form in Application.OpenForms)
            {
                if (form != this) // Aktif formun dışındaki formları kapat
                {
                    form.Close();
                }
            }
            Environment.Exit(0);
            //Application.Exit();
        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {

        }
    }
}
