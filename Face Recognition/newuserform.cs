using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Data.SqlServerCe;

namespace protect_system
{

    public partial class newuserform : Form
    {
        [DllImport("advapi32.dll")]
        static extern bool EncryptFile(string filename);

        public newuserform()
        {
            InitializeComponent();            
        }

       
        private void button1_Click(object sender, EventArgs e)
        {
            //add face data
            int error = 0;
            Training_Form tr1 = new Training_Form(this, ref error,textBox1.Text);
            if (error == 0)
                tr1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //add fingerprint data
            var addFingerprintForm = new Fingeprint.addfingerprint(textBox1.Text);
            addFingerprintForm.Show();
        }


        private void hover_button(object sender, EventArgs e)
        {
            Button thisButton = sender as Button;
            thisButton.BackColor = Color.FromArgb(255, 88, 197, 234);
        }

        private void leave_button(object sender, EventArgs e)
        {
            Button thisButton = sender as Button;
            thisButton.UseVisualStyleBackColor = true;

        }

        private void newuserform_Load(object sender, EventArgs e)
        {
            textBox1.Text = "Введите свое имя";
            textBox1.ForeColor = System.Drawing.Color.Gray;
            string myconnectionString = "DataSource=" + Application.StartupPath + "/../../ProjectData.sdf;" + "Password=VuMyc2iFdP0TiDgI7n";
            using (SqlCeConnection con = new SqlCeConnection(myconnectionString))
            {
                con.Open();
                using (SqlCeCommand com = new SqlCeCommand("SELECT * from users", con))
                {
                    SqlCeDataReader reader = com.ExecuteReader();
                    UnicodeEncoding UE = new UnicodeEncoding();
                    while (reader.Read())
                    {
                        long userid = reader.GetInt64(0);                        
                        string username = reader.GetString(1);
                        listView1.Items.Add(username);
                    }
                }
            }

            foreach (var button in Controls.OfType<Button>())
            {
                button.MouseHover += hover_button;
                button.MouseLeave += leave_button;
            }


        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            //if (textBox1.Modified)
            {
                textBox1.Clear();
                textBox1.ForeColor = SystemColors.WindowText;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            //this is just for example reasons
            //take password from local params, dont store them in code
            string myconnectionString = "DataSource=" + Application.StartupPath + "/../../ProjectData.sdf;" + "Password=VuMyc2iFdP0TiDgI7n";
            using (SqlCeConnection con = new SqlCeConnection(myconnectionString))
            {
                con.Open();
                var Rowid = new SqlCeCommand("SELECT TOP(1) Id FROM users ORDER BY Id DESC", con).ExecuteScalar();
                using (SqlCeCommand cmd = new SqlCeCommand("INSERT INTO users VALUES (@Id, @Name,@status)", con))
                {
                    int Newid = Convert.ToInt32(Rowid) + 1;
                    cmd.Parameters.AddWithValue("@Id", Newid);
                    cmd.Parameters.AddWithValue("@Name", username);
                    cmd.Parameters.AddWithValue("@status", "0");
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
          
            {
                if (!Directory.Exists(Application.StartupPath + "/UsersData/"))
                {
                    Directory.CreateDirectory(Application.StartupPath + "/UsersData/");
                }
                Directory.CreateDirectory(Application.StartupPath + "/UsersData/" + username);
                Process.Start(Application.StartupPath + "/UsersData/");
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

      



    }
}
