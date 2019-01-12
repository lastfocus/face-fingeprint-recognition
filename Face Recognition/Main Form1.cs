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
using System.Xml;
using System.Runtime.InteropServices;
using System.Threading;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

//using Face_Recognition;

namespace protect_system
{
    public partial class Form1 : Form
    {
        #region variables
        Image<Bgr, Byte> currentFrame; //current image aquired from webcam for display
        Image<Gray, byte> result, TrainedFace = null; //used to store the result image and trained face
        Image<Gray, byte> gray_frame = null; //grayscale current image aquired from webcam for processing

        Capture grabber; //This is our capture variable

        public CascadeClassifier Face = new CascadeClassifier(Application.StartupPath + "/Cascades/haarcascade_frontalface_default.xml");//Our face detection method 

        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_COMPLEX, 0.5, 0.5); //Our fount for writing within the frame

        int NumLabels;
        
        //Classifier with default training location
        Classifier_Train Recognition = new Classifier_Train();

        #endregion

        public Form1(ref int error)
        {
            InitializeComponent();
            
            //Load of previus trainned faces and labels for each image

            if (Recognition.IsTrained)
            {
                message_bar.Text = "Training Data loaded";
            }
            else
            {
                message_bar.Text = "No training data found, please train program using Train menu option";
            }

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

        /*public Form1(Main_menu mm)
        {
            InitializeComponent();

            Recognition.LoadData(Application.StartupPath + "\\TrainedFaces");
        }*/

        //Open training form and pass this
        private void trainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Stop Camera
            stop_capture();

            //OpenForm
            int error = 0;
            Training_Form TF = new Training_Form(this, ref error);
            if (error == 0)
                TF.Show();
        }
        public void retrain()
        {

            Recognition = new Classifier_Train();
            if (Recognition.IsTrained)
            {
                message_bar.Text = "Training Data loaded";
            }
            else
            {
                message_bar.Text = "No training data found, please train program using Train menu option";
            }
        }

        //Camera Start Stop
        public void initialise_capture()
        {
            grabber = new Capture();
            grabber.QueryFrame();
            //Initialize the FrameGraber event
            if (parrellelToolStripMenuItem.Checked)
            {
                Application.Idle += new EventHandler(FrameGrabber_Parrellel);
            }
            else
            {
                Application.Idle += new EventHandler(FrameGrabber_Standard);
            }
        }
        private void stop_capture()
        {
            if (parrellelToolStripMenuItem.Checked)
            {
                Application.Idle -= new EventHandler(FrameGrabber_Parrellel);
            }
            else
            {
                Application.Idle -= new EventHandler(FrameGrabber_Standard);
            }
            if(grabber!= null)
            {
            grabber.Dispose();
            }
        }

        //Process Frame
        void FrameGrabber_Standard(object sender, EventArgs e)
        {
            //Get the current frame form capture device
            currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

            //Convert it to Grayscale
            if (currentFrame != null)
            {
                gray_frame = currentFrame.Convert<Gray, Byte>();

                //Face Detector
                Rectangle[] facesDetected = Face.DetectMultiScale(gray_frame, 1.2, 10, new Size(50, 50), Size.Empty);

                //Action for each element detected
                for (int i = 0; i < facesDetected.Length; i++)// (Rectangle face_found in facesDetected)
                {
                    //This will focus in on the face from the haar results its not perfect but it will remove a majoriy
                    //of the background noise
                    facesDetected[i].X += (int)(facesDetected[i].Height * 0.15);
                    facesDetected[i].Y += (int)(facesDetected[i].Width * 0.22);
                    facesDetected[i].Height -= (int)(facesDetected[i].Height * 0.3);
                    facesDetected[i].Width -= (int)(facesDetected[i].Width * 0.35);

                    result = currentFrame.Copy(facesDetected[i]).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                    result._EqualizeHist();
                    //draw the face detected in the 0th (gray) channel with blue color
                    currentFrame.Draw(facesDetected[i], new Bgr(Color.Red), 2);

                    if (Recognition.IsTrained)
                    {
                        string name = Recognition.Recognise(result);
                        int match_value = (int)Recognition.Get_Eigen_Distance;

                        //Draw the label for each face detected and recognized
                        currentFrame.Draw(name + " ", ref font, new Point(facesDetected[i].X - 2, facesDetected[i].Y - 2), new Bgr(Color.LightGreen));
                        ADD_Face_Found(result, name, match_value);
                    }
                }
                //Show the faces procesed and recognized
                image_PICBX.Image = currentFrame.ToBitmap();
            }
        }

