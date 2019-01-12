using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Emgu.CV.UI;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

using System.IO;
using System.Drawing.Imaging;
using System.Xml;
using System.Threading;

using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

using System.Data.SqlServerCe;
//using System.Text.RegularExpressions;

//using Face_Recognition;

namespace protect_system
{
    public partial class Training_Form : Form
    {
        #region Variables
        //Camera specific
        Capture grabber;

        //Images for finding face
        Image<Bgr, Byte> currentFrame;
        Image<Gray, byte> result = null;
        Image<Gray, byte> gray_frame = null;

        //Classifier
        //CascadeClassifier Face = new CascadeClassifier(Application.StartupPath + "/Cascades/haarcascade_frontalface_default.xml");//Our face detection method 
        CascadeClassifier Face;

        //For aquiring 10 images in a row
        List<Image<Gray, byte>> resultImages = new List<Image<Gray, byte>>();
        int results_list_pos = 0;
        int num_faces_to_aquire = 10;
        bool RECORD = false;

        //Saving Jpg
        List<Image<Gray, byte>> ImagestoWrite = new List<Image<Gray, byte>>();
        EncoderParameters ENC_Parameters = new EncoderParameters(1);
        EncoderParameter ENC = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100);
        ImageCodecInfo Image_Encoder_JPG;

        //Saving XAML Data file
        List<string> NamestoWrite = new List<string>();
        List<string> NamesforFile = new List<string>();
        XmlDocument docu = new XmlDocument();

        //Variables
        Form1 Parent;
        Main_menu Parent2;
        newuserform  Parent3;
        #endregion

        public Training_Form(Form1 _Parent, ref int error)
        {
            InitializeComponent();
            Parent = _Parent;
            Face = Parent.Face;
            //Face = new HaarCascade(Application.StartupPath + "/Cascades/haarcascade_frontalface_alt2.xml");
            ENC_Parameters.Param[0] = ENC;
            Image_Encoder_JPG = GetEncoder(ImageFormat.Jpeg);
            try
            {
                initialise_capture();
                error = 0;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Не удалось подключить камеру");
                this.Close();
                error = 1;
            }
            
        }

        public Training_Form(Main_menu _Parent, ref int error)
        {
            InitializeComponent();
            Parent2 = _Parent;
            //Face = Parent.Face;
            //Face = new HaarCascade(Application.StartupPath + "/Cascades/haarcascade_frontalface_alt2.xml");
            Face = new CascadeClassifier(Application.StartupPath + "/Cascades/haarcascade_frontalface_default.xml");//Our face detection method 
            ENC_Parameters.Param[0] = ENC;
            Image_Encoder_JPG = GetEncoder(ImageFormat.Jpeg);
            try
            {
                initialise_capture();
                error = 0;
            }
            catch
            {
                MessageBox.Show("Не удалось подключить камеру");
                this.Close();
                error = 1;
            }
        }

public Training_Form(newuserform _Parent, ref int error,string name)
{
    InitializeComponent();
    Parent3 = _Parent;
    //Face = Parent.Face;
    //Face = new HaarCascade(Application.StartupPath + "/Cascades/haarcascade_frontalface_alt2.xml");
    Face = new CascadeClassifier(Application.StartupPath + "/Cascades/haarcascade_frontalface_default.xml");//Our face detection method 
    ENC_Parameters.Param[0] = ENC;
    NAME_PERSON.Text = name;
    Image_Encoder_JPG = GetEncoder(ImageFormat.Jpeg);
    try
    {
        initialise_capture();
        error = 0;
    }
    catch
    {
        MessageBox.Show("Не удалось подключить камеру");
        this.Close();
        error = 1;
    }
            
}


        private void Training_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            stop_capture();

            //what crate method we use
            if (Parent != null) //from menu form
            {
                Parent.retrain();
                Parent.initialise_capture();
            }            
        }



        //Camera Start Stop
public void initialise_capture()
{
    grabber = new Capture();
    grabber.QueryFrame();
    //Initialize the FrameGraber event
    Application.Idle += new EventHandler(FrameGrabber);
}
private void stop_capture()
{
    Application.Idle -= new EventHandler(FrameGrabber);
    if (grabber != null)
    {
        grabber.Dispose();
    }
    //Initialize the FrameGraber event
}

        //Process Frame
