using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;

namespace FakePhotoshop
{
    class ImageFactory
    {
        public static Bitmap GreyScale(string fileStream)
        {
            Bitmap bmp = new Bitmap(fileStream);

            //get image dimension
            int width = bmp.Width;
            int height = bmp.Height;

            //color of pixel
            Color p;

            //grayscale
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //get pixel value
                    p = bmp.GetPixel(x, y);

                    //extract pixel component ARGB
                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;

                    //find average
                    int avg = (r + g + b) / 3;

                    //set new pixel value
                    bmp.SetPixel(x, y, Color.FromArgb(a, avg, avg, avg));
                }
            }

            return bmp;
        }

        public static Bitmap WatermarkImage(string fileStream, Bitmap watermark)
        {
            // load image
            Bitmap image = new Bitmap(fileStream);

            // set image watermark
            using (Graphics imageGraphics = Graphics.FromImage(image))
            {
                watermark.SetResolution(imageGraphics.DpiX, imageGraphics.DpiY);

                int x = (image.Width - watermark.Width) / 2;
                int y = (image.Height - watermark.Height) / 2;

                int watermarkWidth = watermark.Width / 4;
                int watermarkHeight = watermark.Height / 4;

                imageGraphics.DrawImage(watermark, x, y, watermarkWidth, watermarkHeight);
            }

            return image;
        }



        public static Bitmap VietnamFlashback(string fileStream, Bitmap watermark)
        {
            // load image
            Bitmap image = new Bitmap(fileStream);

            // set image flashback
            using (Graphics imageGraphics = Graphics.FromImage(image))
            {
                watermark.SetResolution(imageGraphics.DpiX, imageGraphics.DpiY);

                
                int x = (image.Width - watermark.Width) / 2;
                int y = (image.Height - watermark.Height) / 2;

                imageGraphics.DrawImage(SetImageOpacity(watermark, 0.3f), x, y, watermark.Width, watermark.Height);
            }

            return image;
        }


        public static Bitmap SetImageOpacity(Bitmap image, float opacity)
        {
            try
            {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(image.Width, image.Height);

                //create a graphics object from the image  
                using (Graphics gfx = Graphics.FromImage(bmp))
                {

                    //create a color matrix object  
                    ColorMatrix matrix = new ColorMatrix();

                    //set the opacity  
                    matrix.Matrix33 = opacity;

                    //create image attributes  
                    ImageAttributes attributes = new ImageAttributes();

                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //now draw the image  
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
