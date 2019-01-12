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

namespace Fingeprint
{
    public partial class FingerprintOption : Form
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

        public FingerprintOption()
        {
            InitializeComponent();
            m_btnOpenDevice.Enabled = false;
            m_btnClose.Enabled = false;
            m_grpParameters.Enabled = false;            
            m_hDevice = null;
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

                m_cmbInterfaces.Enabled = false;
                m_btnOpenDevice.Enabled = false;
                m_btnClose.Enabled = true;
                OnTestGetFrame(sender, e);
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

        private void FingerprintOption_Load(object sender, EventArgs e)
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
       

        private void OnDetectFakeFinger(object sender, EventArgs e)
        {
            if (m_hDevice != null)
            {
                m_hDevice.DetectFakeFinger = m_chkDetectFakeFinger.Checked;
            }
        }

        private void OnFFD(object sender, EventArgs e)
        {
            if (m_hDevice != null)
            {
                m_hDevice.FastFingerDetectMethod = m_chkFFD.Checked;
            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.InitialDirectory = Application.StartupPath;
            opf.Filter = "image files. | *.bmp";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(opf.FileName);
            }
        }
    }
}
