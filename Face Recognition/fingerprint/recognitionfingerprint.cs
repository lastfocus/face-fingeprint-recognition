using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScanAPIHelper;
using System.Threading;
using System.Runtime.InteropServices;

using System.Security;
using System.Security.Cryptography;
using System.Data.SqlServerCe;
using System.IO;
using System.Drawing.Imaging;

using Recognition=SourceAFIS.Simple;

namespace Fingeprint
{
    public partial class recognitionfingerprint : Form
    {
        const int FTR_ERROR_EMPTY_FRAME = 4306; /* ERROR_EMPTY */
        const int FTR_ERROR_MOVABLE_FINGER = 0x20000001;
        const int FTR_ERROR_NO_FRAME = 0x20000002;
        const int FTR_ERROR_USER_CANCELED = 0x20000003;
        const int FTR_ERROR_HARDWARE_INCOMPATIBLE = 0x20000004;
        const int FTR_ERROR_FIRMWARE_INCOMPATIBLE = 0x20000005;
        const int FTR_ERROR_INVALID_AUTHORIZATION_CODE = 0x20000006;

        /* Other return codes are Windows-compatible */
        const int ERROR_NO_MORE_ITEMS = 259;                // ERROR_NO_MORE_ITEMS
        const int ERROR_NOT_ENOUGH_MEMORY = 8;              // ERROR_NOT_ENOUGH_MEMORY
        const int ERROR_NO_SYSTEM_RESOURCES = 1450;         // ERROR_NO_SYSTEM_RESOURCES
        const int ERROR_TIMEOUT = 1460;                     // ERROR_TIMEOUT
        const int ERROR_NOT_READY = 21;                     // ERROR_NOT_READY
        const int ERROR_BAD_CONFIGURATION = 1610;           // ERROR_BAD_CONFIGURATION
        const int ERROR_INVALID_PARAMETER = 87;             // ERROR_INVALID_PARAMETER
        const int ERROR_CALL_NOT_IMPLEMENTED = 120;         // ERROR_CALL_NOT_IMPLEMENTED
        const int ERROR_NOT_SUPPORTED = 50;                 // ERROR_NOT_SUPPORTED
        const int ERROR_WRITE_PROTECT = 19;                 // ERROR_WRITE_PROTECT
        const int ERROR_MESSAGE_EXCEEDS_MAX_SIZE = 4336;    // ERROR_MESSAGE_EXCEEDS_MAX_SIZE

        private Device m_hDevice;
        private bool m_bCancelOperation;
        private byte[] m_Frame;
        public string username;

        
Recognition.Person owner = new Recognition.Person();
Recognition.Person candidate = new Recognition.Person();
static Recognition.AfisEngine afis = new Recognition.AfisEngine();

List<Image> trainingImages = new List<Image>();//Imagesfingerprint
List<string> Names_List = new List<string>(); //labels
List<int> Names_List_ID = new List<int>(); //id        
Dictionary<string, Recognition.Person> fingerTemplates = new Dictionary<string, Recognition.Person>();

