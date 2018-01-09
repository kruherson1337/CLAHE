using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vaja1_CLAHE
{
    public class ImageRendering
    {
        public static Bitmap getChannelImage(MyBitplane bitplane, int ch, int numCh)
        {
            // Draw image channel
            MyImage myImage = new MyImage(bitplane.Width, bitplane.Height, numCh);
            myImage.Bitplane[ch] = bitplane;

            return myImage.GetBitmap();
        }

        public static Bitmap drawHistogram(double[] histogram)
        {
            double max = Utils.findMax(histogram);

            // Draw Histogram
            Bitmap bmpHistogram = new Bitmap(256, 256);
            using (Graphics graphics = Graphics.FromImage(bmpHistogram))
            {
                for (int i = 0; i < 256; ++i)
                    graphics.DrawLine(Pens.Black, new PointF(i, 255), new PointF(i, (float)(255 - (histogram[i] / max) * 255)));
            }
            return bmpHistogram;
        }

        public static Bitmap drawComulativeFrequency(double[] comulativeFrequency)
        {
            double max = Utils.findMax(comulativeFrequency);

            // Draw Comulative Frequency
            Bitmap bmpHistogram = new Bitmap(256, 256);
            using (Graphics graphics = Graphics.FromImage(bmpHistogram))
            {
                using (Pen blackPen = new Pen(Color.Black,2))
                {
                    for (int i = 0; i < 256; ++i)
                    {
                        float percentage = (float)((comulativeFrequency[i] / max) * 255);
                        graphics.DrawLine(blackPen, new PointF(i, (255 - percentage)+2), new PointF(i, 255 - percentage));
                    }
                }
            }
            return bmpHistogram;
        }
    }
}
