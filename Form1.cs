using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace SQLApplication
{
    public partial class Form1 : Form
    {
        public class Utilizator
        {
            string name;
            string pass;

            public Utilizator(string name, string pass)
            {
                this.name = name;
                this.pass = pass;
            }
             
            public string getPass() { return pass; } 
        }

        List<Utilizator> usrs = new List<Utilizator>();

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            comboBox1.Items.Clear();

            XmlDocument doc = new XmlDocument();
            doc.Load("Autentificare.xml");
            List<string> names = new List<string>();
            List<string> passs = new List<string>();

            XmlNodeList name = doc.SelectNodes("Data/Utilizator/nume");
            XmlNodeList pass = doc.SelectNodes("Data/Utilizator/parola");

            foreach (XmlNode node in name)
            {
                comboBox1.Items.Add(node.InnerText);
                names.Add(node.InnerText);
            }

            foreach (XmlNode node in pass)
            {
                passs.Add(node.InnerText);
            }

            for (int i = 0; i < names.Count; i++)
            {
                Utilizator us = new Utilizator(names[i], passs[i]);
                usrs.Add(us);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            label3.Text = "";
        }

        int verifyPass()
        {
            if (textBox1.Text == usrs.ElementAt(comboBox1.SelectedIndex).getPass())
                return 1;
            else
                return 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "")
            {
                label3.Text = "* select a user";
            }
            else if (verifyPass() == 1)
            {
                Form2 form2 = new Form2();
                this.Hide();
                form2.ShowDialog();
            }
            else
                label3.Text = "* incorrect password";
                

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