        protect_system.Main_menu mainform;
public bool loadData(string Folder_location)
{
if (Directory.Exists(Folder_location))
{
    string myconnectionString = "DataSource=" + Application.StartupPath + "/../../ProjectData.sdf;" + "Password=VuMyc2iFdP0TiDgI7n";
    try
    {
        Names_List.Clear();
        Names_List_ID.Clear();
        trainingImages.Clear();

        using (SqlCeConnection con = new SqlCeConnection(myconnectionString))
        {
            con.Open();
            using (SqlCeCommand com = new SqlCeCommand("SELECT * from fingerprints", con))
            {
                SqlCeDataReader reader = com.ExecuteReader();
                UnicodeEncoding UE = new UnicodeEncoding();
                while (reader.Read())
                {
                    long fingerprintId = reader.GetInt64(0);
                    string path = reader.GetString(1);
                    string username = reader.GetString(2);
                    string password = @"cl560Rc9g7ETrT18"; // key for decrypting file
                    byte[] key = UE.GetBytes(password);
                    FileStream fsCrypt = new FileStream(path, FileMode.Open);
                    RijndaelManaged RMCrypto = new RijndaelManaged();
                    RMCrypto.BlockSize *= 2;
                    CryptoStream cs = new CryptoStream(fsCrypt,
                        RMCrypto.CreateDecryptor(key, key),
                        CryptoStreamMode.Read);

                    Names_List_ID.Add(Names_List.Count);
                    Names_List.Add(username);
                    Bitmap image = new Bitmap(cs);
                    trainingImages.Add(image);
                    cs.Close();
                    fsCrypt.Close();
                }
            }
        }

        if (trainingImages.ToArray().Length != 0)
        {
            int len = trainingImages.ToArray().Length;
            Dictionary<string,bool> names = new Dictionary<string,bool>();
                        
            for(int i=0;i<len;i++)
            {
                if (!names.ContainsKey(Names_List[i]))
                {
                    names[Names_List[i].ToString()] = true;
                    Recognition.Person temp = new Recognition.Person();
                    Recognition.Fingerprint fingertemp = new Recognition.Fingerprint();
                    fingertemp.AsBitmap = (Bitmap)trainingImages[i];
                    temp.Fingerprints.Add(fingertemp);
                    temp.Id = i;
                    fingerTemplates.Add(Names_List[i],temp);
                }
                else
                {
                    Recognition.Fingerprint fingertemp = new Recognition.Fingerprint();
                    fingertemp.AsBitmap = (Bitmap)trainingImages[i];
                    fingerTemplates[Names_List[i]].Fingerprints.Add(fingertemp);
                }
            }

            foreach (var item in fingerTemplates)
            {
                afis.Extract(item.Value);
            }
                        
                        
                return true;
        }
        else
        {
            return false;
        }
    }
    catch (System.Exception ex)
    {
        return false;
    }
}
else
    return false;
}



        class ComboBoxItem
        {
            public ComboBoxItem(String value, int interfaceNumber)
            {
                m_String = value;
                m_InterfaceNumber = interfaceNumber;
            }

            public override string ToString()
            {
                return m_String;
            }

            public int interfaceNumber
            {
                get
                {
                    return m_InterfaceNumber;
                }
            }

            private String m_String;
            private int m_InterfaceNumber;
        }

        public recognitionfingerprint(protect_system.Main_menu mainform)
        {
            if (loadData(Application.StartupPath + "\\TrainedFingerprints") != true)
            {
                MessageBox.Show("Не удалось найти шаблоны для опознания.");
                return;
            }
            InitializeComponent();
            m_btnOpenDevice.Enabled = false;
            m_btnClose.Enabled = false;
            m_grpParameters.Enabled = false;
            m_grpTests.Enabled = false;
            m_hDevice = null;
            this.mainform = mainform; 
           
        }

       

        private void m_btnClose_Click(object sender, EventArgs e)
        {
            fingerprint = false;
            Size size = m_hDevice.ImageSize;
            m_hDevice.Dispose();
            m_hDevice = null;



            m_chkDetectFakeFinger.Checked = false;
            m_chkFFD.Checked = false;

            m_lblCompatibility.Text = String.Empty;
            m_lblImageSize.Text = String.Empty;

            m_lblCurrentImageSize.Text = String.Empty;
            m_lblEEPROMSize.Text = String.Empty;

            Graphics gr = m_picture.CreateGraphics();
            Bitmap hBitmap = CreateBitmap(gr.GetHdc(), size, null);
            hBitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
            m_picture.Image = hBitmap;

            m_grpParameters.Enabled = false;

            m_cmbInterfaces.Enabled = true;
            m_btnOpenDevice.Enabled = true;
            m_btnClose.Enabled = false;
            this.Close();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            try
            {
                int defaultInterface = ScanAPIHelper.Device.BaseInterface;
                FTRSCAN_INTERFACE_STATUS[] status = ScanAPIHelper.Device.GetInterfaces();
                for (int interfaceNumber = 0; interfaceNumber < status.Length; interfaceNumber++)
                {
                    if (status[interfaceNumber] == FTRSCAN_INTERFACE_STATUS.FTRSCAN_INTERFACE_STATUS_CONNECTED)
                    {
                        int index = m_cmbInterfaces.Items.Add(new ComboBoxItem(interfaceNumber.ToString(), interfaceNumber));
                        if (defaultInterface == interfaceNumber)
                        {
                            m_cmbInterfaces.SelectedIndex = index;
                        }
                    }
                }
            }
            catch (ScanAPIException ex)
            {
                ShowError(ex);
            }
        }

