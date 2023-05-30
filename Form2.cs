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
using System.IO;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections;



namespace SQLApplication
{
    public partial class Form2 : Form
    {

        //public string connect = @"Data Source=DM;Initial Catalog=Biblioteca;Integrated Security=True";
        public string connect = "Server=127.0.0.1;Database=Biblioteca;Uid=root;Pwd=2595;";

        public void initilizeTables()
        {
            string tabel_date1 = "select * from Carte";
            MySqlDataAdapter da1 = new MySqlDataAdapter(tabel_date1, connect);
            DataSet carti = new DataSet();
            da1.Fill(carti, "Carte");
            dataGridView1.DataSource = carti.Tables["Carte"].DefaultView;

            string tabel_date2 = "select * from Client";
            MySqlDataAdapter da2 = new MySqlDataAdapter(tabel_date2, connect);
            DataSet clienti = new DataSet();
            da2.Fill(clienti, "Client");
            dataGridView2.DataSource = clienti.Tables["Client"].DefaultView;

            dataGridView1.CellClick += DataGridViewCellClick1;
            dataGridView2.CellClick += DataGridViewCellClick2;

            da1.Dispose();
            da2.Dispose();
        }


        public Form2()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            MySqlConnection cnn = new MySqlConnection(connect);
            cnn.Open();

            initilizeTables();

            cnn.Close();
        }

        private DataGridViewRow selectedRow;

        private void DataGridViewCellClick1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedRow = dataGridView1.Rows[e.RowIndex];

                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                string id = row.Cells["ID_Detinator"].Value.ToString();

                string query = "SELECT Client.* " +
                               "FROM Client " +
                               "JOIN Carte ON Client.ID_Detinator = Carte.ID_Detinator " +
                               $"WHERE Carte.ID_Detinator = '{id}' " +
                               "LIMIT 1";


                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();
                    MySqlDataAdapter da = new MySqlDataAdapter(query, connect);
                    DataSet clientFilter = new DataSet();
                    da.Fill(clientFilter, "Client");
                    dataGridView2.DataSource = clientFilter.Tables["Client"].DefaultView;
                    connection.Close();
                }
            }
        }

        private void DataGridViewCellClick2(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {

                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];

                string id = row.Cells["ID_Detinator"].Value.ToString();

                button2.Text = "new Book for Client " + id;
                button2.Click += button2_Click;

                string query = "SELECT Carte.* " +
                               "FROM Carte " +
                               "JOIN Client ON Carte.ID_Detinator = Client.ID_Detinator " +
                               $"WHERE Client.ID_Detinator = '{id}'";

                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();
                    MySqlDataAdapter da = new MySqlDataAdapter(query, connect);
                    DataSet cartiFilter = new DataSet();
                    da.Fill(cartiFilter, "Carte");
                    dataGridView1.DataSource = cartiFilter.Tables["Carte"].DefaultView;
                    connection.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form3 = new Form3();
            form3.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            initilizeTables();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataGridViewCell selectedCell = dataGridView2.SelectedCells[0];
            string id = selectedCell.OwningRow.Cells["ID_Detinator"].Value.ToString();

            Form form4 = new Form4(id);
            form4.ShowDialog();
            this.Hide();
        }

        private void CautaNumeBTN_Click(object sender, EventArgs e)
        {

            string searchText = textBox1.Text.Trim();

            DataView dataView1 = dataGridView1.DataSource as DataView;
            if (dataView1 != null)
            {
                dataView1.RowFilter = $"Titlu LIKE '%{searchText}%' OR Autor LIKE '%{searchText}%'";
            }

            DataView dataView2 = dataGridView2.DataSource as DataView;
            if (dataView2 != null)
            {
                dataView2.RowFilter = $"Nume LIKE '%{searchText}%' OR Prenume LIKE '%{searchText}%'";
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            if (selectedRow != null)
            {
                string id = selectedRow.Cells["ID_Carte"].Value.ToString();
                DeleteRowFromTable(id);
                dataGridView1.Rows.Remove(selectedRow);
                selectedRow = null; 
            }
        }

        private void DeleteRowFromTable(string id)
        {
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();
                string query = $"DELETE FROM Carte WHERE ID_Carte = @id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
