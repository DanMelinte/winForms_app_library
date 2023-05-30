using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SQLApplication
{
    public partial class Form4 : Form
    {
        private Image resizedPhoto(Image photo)
        {
            double widthRatio = (double)photo.Width / pictureBox1.Width;
            double heightRatio = (double)photo.Height / pictureBox1.Height;
            double ratio = Math.Max(widthRatio, heightRatio);
            int newWidth = (int)(photo.Width / ratio);
            int newHeight = (int)(photo.Height / ratio);

            Image resizedPhoto = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(resizedPhoto))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(photo, 0, 0, newWidth, newHeight);
            }

            return resizedPhoto;
        }
        public string Id { get; set; }

        private string connect = "Server=127.0.0.1;Database=Biblioteca;Uid=root;Pwd=2595;";
        private DataTable dataTable1;
        private DataTable dataTable2;

        public Form4(string id)
        {
            InitializeComponent();
            Id = id;

            MySqlConnection connection = new MySqlConnection(connect);
            connection.Open();

            string query = "SELECT * FROM Client WHERE ID_Detinator = @id";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        textBox1.Text = reader["Nume"].ToString();
                        textBox2.Text = reader["Prenume"].ToString();
                        textBox3.Text = reader["CNP"].ToString();

                        if (reader["Sex"] is int sexValue)
                        {
                            textBox4.Text = sexValue == 1 ? "Masculin" : "Feminin";
                        }

                        dateTimePicker1.Text = reader["Data_Nasterii"].ToString();

                        if (reader["Photo"] != null && reader["Photo"] != DBNull.Value)
                        {
                            byte[] imageBytes = null;
                            imageBytes = (byte[])reader["Photo"];
                            Image image;
                            using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                            {
                                image = Image.FromStream(memoryStream);
                                image = resizedPhoto(image);
                            }
                            pictureBox1.Image = image;
                        }

                    }
                }
            }

            initilizeTables();
        }

        private DataTable GetData(string query)
        {
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        public void initilizeTables()
        {
            string query1 = "SELECT * FROM Carte WHERE ID_Detinator IS NULL";
            dataTable1 = GetData(query1);

            string query2 = $"SELECT * FROM Carte WHERE ID_Detinator = '{Id}'";
            dataTable2 = GetData(query2);

            dataGridView1.DataSource = dataTable1;
            dataGridView2.DataSource = dataTable2;

            dataGridView1.CellClick += DataGridView1_CellClick;
            dataGridView2.CellClick += DataGridView2_CellClick;
        }


        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataRow selectedRow = dataTable1.Rows[e.RowIndex];
                DataRow newRow = dataTable2.NewRow();

                newRow.ItemArray = selectedRow.ItemArray;
                newRow["ID_Detinator"] = int.Parse(Id); 

                dataTable2.Rows.Add(newRow);
                dataTable1.Rows.Remove(selectedRow);


                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();
                    string query = $"UPDATE Carte SET ID_Detinator = '{Id}' WHERE ID_Carte = '{newRow["ID_Carte"]}'";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        private void DataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataRow selectedRow = dataTable2.Rows[e.RowIndex];
                DataRow newRow = dataTable1.NewRow();

                newRow.ItemArray = selectedRow.ItemArray;
                newRow["ID_Detinator"] = DBNull.Value; 

                dataTable1.Rows.Add(newRow);
                dataTable2.Rows.Remove(selectedRow);

                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();
                    string query = "UPDATE Carte SET ID_Detinator = @ID_Detinator WHERE ID_Carte = @ID_Carte";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ID_Detinator", DBNull.Value);
                    command.Parameters.AddWithValue("@ID_Carte", newRow["ID_Carte"]);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();

            Form form2 = new Form2();
            form2.ShowDialog();
        }
    }
}