        private void ShowError(ScanAPIException ex)
        {
            String szMessage;
            switch (ex.ErrorCode)
            {
                case FTR_ERROR_EMPTY_FRAME:
                    szMessage = "Error code FTR_ERROR_EMPTY_FRAME";
                    break;

                case FTR_ERROR_MOVABLE_FINGER:
                    szMessage = "Error code FTR_ERROR_MOVABLE_FINGER";
                    break;

                case FTR_ERROR_NO_FRAME:
                    szMessage = "Error code FTR_ERROR_NO_FRAME";
                    break;

                case FTR_ERROR_USER_CANCELED:
                    szMessage = "Error code FTR_ERROR_USER_CANCELED";
                    break;

                case FTR_ERROR_HARDWARE_INCOMPATIBLE:
                    szMessage = "Error code FTR_ERROR_HARDWARE_INCOMPATIBLE";
                    break;

                case FTR_ERROR_FIRMWARE_INCOMPATIBLE:
                    szMessage = "Error code FTR_ERROR_FIRMWARE_INCOMPATIBLE";
                    break;

                case FTR_ERROR_INVALID_AUTHORIZATION_CODE:
                    szMessage = "Error code FTR_ERROR_INVALID_AUTHORIZATION_CODE";
                    break;

                case ERROR_NO_MORE_ITEMS:
                    szMessage = "Error code ERROR_NO_MORE_ITEMS";
                    break;

                case ERROR_NOT_ENOUGH_MEMORY:
                    szMessage = "Error code ERROR_NOT_ENOUGH_MEMORY";
                    break;

                case ERROR_NO_SYSTEM_RESOURCES:
                    szMessage = "Error code ERROR_NO_SYSTEM_RESOURCES";
                    break;

                case ERROR_TIMEOUT:
                    szMessage = "Error code ERROR_TIMEOUT";
                    break;

                case ERROR_NOT_READY:
                    szMessage = "Error code ERROR_NOT_READY";
                    break;

                case ERROR_BAD_CONFIGURATION:
                    szMessage = "Error code ERROR_BAD_CONFIGURATION";
                    break;

                case ERROR_INVALID_PARAMETER:
                    szMessage = "Error code ERROR_INVALID_PARAMETER";
                    break;

                case ERROR_CALL_NOT_IMPLEMENTED:
                    szMessage = "Error code ERROR_CALL_NOT_IMPLEMENTED";
                    break;

                case ERROR_NOT_SUPPORTED:
                    szMessage = "Error code ERROR_NOT_SUPPORTED";
                    break;

                case ERROR_WRITE_PROTECT:
                    szMessage = "Error code ERROR_WRITE_PROTECT";
                    break;

                case ERROR_MESSAGE_EXCEEDS_MAX_SIZE:
                    szMessage = "Error code ERROR_MESSAGE_EXCEEDS_MAX_SIZE";
                    break;

                default:
                    szMessage = String.Format("Error code: {0}", ex.ErrorCode);
                    break;
            }
            MessageBox.Show(szMessage);
        }


        private void OnUserBreak()
        {
            m_bCancelOperation = true;
        }

        bool fingerprint = true;

        private void OnTestGetFrame(object sender, EventArgs e)
        {
            //m_bCancelOperation = false;
            // m_lblMessage.Text = "Put finger to the scanner";           
            fingerprint = true;
            System.Threading.Thread thread = new System.Threading.Thread(GetFrame);
            thread.Start();
            System.Threading.Thread.Sleep(5);
            //GetFrame();
            //MessageDlg.HideMessageDlg();
            /*if (m_Frame != null && m_Frame.Length != 0)
            {
                Size size = m_hDevice.ImageSize;
                Graphics gr = m_picture.CreateGraphics();
                Bitmap hBitmap = CreateBitmap(gr.GetHdc(), size, m_Frame);
                hBitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                m_picture.Image = hBitmap;
            }*/
        }

