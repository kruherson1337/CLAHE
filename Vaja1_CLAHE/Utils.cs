using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Vaja1_CLAHE
{
    public class Utils
    {
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
        public static BitmapSource getSource(Bitmap bmp)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bmp.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(
                    bmp.Width,
                    bmp.Height
                    ));
        }
        public static System.Windows.Controls.Image GetImageView(Bitmap bmp, double width, double height)
        {
            return new System.Windows.Controls.Image
            {
                Source = getSource(bmp),
                Width = width,
                Height = height,
                Stretch = Stretch.Fill
            };
        }
        public static double findMax(double[] array)
        {
            double max = double.MinValue;
            foreach (double value in array)
                if (value > max)
                    max = value;
            return max;
        }
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