        void FrameGrabber_Parrellel(object sender, EventArgs e)
        {
            //Get the current frame form capture device
            currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

            //Convert it to Grayscale
            //Clear_Faces_Found();

            if (currentFrame != null)
            {
                gray_frame = currentFrame.Convert<Gray, Byte>();
                //Face Detector
                Rectangle[] facesDetected = Face.DetectMultiScale(gray_frame, 1.2, 10, new Size(50, 50), Size.Empty);

                //Action for each element detected
                Parallel.For(0, facesDetected.Length, i =>
                    {
                        try
                        {
                            facesDetected[i].X += (int)(facesDetected[i].Height * 0.15);
                            facesDetected[i].Y += (int)(facesDetected[i].Width * 0.22);
                            facesDetected[i].Height -= (int)(facesDetected[i].Height * 0.3);
                            facesDetected[i].Width -= (int)(facesDetected[i].Width * 0.35);

                            result = currentFrame.Copy(facesDetected[i]).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                            result._EqualizeHist();
                            //draw the face detected in the 0th (gray) channel with blue color
                            currentFrame.Draw(facesDetected[i], new Bgr(Color.Red), 2);

                            if (Recognition.IsTrained)
                            {
                                string name = Recognition.Recognise(result);
                                int match_value = (int)Recognition.Get_Eigen_Distance;

                                //Draw the label for each face detected and recognized
                                currentFrame.Draw(name + " ", ref font, new Point(facesDetected[i].X - 2, facesDetected[i].Y - 2), new Bgr(Color.LightGreen));
                                ADD_Face_Found(result, name, match_value);
                            }
                            
                        }
                        catch
                        {
                            //do nothing as parrellel loop buggy
                            //No action as the error is useless, it is simply an error in 
                            //no data being there to process and this occurss sporadically 
                        }
                    });
                //Show the faces procesed and recognized
                image_PICBX.Image = currentFrame.ToBitmap();
            }
        }

        //ADD Picture box and label to a panel for each face
        int faces_count = 0;
        int faces_panel_Y = 0;
        int faces_panel_X = 0;

        void Clear_Faces_Found()
        {
            this.Faces_Found_Panel.Controls.Clear();
            faces_count = 0;
            faces_panel_Y = 0;
            faces_panel_X = 0;
        }
        void ADD_Face_Found(Image<Gray, Byte> img_found, string name_person, int match_value)
        {
            PictureBox PI = new PictureBox();
            PI.Location = new Point(faces_panel_X, faces_panel_Y);
            PI.Height = 80;
            PI.Width = 80;
            PI.SizeMode = PictureBoxSizeMode.StretchImage;
            PI.Image = img_found.ToBitmap();
            Label LB = new Label();
            LB.Text = name_person + " " + match_value.ToString();
            LB.Location = new Point(faces_panel_X, faces_panel_Y + 80);
            //LB.Width = 80;
            LB.Height = 15;
           
            this.Faces_Found_Panel.Controls.Add(PI);
            this.Faces_Found_Panel.Controls.Add(LB);
            faces_count++;
            if (faces_count == 2)
            {
                faces_panel_X = 0;
                faces_panel_Y += 100;
                faces_count = 0;
            }
            else faces_panel_X += 85;

            if (Faces_Found_Panel.Controls.Count > 10)
            {
                Clear_Faces_Found();
            }

        }

        //Menu Opeartions
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void singleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parrellelToolStripMenuItem.Checked = false;
            singleToolStripMenuItem.Checked = true;
            Application.Idle -= new EventHandler(FrameGrabber_Parrellel);
            Application.Idle += new EventHandler(FrameGrabber_Standard);
        }
        private void parrellelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parrellelToolStripMenuItem.Checked = true;
            singleToolStripMenuItem.Checked = false;
            Application.Idle -= new EventHandler(FrameGrabber_Standard);
            Application.Idle += new EventHandler(FrameGrabber_Parrellel);
            
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog SF = new SaveFileDialog();
            //As there is no identification in files to recogniser type we will set the extension ofthe file to tell us
            switch (Recognition.Recognizer_Type)
            {
                case ("EMGU.CV.LBPHFaceRecognizer"):
                    SF.Filter = "LBPHFaceRecognizer File (*.LBPH)|*.LBPH";
                    break;
                case ("EMGU.CV.FisherFaceRecognizer"):
                    SF.Filter = "FisherFaceRecognizer File (*.FFR)|*.FFR";
                    break;
                case ("EMGU.CV.EigenFaceRecognizer"):
                    SF.Filter = "EigenFaceRecognizer File (*.EFR)|*.EFR";
                    break;
            }
            if (SF.ShowDialog() == DialogResult.OK)
            {
                Recognition.Save_Eigen_Recogniser(SF.FileName);
            }
        }
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog OF = new OpenFileDialog();
            OF.Filter = "EigenFaceRecognizer File (*.EFR)|*.EFR|LBPHFaceRecognizer File (*.LBPH)|*.LBPH|FisherFaceRecognizer File (*.FFR)|*.FFR";
            if (OF.ShowDialog() == DialogResult.OK)
            {
                Recognition.Load_Eigen_Recogniser(OF.FileName);
            }
        }

        //Unknow Eigen face calibration
        private void Eigne_threshold_txtbxChanged(object sender, EventArgs e)
        {
            try
            {
                Recognition.Set_Eigen_Threshold = Math.Abs(Convert.ToInt32(Eigne_threshold_txtbx.Text));
                message_bar.Text = "Eigen Threshold Set";
            }
            catch
            {
                message_bar.Text = "Error in Threshold input please use int";
            }
        }

        private void eigenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Uncheck other menu items
            fisherToolStripMenuItem.Checked = false;
            lBPHToolStripMenuItem.Checked = false;

            Recognition.Recognizer_Type = "EMGU.CV.EigenFaceRecognizer";
            Recognition.Retrain();
        }

        private void fisherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Uncheck other menu items
            lBPHToolStripMenuItem.Checked = false;
            eigenToolStripMenuItem.Checked = false;

            Recognition.Recognizer_Type = "EMGU.CV.FisherFaceRecognizer";
            Recognition.Retrain();
        }

        private void lBPHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Uncheck other menu items
            fisherToolStripMenuItem.Checked = false;
            eigenToolStripMenuItem.Checked = false;

            Recognition.Recognizer_Type = "EMGU.CV.LBPHFaceRecognizer";
            Recognition.Retrain();
        }
    }
}
