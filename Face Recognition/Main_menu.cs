using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security;
using System.Security.Cryptography;


namespace protect_system
{
    public partial class Main_menu : Form
    {
        public Main_menu()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int error = 0;
            Form1 f1 = new Form1(ref error);
            if (error == 0)
            {
                f1.Show();
            }
        }

        public string fingerusername="";
        public string faceusername="";
        public bool access = false;

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int error = 0;
            Training_Form TF = new Training_Form(this, ref error);
            if (error == 0)
                TF.Show();
        }

        private void dfToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void dfToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            var fingerprint = new Fingeprint.FingerprintOption();
            fingerprint.Show();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            newuserform nuf = new newuserform();
            nuf.Show();          
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Fingeprint.recognitionfingerprint rff = new Fingeprint.recognitionfingerprint(this);
            rff.Show();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newuserform nuf = new newuserform();
            nuf.Show();
        }

        private void authenticateToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Main_menu_Activated(object sender, EventArgs e)
        {
            if (!access)
            {
                if (fingerusername != "")
                {
                    access = true;
                    getAccess(fingerusername);
                }
                else if (faceusername != "")
                {
                    access = true;
                    getAccess(faceusername);
                }
            }
            
        }

        private void getAccess(string username)
        {           
            //decode data in user's folder
            MessageBox.Show("Здравсвуйте, " + username);
            if (!Directory.Exists(Application.StartupPath + "/UsersData/" + username))
            {
                Directory.CreateDirectory(Application.StartupPath + "/UsersData/" + username);
            }
            else
            {
                decryptData(Application.StartupPath + "/UsersData/" + username);
            }
            System.Diagnostics.Process.Start(Application.StartupPath + "/UsersData/"+username);
        }

        private void encryptData(string folder)
        {
            //this is just for example reasons
            //dont store passwords in code
            string password = @"cl560Rc9g7ETrT18";
            string[] files = Directory.GetFiles(folder);
            foreach (string fileName in files)
            {
                string sub = fileName.Substring(fileName.Length-3);
                if (sub != "ENC")
                {
                    EncryptFile(fileName,fileName+"ENC",password,salt,iterations);
                    File.Delete(fileName);
                }
                //encryptFile()
                //File.Delete(fileName);
            }

            string[] subDirectories = Directory.GetDirectories(folder);
            foreach (string subDirectory in subDirectories)
            {
                encryptData(subDirectory);
            }
        }

        private void decryptData(string folder)
        {
            //this is just for example reasons
            //dont store passwords in code
            string password = @"cl560Rc9g7ETrT18";
            string[] files = Directory.GetFiles(folder);
            foreach (string fileName in files)
            {
                string sub = fileName.Substring(fileName.Length - 3);
                if (sub == "ENC")
                {
                    DecryptFile(fileName, fileName.Substring(0,fileName.Length-3), password,salt,iterations);
                    File.Delete(fileName);
                }
                //decryptFile(fileName);
                //File.Delete(fi)
            }

            string[] subDirectories = Directory.GetDirectories(folder);
            foreach (string subDirectory in subDirectories)
            {
                decryptData(subDirectory);
            }
        }

        private readonly byte[] salt = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; // Must be at least eight bytes.  MAKE THIS SALTIER!
        private const int iterations = 1042; // Recommendation is >= 1000.

        public void DecryptFile(string sourceFilename, string destinationFilename, string password, byte[] salt, int iterations)
        {
            AesManaged aes = new AesManaged();
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            // NB: Rfc2898DeriveBytes initialization and subsequent calls to   GetBytes   must be eactly the same, including order, on both the encryption and decryption sides.
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, iterations);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Mode = CipherMode.CBC;
            ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);

            using (FileStream destination = new FileStream(destinationFilename, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                using (CryptoStream cryptoStream = new CryptoStream(destination, transform, CryptoStreamMode.Write))
                {
                    try
                    {
                        using (FileStream source = new FileStream(sourceFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            source.CopyTo(cryptoStream);
                        }
                    }
                    catch (CryptographicException exception)
                    {
                        if (exception.Message == "Padding is invalid and cannot be removed.")
                            throw new ApplicationException("Universal Microsoft Cryptographic Exception (Not to be believed!)", exception);
                        else
                            throw;
                    }
                }
            }
        }

        /// <summary>Encrypt a file.</summary>
        /// <param name="sourceFilename">The full path and name of the file to be encrypted.</param>
        /// <param name="destinationFilename">The full path and name of the file to be output.</param>
        /// <param name="password">The password for the encryption.</param>
        /// <param name="salt">The salt to be applied to the password.</param>
        /// <param name="iterations">The number of iterations Rfc2898DeriveBytes should use before generating the key and initialization vector for the decryption.</param>
        public void EncryptFile(string sourceFilename, string destinationFilename, string password, byte[] salt, int iterations)
        {
            AesManaged aes = new AesManaged();
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            // NB: Rfc2898DeriveBytes initialization and subsequent calls to   GetBytes   must be eactly the same, including order, on both the encryption and decryption sides.
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, iterations);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Mode = CipherMode.CBC;
            ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);

            using (FileStream destination = new FileStream(destinationFilename, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                using (CryptoStream cryptoStream = new CryptoStream(destination, transform, CryptoStreamMode.Write))
                {
                    using (FileStream source = new FileStream(sourceFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        source.CopyTo(cryptoStream);
                    }
                }
            }
        }

        private void Main_menu_FormClosing(object sender, FormClosingEventArgs e)
        {           
            //if user is authenticated then encode his data in folder
            if (access)
            {
                string username;
                if (fingerusername != "")
                    username = fingerusername;
                else if (faceusername != "")
                    username = faceusername;
                else
                    return;
                if (Directory.Exists(Application.StartupPath + "/UsersData/" + username))
                {
                    encryptData(Application.StartupPath + "/UsersData/" + username);
                }              
            }
            
        }

    }
}
