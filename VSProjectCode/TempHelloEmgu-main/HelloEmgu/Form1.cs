using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoCoMoCoTest
{
    public partial class Form1 : Form
    {
        private VideoCapture _capture;
        private LoCoMoCo MoveRobot;
        private Thread _captureThread;
        private int _threshold = 150;
        private int threshold_Hmin = 0, threshold_Hmax = 179, threshold_Smin = 0, threshold_Smax = 255, threshold_Vmin = 0, threshold_Vmax = 255;
        private int threshold_YHmin = 0, threshold_YHmax = 179, threshold_YSmin = 0, threshold_YSmax = 255, threshold_YVmin = 0, threshold_YVmax = 255;
        public Form1()
        {
            InitializeComponent();
        }

        private void trackBarH1_Scroll(object sender, EventArgs e)
        {
            threshold_Hmin = trackBarH1.Value;
            Hmin.Text = $"{threshold_Hmin}";
        }

        private void trackBarH2_Scroll(object sender, EventArgs e)
        {
            threshold_Hmax = trackBarH2.Value;
        }

        private void trackBarS1_Scroll(object sender, EventArgs e)
        {
            threshold_Smin = trackBarS1.Value;
        }

        private void trackBarS2_Scroll(object sender, EventArgs e)
        {
            threshold_Smax = trackBarS2.Value;
        }

        private void trackBarV1_Scroll(object sender, EventArgs e)
        {
            threshold_Vmin = trackBarV1.Value;
        }

   
        private void trackBarV2_Scroll(object sender, EventArgs e)
        {
            threshold_Vmax = trackBarV2.Value;
        }

        private void trackBarYH1_Scroll(object sender, EventArgs e)
        {
            threshold_YHmin = trackBarYH1.Value;
        }

        private void trackBarYH2_Scroll(object sender, EventArgs e)
        {
            threshold_YHmax = trackBarYH2.Value;
        }

        private void trackBarYS1_Scroll(object sender, EventArgs e)
        {
            threshold_YSmin = trackBarYS1.Value;
        }

        private void trackBarYS2_Scroll(object sender, EventArgs e)
        {
            threshold_YSmax = trackBarYS2.Value;
        }

        private void trackBarYV1_Scroll(object sender, EventArgs e)
        {
            threshold_YVmin = trackBarYV1.Value;
        }

        private void trackBarYV2_Scroll(object sender, EventArgs e)
        {
            threshold_YVmax = trackBarYV2.Value;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _capture = new VideoCapture(0);
            _captureThread = new Thread(DisplayWebcam);
            _captureThread.Start();
   
            thresholdTrackbar.Value = _threshold;
        }
        private void DisplayWebcam()
        {
            while (_capture.IsOpened)
            {
                //Frame maintenance:
                Mat frame = _capture.QueryFrame();

                //resize to PictureBox aspect ratio
                int newHeight = (frame.Size.Height * emguPictureBox.Size.Width) / frame.Size.Width;

                Size newSize = new Size(emguPictureBox.Size.Width, newHeight);

                CvInvoke.Resize(frame, frame, newSize);

                // display the image in the PictureBox
                emguPictureBox.Image = frame.ToBitmap();

                Mat greyFrame= new Mat();


                CvInvoke.CvtColor(frame, greyFrame, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

                CvInvoke.Threshold(greyFrame, greyFrame ,_threshold, 255 , Emgu.CV.CvEnum.ThresholdType.Binary);

               emguBinary.Image = greyFrame.ToBitmap();

                //Convert the image from camera to HSV image of the red line
                Mat hsvFrame = new Mat();

                CvInvoke.CvtColor(frame, hsvFrame, Emgu.CV.CvEnum.ColorConversion.Bgr2Hsv);

                Mat[] hsvChannels = hsvFrame.Split();

                //hue filter
                Mat hueFilter = new Mat();
                CvInvoke.InRange(hsvChannels[0], new ScalarArray(threshold_Hmin), new ScalarArray(threshold_Hmax), hueFilter);
                Invoke(new Action(() => { hPictureBox.Image = hueFilter.ToBitmap(); }));

                //saturation

                Mat saturationFilter = new Mat();
                CvInvoke.InRange(hsvChannels[1], new ScalarArray(threshold_Smin), new ScalarArray(threshold_Smax), saturationFilter);
                Invoke(new Action(() => { sPictureBox.Image = saturationFilter.ToBitmap(); }));

                //value

                Mat valueFilter = new Mat();
                CvInvoke.InRange(hsvChannels[2], new ScalarArray(threshold_Vmin), new ScalarArray(threshold_Vmax), valueFilter);
                Invoke(new Action(() => { vPictureBox.Image = valueFilter.ToBitmap(); }));

                //mergedImage
                Mat mergedImage = new Mat();
                CvInvoke.BitwiseAnd(hueFilter, saturationFilter, mergedImage);
                CvInvoke.BitwiseAnd(mergedImage, valueFilter, mergedImage);
                Invoke(new Action(() => { mergedPictureBox.Image = mergedImage.ToBitmap(); }));




                //Convert the image from camera to HSV image of the yellow line
                Mat hsvFrameY = new Mat();

                CvInvoke.CvtColor(frame, hsvFrameY, Emgu.CV.CvEnum.ColorConversion.Bgr2Hsv);

                Mat[] hsvChannelsY = hsvFrameY.Split();


                //hue filter
                Mat hueFilterY = new Mat();
                CvInvoke.InRange(hsvChannelsY[0], new ScalarArray(threshold_YHmin), new ScalarArray(threshold_YHmax), hueFilterY);
                Invoke(new Action(() => { hPictureBoxY.Image = hueFilterY.ToBitmap(); }));

                //saturation

                Mat saturationFilterY = new Mat();
                CvInvoke.InRange(hsvChannelsY[1], new ScalarArray(threshold_YSmin), new ScalarArray(threshold_YSmax), saturationFilterY);
                Invoke(new Action(() => { sPictureBoxY.Image = saturationFilterY.ToBitmap(); }));

                //value

                Mat valueFilterY = new Mat();
                CvInvoke.InRange(hsvChannelsY[2], new ScalarArray(threshold_YVmin), new ScalarArray(threshold_YVmax), valueFilterY);
                Invoke(new Action(() => { vPictureBoxY.Image = valueFilterY.ToBitmap(); }));

                //mergedImage
                Mat mergedImageY = new Mat();
                CvInvoke.BitwiseAnd(hueFilterY, saturationFilterY, mergedImageY);
                CvInvoke.BitwiseAnd(mergedImageY, valueFilterY, mergedImageY);
                Invoke(new Action(() => { mergedPictureBoxY.Image = mergedImageY.ToBitmap(); }));




                //Slice 1
                int whitePixels = 0;
                Image<Gray, byte> img2 = greyFrame.ToImage<Gray, byte>(); 
                for(int x= 0; x <greyFrame.Width / 5; x++)
                {
                    for(int y =0;y <greyFrame.Height; y++)
                    {
                        if (img2.Data[y, x, 0] == 255) whitePixels++;
                        
                 
                    }
                }
                Invoke(new Action(() =>
                {
                    Slice1.Text = $"{whitePixels} White Pixels";
               
                }));


                // Slice 2
                whitePixels = 0;

         
                for (int x = greyFrame.Width / 5; x < 2 * (greyFrame.Width / 5); x++)
                {
                    for (int y = 0; y < greyFrame.Height; y++)
                    {
                        if (img2.Data[y, x, 0] == 255) whitePixels++;
                    }
                }
                Invoke(new Action(() =>
                {
                    Slice2.Text = $"{whitePixels} White Pixels";
                }));


               //Slice 3
                whitePixels = 0;

          
                for (int x = 2 * (greyFrame.Width / 5); x < 3 * (greyFrame.Width / 5); x++)
                {
                    for (int y = 0; y < greyFrame.Height; y++)
                    {
                        if (img2.Data[y, x, 0] == 255) whitePixels++;
                    }
                }
                Invoke(new Action(() =>
                {
                    Slice3.Text = $"{whitePixels} White Pixels";
                }));

                //Slice4
                whitePixels = 0;

                for (int x = 3 * (mergedImage.Width / 5); x < 4 * (mergedImage.Width / 5); x++)
                {
                    for (int y = 0; y < mergedImage.Height; y++)
                    {
                        if (img2.Data[y, x, 0] == 255) whitePixels++;
                    }
                }
                Invoke(new Action(() =>
                {
                    Slice4.Text = $"{whitePixels} White Pixels";
                }));

                //Slice5
                whitePixels = 0;

                for (int x = 4 * (greyFrame.Width / 5); x < greyFrame.Width; x++)
                {
                    for (int y = 0; y < greyFrame.Height; y++)
                    {
                        if (img2.Data[y, x, 0] == 255) whitePixels++;
                    }
                }
                Invoke(new Action(() =>
                {
                    Slice5.Text = $"{whitePixels} White Pixels";
                }));



                Thread.Sleep(10);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _captureThread.Abort();
        }

        private void thresholdTrackbar_ValueChanged(object sender, EventArgs e)
        {
            _threshold = thresholdTrackbar.Value;
        }


    
    }
}
