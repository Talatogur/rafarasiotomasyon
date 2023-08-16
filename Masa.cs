using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RafArasi2
{
    public partial class Masa : Form
    {
        public Masa()
        {
            InitializeComponent();
        }

        private void Masa_Load(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
                {
                    connection.Open();

                    string query = "SELECT MasaNo, SesDurum, KulDurum FROM Masa";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable;

                    // DataGridView'deki masaların renklerini güncelle
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["KulDurum"].Value != null)
                        {
                            bool kullanımDurumu = (bool)row.Cells["KulDurum"].Value;
                            if (kullanımDurumu)
                            {
                                row.DefaultCellStyle.BackColor = Color.LightCoral;
                            }
                            else
                            {
                                row.DefaultCellStyle.BackColor = Color.LightGreen;
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

        private void UpdateKulDurum()
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
            {
                connection.Open();

                string selectQuery = "SELECT MasaNo FROM musteri WHERE CikisSaati <= GETDATE()";
                SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                SqlDataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    string masaNo = reader["MasaNo"].ToString();

                    string updateQuery = "UPDATE Masa SET KulDurum = 0 WHERE MasaNo = @MasaNo";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@MasaNo", masaNo);
                    updateCommand.ExecuteNonQuery();
                }

                reader.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string masaNo = textBox1.Text;
                bool sesDurum = checkBox1.Checked;
                bool kulDurum = false; // KulDurum'un false olarak ayarlandığını varsayalım

                using (SqlConnection connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
                {
                    connection.Open();

                    // Kontrol etmek için aynı masa numarasının veritabanında olup olmadığını sorgulayalım
                    string checkQuery = "SELECT COUNT(*) FROM Masa WHERE MasaNo = @MasaNo";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@MasaNo", masaNo);
                    int existingCount = (int)checkCommand.ExecuteScalar();

                    if (existingCount > 0)
                    {
                        MessageBox.Show("Bu masa numarası zaten mevcut.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO Masa (MasaNo, SesDurum, KulDurum) VALUES (@MasaNo, @SesDurum, @KulDurum)";
                        SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                        insertCommand.Parameters.AddWithValue("@MasaNo", masaNo);
                        insertCommand.Parameters.AddWithValue("@SesDurum", sesDurum);
                        insertCommand.Parameters.AddWithValue("@KulDurum", kulDurum);
                        insertCommand.ExecuteNonQuery();

                        MessageBox.Show("Veri başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // DataGridView'i yenile
                        string selectQuery = "SELECT MasaNo, SesDurum, KulDurum FROM Masa";
                        SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                        SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable;
                        // DataGridView'deki masaların renklerini güncelle
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells["KulDurum"].Value != null)
                            {
                                bool kullanımDurumu = (bool)row.Cells["KulDurum"].Value;
                                if (kullanımDurumu)
                                {
                                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                                }
                                else
                                {
                                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if any row is selected
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen silmek için bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get the selected row
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Get the values from the selected row
                string masaNo = selectedRow.Cells["MasaNo"].Value.ToString();
                bool kulDurum = (bool)selectedRow.Cells["KulDurum"].Value;

                // If KulDurum is true, ask for confirmation
                if (kulDurum)
                {
                    DialogResult result = MessageBox.Show("Bu masa dolu. Silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.No)
                    {
                        return; // Cancel deletion
                    }
                }

                // Delete the selected row
                using (SqlConnection connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
                {
                    connection.Open();

                    string deleteQuery = "DELETE FROM Masa WHERE MasaNo = @MasaNo";
                    SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                    deleteCommand.Parameters.AddWithValue("@MasaNo", masaNo);
                    deleteCommand.ExecuteNonQuery();

                    MessageBox.Show("Satır başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh the DataGridView
                    string selectQuery = "SELECT MasaNo, SesDurum, KulDurum FROM Masa";
                    SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                    // DataGridView'deki masaların renklerini güncelle
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["KulDurum"].Value != null)
                        {
                            bool kullanımDurumu = (bool)row.Cells["KulDurum"].Value;
                            if (kullanımDurumu)
                            {
                                row.DefaultCellStyle.BackColor = Color.LightCoral;
                            }
                            else
                            {
                                row.DefaultCellStyle.BackColor = Color.LightGreen;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if any row is selected
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen güncellemek için bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get the selected row
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Get the values from the selected row
                string masaNo = selectedRow.Cells["MasaNo"].Value.ToString();
                bool sesDurum = (bool)selectedRow.Cells["SesDurum"].Value;
                bool kulDurum = (bool)selectedRow.Cells["KulDurum"].Value;

                // Modify the values as needed
                // For example, you can update sesDurum and kulDurum

                // Update the selected row
                using (SqlConnection connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=DbRafArasi1;Integrated Security=True"))
                {
                    connection.Open();

                    string updateQuery = "UPDATE Masa SET SesDurum = @SesDurum, KulDurum = @KulDurum WHERE MasaNo = @MasaNo";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@SesDurum", sesDurum);
                    updateCommand.Parameters.AddWithValue("@KulDurum", kulDurum);
                    updateCommand.Parameters.AddWithValue("@MasaNo", masaNo);
                    updateCommand.ExecuteNonQuery();

                    MessageBox.Show("Satır başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh the DataGridView
                    string selectQuery = "SELECT MasaNo, SesDurum, KulDurum FROM Masa";
                    SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                    // DataGridView'deki masaların renklerini güncelle
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["KulDurum"].Value != null)
                        {
                            bool kullanımDurumu = (bool)row.Cells["KulDurum"].Value;
                            if (kullanımDurumu)
                            {
                                row.DefaultCellStyle.BackColor = Color.LightCoral;
                            }
                            else
                            {
                                row.DefaultCellStyle.BackColor = Color.LightGreen;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