        delegate void GetFrameCompleted();
        private void GetFrame()
        {
            int LastErrorCode = 0;
            bool bContinue = true;
            while (fingerprint)
            {
                do
                {
                    while (!m_hDevice.IsFingerPresent && !m_bCancelOperation && fingerprint)
                    {
                        if (LastErrorCode != m_hDevice.LastErrorCode)
                        {
                            LastErrorCode = m_hDevice.LastErrorCode;
                            /* switch (LastErrorCode)
                             {
                                 case 0:
                                     break;
                                 case FTR_ERROR_EMPTY_FRAME:
                                     MessageDlg.SetMessageDlg("Put finger to the scanner");
                                     break;
                                 case FTR_ERROR_MOVABLE_FINGER:
                                     MessageDlg.SetMessageDlg("There is no stable fingerprint image on the device.");
                                     break;
                                 case FTR_ERROR_NO_FRAME:
                                     MessageDlg.SetMessageDlg("Fake finger was detected");
                                     break;
                                 case FTR_ERROR_HARDWARE_INCOMPATIBLE:
                                 case FTR_ERROR_FIRMWARE_INCOMPATIBLE:
                                     MessageDlg.SetMessageDlg("The device does not support the requested feature");
                                     break;
                                 default:
                                     String msg = String.Format("Unknown error #{0}", LastErrorCode);
                                     MessageDlg.SetMessageDlg("msg");
                                     break;
                             }*/
                        }
                        Thread.Sleep(0);
                    }
                    if (!m_bCancelOperation)
                    {
                        try
                        {
                            m_Frame = m_hDevice.GetFrame();
                            bContinue = false;
                        }
                        catch (ScanAPIException)
                        {
                            if (!fingerprint)
                            {
                                return;
                            }
                            LastErrorCode = m_hDevice.LastErrorCode;
                            /*switch (LastErrorCode)
                            {
                                case 0:
                                    break;
                                case FTR_ERROR_EMPTY_FRAME:
                                    MessageDlg.SetMessageDlg("Put finger to the scanner");
                                    break;
                                case FTR_ERROR_MOVABLE_FINGER:
                                    MessageDlg.SetMessageDlg("There is no stable fingerprint image on the device.");
                                    break;
                                case FTR_ERROR_NO_FRAME:
                                    MessageDlg.SetMessageDlg("Fake finger was detected");
                                    break;
                                case FTR_ERROR_HARDWARE_INCOMPATIBLE:
                                case FTR_ERROR_FIRMWARE_INCOMPATIBLE:
                                    MessageDlg.SetMessageDlg("The device does not support the requested feature");
                                    break;
                                default:
                                    String msg = String.Format("Unknown error #{0}", LastErrorCode);
                                    MessageDlg.SetMessageDlg("msg");
                                    break;
                            }*/
                        }
                    }
                    else
                    {
                        break;
                    }
                } while (bContinue);

                if (m_Frame != null && m_Frame.Length != 0)
                {
                    Size size = m_hDevice.ImageSize;
                    Graphics gr = m_picture.CreateGraphics();
                    Bitmap hBitmap = CreateBitmap(gr.GetHdc(), size, m_Frame);
                    hBitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                    m_picture.Image = hBitmap;
                }
            }
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_hDevice != null)
            {
                m_hDevice.Dispose();
                m_hDevice = null;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateDIBitmap(IntPtr hdc, [In] ref BITMAPINFOHEADER lpbmih,
                                            uint fdwInit, byte[] lpbInit, byte[] lpbmi,
                                            uint fuUsage);

        /* constants for CreateDIBitmap */
        const int CBM_INIT = 0x04;   /* initialize bitmap */

        /* DIB color table identifiers */

        const int DIB_RGB_COLORS = 0; /* color table in RGBs */
        const int DIB_PAL_COLORS = 1; /* color table in palette indices */

        const int BI_RGB = 0;
        const int BI_RLE8 = 1;
        const int BI_RLE4 = 2;
        const int BI_BITFIELDS = 3;
        const int BI_JPEG = 4;
        const int BI_PNG = 5;

        private Bitmap CreateBitmap(IntPtr hDC, Size bmpSize, byte[] data)
        {
            System.IO.MemoryStream mem = new System.IO.MemoryStream();
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(mem);
            //BITMAPINFO bmpInfo = new BITMAPINFO();
            BITMAPINFOHEADER bmiHeader = new BITMAPINFOHEADER();
            bmiHeader.biSize = 40;
            bmiHeader.biWidth = bmpSize.Width;
            bmiHeader.biHeight = bmpSize.Height;
            bmiHeader.biPlanes = 1;
            bmiHeader.biBitCount = 8;
            bmiHeader.biCompression = BI_RGB;
            bw.Write(bmiHeader.biSize);
            bw.Write(bmiHeader.biWidth);
            bw.Write(bmiHeader.biHeight);
            bw.Write(bmiHeader.biPlanes);
            bw.Write(bmiHeader.biBitCount);
            bw.Write(bmiHeader.biCompression);
            bw.Write(bmiHeader.biSizeImage);
            bw.Write(bmiHeader.biXPelsPerMeter);
            bw.Write(bmiHeader.biYPelsPerMeter);
            bw.Write(bmiHeader.biClrUsed);
            bw.Write(bmiHeader.biClrImportant);

            for (int i = 0; i < 256; i++)
            {
                bw.Write((byte)i);
                bw.Write((byte)i);
                bw.Write((byte)i);
                bw.Write((byte)0);
            }

            IntPtr hBitmap;
            if (data != null)
            {
                hBitmap = CreateDIBitmap(hDC, ref bmiHeader, CBM_INIT, data, mem.ToArray(), DIB_RGB_COLORS);
            }
            else
            {
                hBitmap = CreateDIBitmap(hDC, ref bmiHeader, 0, null, mem.ToArray(), DIB_RGB_COLORS);
            }
            return Bitmap.FromHbitmap(hBitmap);
        }

        /* private void button1_Click(object sender, EventArgs e)
         {
             OpenFileDialog opf = new OpenFileDialog();
             opf.InitialDirectory = Application.StartupPath;
             opf.Filter = "image files. | *.bmp";
             if (opf.ShowDialog() == DialogResult.OK)
             {
                 pictureBox1.Image = Image.FromFile(opf.FileName);
             }
         }*/

        private void m_btnOpenDevice_Click(object sender, EventArgs e)
        {
            try
            {
                m_hDevice = new Device();
                m_hDevice.Open();

                // gets devce parameters
                VersionInfo version = m_hDevice.VersionInformation;


                m_chkDetectFakeFinger.Checked = m_hDevice.DetectFakeFinger;
                m_chkFFD.Checked = m_hDevice.FastFingerDetectMethod;
                m_chkReceiveLongImage.Checked = m_hDevice.ReceiveLongImage;

                DeviceInfo dinfo = m_hDevice.Information;
                if (dinfo.DeviceCompatibility == 0)
                {
                    m_lblCompatibility.Text = "USB 1.1 device";
                }
                else if (dinfo.DeviceCompatibility == 1)
                {
                    m_lblCompatibility.Text = "USB 2.0 device";
                }
                else
                {
                    m_lblCompatibility.Text = "USB 2.0 device";
                }
                m_lblImageSize.Text = dinfo.imageSize.ToString();

                m_lblCurrentImageSize.Text = m_hDevice.ImageSize.ToString();
                m_lblEEPROMSize.Text = m_hDevice.MemorySize.ToString(CultureInfo.InvariantCulture.NumberFormat);

                m_grpParameters.Enabled = true;
                m_grpTests.Enabled = true;

                m_cmbInterfaces.Enabled = false;
                m_btnOpenDevice.Enabled = false;
                m_btnClose.Enabled = true;
            }
            catch (ScanAPIException ex)
            {
                if (m_hDevice != null)
                {
                    m_hDevice.Dispose();
                    m_hDevice = null;
                }
                ShowError(ex);
            }

        }

        private void m_btnGetFrame_Click(object sender, EventArgs e)
        {
            SaveFrame.Enabled = true;
            fingerprint = true;
            m_btnGetFrame.Enabled = false;
            m_btnCancel.Enabled = true;
            System.Threading.Thread thread = new System.Threading.Thread(GetFrame);
            thread.Start();
            System.Threading.Thread.Sleep(5);
        }



        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            fingerprint = false;
            SaveFrame.Enabled = false;
        }

