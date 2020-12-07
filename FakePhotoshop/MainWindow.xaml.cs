using System;
using System.Windows;
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;

namespace FakePhotoshop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double FirstXPos, FirstYPos, FirstArrowXPos, FirstArrowYPos;
        object MovingObject;

        public MainWindow()
        {
            InitializeComponent();
        }

        string fileStream;

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // save image to bin folder
            SaveToPng(mainImage);
        }

        // open image
        private void OpenFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                fileStream = openFileDialog.InitialDirectory + openFileDialog.FileName;

                // load image to the wpf form
                mainImage.Source = new BitmapImage(new Uri(fileStream));
            }
        }

        private void greyScaleBtn_Click(object sender, RoutedEventArgs e)
        {
            // grayscale image with imagefactory
            mainImage.Source = Convert(ImageFactory.GreyScale(fileStream));
        }

        private void WatermarkBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                string watermark = openFileDialog.InitialDirectory + openFileDialog.FileName;
                Bitmap image = new Bitmap(watermark);

                //System.Windows.Media.Imaging.BitmapImage img = Convert(ImageFactory.WatermarkImage(fileStream, image));
                //mainImage.Source = img;

                System.Windows.Controls.Image finalImage = new System.Windows.Controls.Image();
                finalImage.Source = Convert(image);


                finalImage.Width = 400;
                finalImage.Height = 500;
                Canvas.SetZIndex(finalImage, 999);

                finalImage.PreviewMouseLeftButtonDown += this.MouseLeftButtonDown;
                finalImage.PreviewMouseLeftButtonUp += this.PreviewMouseLeftButtonUp;




                //DesigningCanvas.PreviewMouseMove += this.MouseMove;


                DesigningCanvas.Children.Add(finalImage);
            }

        }


        void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // In this event, we should set the lines visibility to Hidden
            MovingObject = null;

        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            /*
             * In this event, at first we check the mouse left button state. If it is pressed and 
             * event sender object is similar with our moving object, we can move our control with
             * some effects.
             */
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                /*
                 * For changing the position of a control, we should use the SetValue method to setting
                 * the Canvas.LeftProperty and Canvas.TopProperty dependencies.
                 * 
                 * For calculating the currect position of the control, we should do :
                 *      Current position of the mouse cursor on the object parent - 
                 *      Mouse position on the control at the start of moving -
                 *      position of the control's parent.
                 */
                (MovingObject as FrameworkElement).SetValue(Canvas.LeftProperty,
                    e.GetPosition((MovingObject as FrameworkElement).Parent as FrameworkElement).X - FirstXPos);

                (MovingObject as FrameworkElement).SetValue(Canvas.TopProperty,
                    e.GetPosition((MovingObject as FrameworkElement).Parent as FrameworkElement).Y - FirstYPos);
            }
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //In this event, we get current mouse position on the control to use it in the MouseMove event.
            FirstXPos = e.GetPosition(sender as Control).X;
            FirstYPos = e.GetPosition(sender as Control).Y;
            FirstArrowXPos = e.GetPosition((sender as Control).Parent as Control).X - FirstXPos;
            FirstArrowYPos = e.GetPosition((sender as Control).Parent as Control).Y - FirstYPos;
            MovingObject = sender;
        }




        private void FlashbackeBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Media.Imaging.BitmapImage img = Convert(ImageFactory.VietnamFlashback(fileStream, FakePhotoshop.Properties.Resources.vietnamflashback));

            mainImage.Source = img;
        }
        
        // converter for bitmap to bitmapimage
        public BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }




        // save the image to png
        void SaveToPng(FrameworkElement visual)
        {
            System.Windows.Forms.SaveFileDialog folderbrowser = new System.Windows.Forms.SaveFileDialog();
            folderbrowser.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif| PNG Image|*.png";


            if (folderbrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {   
                string filePath = folderbrowser.FileName;

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)mainImage.Source));
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    encoder.Save(stream);
            }
        }
    }
}