void FrameGrabber(object sender, EventArgs e)
{
    //Get the current frame form capture device
    currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

    //Convert it to Grayscale
    if (currentFrame != null)
    {
        gray_frame = currentFrame.Convert<Gray, Byte>();                
        Rectangle[] facesDetected = Face.DetectMultiScale(gray_frame, 1.2, 10, new Size(50, 50), Size.Empty);

        //Action for each element detected
        for(int i = 0; i< facesDetected.Length; i++)// (Rectangle face_found in facesDetected)
        {
            //This will focus in on the face from the haar results its not perfect but it will remove a majoriy
            //of the background noise
            facesDetected[i].X += (int)(facesDetected[i].Height * 0.15);
            facesDetected[i].Y += (int)(facesDetected[i].Width * 0.22);
            facesDetected[i].Height -= (int)(facesDetected[i].Height * 0.3);
            facesDetected[i].Width -= (int)(facesDetected[i].Width * 0.35);

            result = currentFrame.Copy(facesDetected[i]).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
            result._EqualizeHist();
            face_PICBX.Image = result.ToBitmap();
            //draw the face detected in the 0th (gray) channel with blue color
            currentFrame.Draw(facesDetected[i], new Bgr(Color.Red), 2);

        }
        if (RECORD && facesDetected.Length > 0 && resultImages.Count < num_faces_to_aquire)
        {
            resultImages.Add(result);
            count_lbl.Text = "Count: " + resultImages.Count.ToString();
            if (resultImages.Count == num_faces_to_aquire)
            {
                ADD_BTN.Enabled = true;
                NEXT_BTN.Visible = true;
                PREV_btn.Visible = true;
                count_lbl.Visible = false;
                Restart_btn.Visible = true;
                ADD_ALL.Visible = true;
                RECORD = false;
                Application.Idle -= new EventHandler(FrameGrabber);
            }
        }
        image_PICBX.Image = currentFrame.ToBitmap();
    }
}

        //Saving The Data
        private bool save_training_data(Image face_data)
        {       
            try
            {
                Random rand = new Random();
                bool file_create = true;
                string facename = "face_" + NAME_PERSON.Text + "_" + rand.Next().ToString() + ".jpg";
                while (file_create)
                {

                    if (!File.Exists(Application.StartupPath + "/TrainedFaces/" + facename))
                    {
                        file_create = false;
                    }
                    else
                    {
                        facename = "face_" + NAME_PERSON.Text + "_" + rand.Next().ToString() + ".jpg";
                    }
                }


                if(Directory.Exists(Application.StartupPath + "/TrainedFaces/"))
                {
                    face_data.Save(Application.StartupPath + "/TrainedFaces/" + facename, ImageFormat.Jpeg);
                }
                else
                {
                    Directory.CreateDirectory(Application.StartupPath + "/TrainedFaces/");
                    face_data.Save(Application.StartupPath + "/TrainedFaces/" + facename, ImageFormat.Jpeg);
                }   
 
                EncryptFile(Application.StartupPath+"/TrainedFaces/"+facename);
                string myconnectionString ="DataSource="+ Application.StartupPath + "/../../ProjectData.sdf;"+"Password=VuMyc2iFdP0TiDgI7n";
                using (SqlCeConnection con = new SqlCeConnection(myconnectionString))
                {
                    con.Open();
                    var Rowid = new SqlCeCommand("SELECT TOP(1) FaceId FROM faces ORDER BY FaceId DESC", con).ExecuteScalar();
                    using (SqlCeCommand cmd = new SqlCeCommand("INSERT INTO faces VALUES (@FaceId, @Data, @User)", con))
                    {
                        int Newid = Convert.ToInt32(Rowid) + 1;
                        cmd.Parameters.AddWithValue("@FaceId", Newid);
                        cmd.Parameters.AddWithValue("@Data", Application.StartupPath + "/TrainedFaces/" + facename + "ENC");
                        cmd.Parameters.AddWithValue("@User", NAME_PERSON.Text);                        
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

                string cryptFile = inputFile+"ENC";
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

        ///<param name="inputFile"></param>
        ///<param name="outputFile"></param>
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

                FileStream fsOut = new FileStream(inputFile.Substring(0,inputFile.Length-3), FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();

            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

       
        
        //Add the image to training data
        private void ADD_BTN_Click(object sender, EventArgs e)
        { 
            if (resultImages.Count == num_faces_to_aquire)
            {
                if (!save_training_data(face_PICBX.Image)) MessageBox.Show("Error", "Error in saving file info. Training data not saved", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                stop_capture();
                if (!save_training_data(face_PICBX.Image)) MessageBox.Show("Error", "Error in saving file info. Training data not saved", MessageBoxButtons.OK, MessageBoxIcon.Error);
                initialise_capture();
            }
        }
        private void Restart_btn_Click(object sender, EventArgs e)
        {
            RECORD = false;
            resultImages.Clear();
            NEXT_BTN.Visible = false;
            PREV_btn.Visible = false;
            Application.Idle += new EventHandler(FrameGrabber);
            Restart_btn.Visible = false;
            count_lbl.Text = "Count: 0";
            count_lbl.Visible = true;
        }
        //Get 10 image to train
        private void RECORD_BTN_Click(object sender, EventArgs e)
        {
            if (RECORD)
            {
                RECORD = false;
            }
            else
            {
                if (resultImages.Count == 10)
                {
                    resultImages.Clear();
                    Application.Idle += new EventHandler(FrameGrabber);
                }
                RECORD = true;
                ADD_BTN.Enabled = false;
            }

        }
        private void NEXT_BTN_Click(object sender, EventArgs e)
        {
            if (results_list_pos < resultImages.Count - 1)
            {
                face_PICBX.Image = resultImages[results_list_pos].ToBitmap();
                results_list_pos++;
                PREV_btn.Enabled = true;
            }
            else
            {
                NEXT_BTN.Enabled = false;
            }
        }
        private void PREV_btn_Click(object sender, EventArgs e)
        {
            if (results_list_pos > 0)
            {
                results_list_pos--;
                face_PICBX.Image = resultImages[results_list_pos].ToBitmap();
                NEXT_BTN.Enabled = true;
            }
            else
            {
                PREV_btn.Enabled = false;
            }
        }
        private void ADD_ALL_Click(object sender, EventArgs e)
        {
            for(int i = 0; i<resultImages.Count;i++)
            {
                face_PICBX.Image = resultImages[i].ToBitmap();
                if (!save_training_data(face_PICBX.Image)) MessageBox.Show("Error", "Error in saving file info. Training data not saved", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Thread.Sleep(100);
            }
            ADD_ALL.Visible = false;
            //restart single face detection
            Restart_btn_Click(null, null);
        }


    }
}