        private void m_btnClose_Click_1(object sender, EventArgs e)
        {

            m_btnCancel_Click(sender, e);
            Size size = m_hDevice.ImageSize;
            m_hDevice.Dispose();
            m_hDevice = null;



            m_chkDetectFakeFinger.Checked = false;
            m_chkFFD.Checked = false;
            m_chkReceiveLongImage.Checked = false;

            m_lblCompatibility.Text = String.Empty;
            m_lblImageSize.Text = String.Empty;

            m_lblCurrentImageSize.Text = String.Empty;
            m_lblEEPROMSize.Text = String.Empty;

            Graphics gr = m_picture.CreateGraphics();
            Bitmap hBitmap = CreateBitmap(gr.GetHdc(), size, null);
            hBitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
            m_picture.Image = hBitmap;

            m_grpParameters.Enabled = false;
            m_grpTests.Enabled = false;

            m_cmbInterfaces.Enabled = true;
            m_btnOpenDevice.Enabled = true;
            m_btnClose.Enabled = false;            
            this.Close();
        }

        private void m_cmbInterfaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_cmbInterfaces.SelectedIndex != -1)
            {
                ComboBoxItem item = (ComboBoxItem)m_cmbInterfaces.Items[m_cmbInterfaces.SelectedIndex];
                try
                {
                    ScanAPIHelper.Device.BaseInterface = item.interfaceNumber;
                    m_btnOpenDevice.Enabled = true;
                }
                catch (ScanAPIException ex)
                {
                    ShowError(ex);
                }
            }
            else
            {
                m_btnOpenDevice.Enabled = false;
            }
        }

