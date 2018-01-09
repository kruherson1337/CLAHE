using System.Drawing;

namespace Vaja1_CLAHE
{
    public class ImageRendering
    {
        /// <summary>
        /// Draw channel image
        /// </summary>
        /// <param name="bitplane">bitplane of current image</param>
        /// <param name="ch">current channel</param>
        /// <param name="numCh">total number of channels</param>
        /// <returns>Image</returns>
        public static Bitmap getChannelImage(MyBitplane bitplane, int ch, int numCh)
        {
            MyImage myImage = new MyImage(bitplane.Width, bitplane.Height, numCh);
            myImage.Bitplane[ch] = bitplane;

            return myImage.GetBitmap();
        }

        /// <summary>
        /// Draw histogram from calculated histogram array
        /// </summary>
        /// <param name="histogram">calculated histogram array</param>
        /// <returns>Image</returns>
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

        /// <summary>
        /// Draw comulative frequency from calculated comulative frequency
        /// </summary>
        /// <param name="comulativeFrequency">calculated comulative frequency</param>
        /// <returns>Image</returns>
        public static Bitmap drawComulativeFrequency(double[] comulativeFrequency)
        {
            double max = Utils.findMax(comulativeFrequency);

            // Draw Comulative Frequency
            Bitmap bmpHistogram = new Bitmap(256, 256);
            using (Graphics graphics = Graphics.FromImage(bmpHistogram))
            {
                using (Pen blackPen = new Pen(Color.Black, 2))
                {
                    for (int i = 0; i < 256; ++i)
                    {
                        float percentage = (float)((comulativeFrequency[i] / max) * 255);
                        graphics.DrawLine(blackPen, new PointF(i, (255 - percentage) + 2), new PointF(i, 255 - percentage));
                    }
                }
            }
            return bmpHistogram;
        }
    }
}
