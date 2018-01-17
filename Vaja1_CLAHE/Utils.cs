using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Vaja1_CLAHE
{
    public class Utils
    {
        /// <summary>
        /// Get bitmap from Image view control
        /// </summary>
        /// <param name="control">Image view control</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ImageSourceToBytes(System.Windows.Controls.Image control)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)control.ActualWidth, (int)control.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(control);
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));
            MemoryStream stream = new MemoryStream();
            png.Save(stream);
            Image image = Image.FromStream(stream);
            return new Bitmap(image);
        }

        /// <summary>
        /// Get bitmapSource from bitmap
        /// </summary>
        /// <param name="bmp">bitmap</param>
        /// <returns>bitmapSource</returns>
        public static BitmapSource getSource(Bitmap bmp)
        {
            BitmapSource bmpSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bmp.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(
                    bmp.Width,
                    bmp.Height
                    ));
            bmp.Dispose();
            return bmpSource;
        }

        /// <summary>
        /// Create image view from bitmap
        /// </summary>
        /// <param name="bmp">bitmap to be displayed</param>
        /// <param name="width">width of image view</param>
        /// <param name="height">height of image view</param>
        /// <returns>Image view control</returns>
        public static System.Windows.Controls.Image GetImageView(Bitmap bmp, double width, double height)
        {
            return new System.Windows.Controls.Image
            {
                Width = width,
                Height = height,
                Source = getSource(bmp),
                Stretch = Stretch.Fill
            };
        }

        /// <summary>
        /// Find max value in array
        /// </summary>
        public static double findMax(double[] array)
        {
            double max = double.MinValue;
            foreach (double value in array)
                if (value > max)
                    max = value;
            return max;
        }

        /// <summary>
        /// Find min value in array
        /// </summary>
        public static double findMin(double[] array)
        {
            double min = double.MaxValue;
            foreach (double value in array)
                if (value < min)
                    min = value;
            return min;
        }
    }
}