private void SaveFrame_Click(object sender, EventArgs e)
{
    Recognition.Fingerprint fp1 = new Recognition.Fingerprint();
    fp1.AsBitmap = ((Bitmap)m_picture.Image);
    candidate.Fingerprints.Add(fp1);
    afis.Extract(candidate);

    var user = afis.Identify(candidate, fingerTemplates.Values).FirstOrDefault();
    if (user == null)
    {
        MessageBox.Show("Пользователь не опознан");
    }
    else
    {
        MessageBox.Show("Здравствуйте, "+Names_List[user.Id]);
        mainform.fingerusername = Names_List[user.Id];
    }
            
}

        private bool save_training_data(Image fingerprint_data)
        {
            string conString = protect_system.Properties.Settings.Default.ProjectDataConnectionString;
            //EncryptFile(Application.StartupPath + "/TrainedFaces/" + "face_PERSON1_408062635.jpg");
            //DecryptFile(Application.StartupPath + "/TrainedFaces/" + "face_PERSON1_408062635.jpgENC");
            //return true;
            try
            {
                Random rand = new Random();
                bool file_create = true;
                string fingerprintname = "fingerprint_" + this.username + "_" + rand.Next().ToString() + ".bmp";
                while (file_create)
                {

                    if (!File.Exists(Application.StartupPath + "/TrainedFaces/" + fingerprintname))
                    {
                        file_create = false;
                    }
                    else
                    {
                        fingerprintname = "fingerprint_" + this.username + "_" + rand.Next().ToString() + ".bmp";
                    }
                }


                if (Directory.Exists(Application.StartupPath + "/TrainedFingeprints/"))
                {
                    fingerprint_data.Save(Application.StartupPath + "/TrainedFingerprints/" + fingerprintname, System.Drawing.Imaging.ImageFormat.Bmp);
                }
                else
                {
                    Directory.CreateDirectory(Application.StartupPath + "/TrainedFingerprints/");
                    fingerprint_data.Save(Application.StartupPath + "/TrainedFingerprints/" + fingerprintname, System.Drawing.Imaging.ImageFormat.Bmp);
                }



                EncryptFile(Application.StartupPath + "/TrainedFingerprints/" + fingerprintname);
                string myconnectionString = "DataSource=" + Application.StartupPath + "/../../ProjectData.sdf;" + "Password=VuMyc2iFdP0TiDgI7n";
                //using (SqlCeConnection con = new SqlCeConnection(conString))
                using (SqlCeConnection con = new SqlCeConnection(myconnectionString))
                {
                    con.Open();
                    var Rowid = new SqlCeCommand("SELECT TOP(1) FingerprintId FROM fingerprints ORDER BY FingerprintId DESC", con).ExecuteScalar();
                    using (SqlCeCommand cmd = new SqlCeCommand("INSERT INTO fingerprints VALUES (@FingerprintId, @Data, @User)", con))
                    {
                        int Newid = Convert.ToInt32(Rowid) + 1;
                        cmd.Parameters.AddWithValue("@FingerprintId", Newid);
                        cmd.Parameters.AddWithValue("@Data", Application.StartupPath + "/TrainedFingerprints/" + fingerprintname + "ENC");
                        cmd.Parameters.AddWithValue("@User", username);
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void EncryptFile(string inputFile)
        {
            try
            {
                string password = @"cl560Rc9g7ETrT18"; // Your Key Here
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = inputFile + "ENC";
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();
                RMCrypto.BlockSize *= 2;
                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch
            {
                MessageBox.Show("Encryption failed!", "Error");
            }
        }

        private void DecryptFile(string inputFile)
        {

            {
                string password = @"cl560Rc9g7ETrT18"; // Your Key Here

                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

                RijndaelManaged RMCrypto = new RijndaelManaged();
                RMCrypto.BlockSize *= 2;
                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateDecryptor(key, key),
                    CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(inputFile.Substring(0, inputFile.Length - 3), FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();

            }
        }
    }
}
