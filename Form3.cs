using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using MySql.Data.MySqlClient;

namespace SQLApplication
{
    public partial class Form3 : Form
    {
        private Image resizedPhoto(Image photo)
        {
            panel1.Visible = false;

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


        private void panel1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Image Files (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string photoPath = openFileDialog.FileName;
                Image photo = Image.FromFile(photoPath);

                pictureBox1.Image = resizedPhoto(photo);
                pictureBox1.Tag = photoPath;

                photo.Dispose();
            }
        }

        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);

            Image photo = Image.FromFile(file[0]);
            pictureBox1.Tag = file[0];

            pictureBox1.Image = resizedPhoto(photo);

            photo.Dispose();
        }


        public Form3()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            comboBox1.Items.Add("Masculin");
            comboBox1.Items.Add("Feminin");
        }

        private bool verifyCNP()
        {
            string cnp = textBox3.Text;
            int s, a1, a2, l1, l2, z1, z2, j1, j2, n1, n2, n3, cifc, u;

            if (cnp.Trim().Length != 13)
            {
                label5.Text = "Invalid cnp";
                return false;
            }
            else
            {
                s = Convert.ToInt16(cnp.Substring(0, 1));
                a1 = Convert.ToInt16(cnp.Substring(1, 1));
                a2 = Convert.ToInt16(cnp.Substring(2, 1));
                l1 = Convert.ToInt16(cnp.Substring(3, 1));
                l2 = Convert.ToInt16(cnp.Substring(4, 1));
                z1 = Convert.ToInt16(cnp.Substring(5, 1));
                z2 = Convert.ToInt16(cnp.Substring(6, 1));
                j1 = Convert.ToInt16(cnp.Substring(7, 1));
                j2 = Convert.ToInt16(cnp.Substring(8, 1));
                n1 = Convert.ToInt16(cnp.Substring(9, 1));
                n2 = Convert.ToInt16(cnp.Substring(10, 1));
                n3 = Convert.ToInt16(cnp.Substring(11, 1));

                cifc = Convert.ToInt16(((s * 2 + a1 * 7 + a2 * 9 + l1 * 1 + l2 * 4 + z1 * 6 + z2 * 3 + j1 * 5 + j2 * 8 + n1 * 2 + n2 * 7 + n3 * 9) % 11));
                if (cifc == 10)
                {
                    cifc = 1;
                }
                u = Convert.ToInt16(cnp.Substring(12, 1));
                if (cifc == u)
                {
                    label5.Text = "";
                    return true;
                }
                else
                {
                    label5.Text = "* invalid cnp";
                    return false;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                if (verifyCNP() == true)
                {
                    label5.Text = "";
                    string connect = "Server=127.0.0.1;Database=Biblioteca;Uid=root;Pwd=2595;";

                    MySqlConnection cnn = new MySqlConnection(connect);
                    cnn.Open();

                    string query = "INSERT INTO Client (Nume, Prenume, CNP, Sex, Data_Nasterii, Photo) VALUES (@n, @p, @cnp, @s, @dn, @ph)";
                    MySqlCommand command = new MySqlCommand(query, cnn);

                    command.Parameters.Add("@n", MySqlDbType.VarChar).Value = textBox1.Text;
                    command.Parameters.Add("@p", MySqlDbType.VarChar).Value = textBox2.Text;
                    command.Parameters.Add("@cnp", MySqlDbType.VarChar).Value = textBox3.Text;


                    if (comboBox1.SelectedIndex == 0)
                        command.Parameters.Add("@s", MySqlDbType.VarChar).Value = 1;
                    else if (comboBox1.SelectedIndex == 1)
                        command.Parameters.Add("@s", MySqlDbType.VarChar).Value = 0;
                    else
                        command.Parameters.Add("@s", MySqlDbType.VarChar).Value = null;

                    command.Parameters.Add("@dn", MySqlDbType.VarChar).Value = dateTimePicker1.Value.ToShortDateString();

                    if (pictureBox1.Image != null)
                    {
                        string FileName = pictureBox1.Tag.ToString();

                        byte[] ImageData;
                        FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                        BinaryReader br = new BinaryReader(fs);
                        ImageData = br.ReadBytes((int)fs.Length);

                        command.Parameters.Add("@ph", MySqlDbType.MediumBlob).Value = ImageData;
                    }
                    else
                        command.Parameters.Add("@ph", MySqlDbType.MediumBlob).Value = null;

                    command.ExecuteNonQuery();
                    cnn.Close();

                    Button btn1 = new Button();
                    btn1.Font = new Font("Palatino Linotype", 12, FontStyle.Bold);
                    btn1.FlatStyle = FlatStyle.Flat;
                    btn1.Name = "btn1";
                    btn1.Text = "ok";
                    btn1.Size = new Size(20, 20);
                    btn1.Click += Btn1_Click;
                    flowLayoutPanel1.Controls.Add(btn1);
                }
            }
            else
            {
                label5.Text = "* inclomplet data";
            }            
        }

        private void Btn1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }
    }
}
