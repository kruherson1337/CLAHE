using System;

namespace Vaja1_CLAHE
{
    public class ImageProcessing
    {
        /// <summary>
        /// Image process manager
        /// </summary>
        /// <param name="bitplane">bitplane of current image</param>
        /// <param name="algorithm">Selected algorithm</param>
        /// <param name="windowSize">Size of window</param>
        /// <param name="contrastLimit">Contrast Limit</param>
        public static void processImage(ref MyBitplane bitplane, Algorithm algorithm, int windowSize, double contrastLimit)
        {
            switch (algorithm)
            {
                case Algorithm.HE:
                    // Histogram Equalization
                    HE(ref bitplane);
                    break;
                case Algorithm.AHE:
                    // Adaptive Histogram Equalization
                    AHE(ref bitplane, windowSize);
                    break;
                case Algorithm.CLHE:
                    // Contrast Limited Histogram Equalization
                    CLHE(ref bitplane, contrastLimit);
                    break;
                case Algorithm.CLAHE:
                    // Contrast Limited Adaptive Histogram Equalization
                    CLAHE(ref bitplane, windowSize, contrastLimit);
                    break;

                case Algorithm.LOAD_IMAGE:
                default:
                    // Do nothing
                    break;
            }
        }

        /// <summary>
        /// Histogram Equalization
        /// </summary>
        /// <param name="bitplane">bitplane of current image</param>
        private static void HE(ref MyBitplane bitplane)
        {
            // Histogram
            double[] histogram = calculateHistogram(bitplane);

            // Probability
            double totalElements = bitplane.Width * bitplane.Height;
            double[] probability = new double[256];
            int i;
            for (i = 0; i < 256; i++)
                probability[i] = histogram[i] / totalElements;

            // Comulative probability
            double[] comulativeProbability = calculateComulativeFrequency(probability);

            // Multiply comulative probability by 256
            int[] floorProbability = new int[256];
            for (i = 0; i < 256; i++)
                floorProbability[i] = (int)Math.Floor(comulativeProbability[i] * 255);

            // Transform old value to new value
            int x;
            for (int y = 0; y < bitplane.Height; y++)
                for (x = 0; x < bitplane.Width; x++)
                    bitplane.SetPixel(x, y, (byte)floorProbability[bitplane.GetPixel(x, y)]);
        }

        /// <summary>
        /// Adaptive Histogram Equalization
        /// </summary>
        /// <param name="bitplane">bitplane of current image</param>
        /// <param name="windowSize">size of window</param>
        private static void AHE(ref MyBitplane bitplane, int windowSize)
        {
            // Prepare data
            MyBitplane newBitplane = new MyBitplane(bitplane.Width, bitplane.Height);
            MyBitplane window = new MyBitplane(windowSize, windowSize);

            int x;
            int y;
            for (y = 0; y < bitplane.Height; ++y)
            {
                for (x = 0; x < bitplane.Width; ++x)
                {
                    // Create window
                    CreateWindow(ref bitplane, windowSize, ref window, y, x);

                    // Histogram equalization on window
                    HE(ref window);

                    // Replace pixel from windowHE
                    newBitplane.SetPixel(x, y, window.GetPixel(windowSize / 2, windowSize / 2));
                }
            }

            // Copy
            for (y = 0; y < bitplane.Height; ++y)
                for (x = 0; x < bitplane.Width; ++x)
                    bitplane.SetPixel(x, y, newBitplane.GetPixel(x, y));
        }
        
        /// <summary>
        /// Contrast Limited Histogram Equalization implementation
        /// </summary>
        /// <param name="bitplane"></param>
        /// <param name="contrastLimit"></param>
        private static void CLHE(ref MyBitplane bitplane, double contrastLimit)
        {
            double cl = (contrastLimit * (bitplane.Width * bitplane.Height)) / 256;
            double top = cl;
            double bottom = 0;
            double SUM = 0;
            int i;

            // Histogram
            double[] histogram = calculateHistogram(bitplane);

            while (top - bottom > 1)
            {
                double middle = (top + bottom) / 2;
                SUM = 0;
                foreach (double value in histogram)
                    if (value > middle)
                        SUM += value - middle;
                if (SUM > (cl - middle) * 256)
                    top = middle;
                else
                    bottom = middle;
            }

            double clipLevel = Math.Round(bottom + (SUM / 256));

            double L = cl - clipLevel;
            for (i = 0; i < 256; i++)
                if (histogram[i] >= clipLevel)
                    histogram[i] = clipLevel;
                else
                    histogram[i] += L;

            double perBin = SUM / 256;

            for (i = 0; i < 256; i++)
                histogram[i] += perBin;

            histogram = calculateComulativeFrequency(histogram);
            int[] finalFreq = new int[256];
            double min = Utils.findMin(histogram);
            for (i = 0; i < 256; i++)
                finalFreq[i] = (int)((histogram[i] - min) / ((bitplane.Width * bitplane.Height) - 2) * 255);

            int x;
            for (int y = 0; y < bitplane.Height; ++y)
                for (x = 0; x < bitplane.Width; ++x)
                    bitplane.SetPixel(x, y, (byte)finalFreq[bitplane.GetPixel(x, y)]);
        }

        /// <summary>
        /// Contrast Limited Adaptive Histogram Equalization implementation
        /// </summary>
        /// <param name="bitplane">bitplane of current image</param>
        /// <param name="windowSize">size of window</param>
        /// <param name="contrastLimit">Contrast limit param</param>
        private static void CLAHE(ref MyBitplane bitplane, int windowSize, double contrastLimit)
        {
            // Prepare data
            MyBitplane newBitplane = new MyBitplane(bitplane.Width, bitplane.Height);
            MyBitplane window = new MyBitplane(windowSize, windowSize);

            int x;
            int y;
            for (y = 0; y < bitplane.Height; ++y)
            {
                for (x = 0; x < bitplane.Width; ++x)
                {
                    // Create window
                    CreateWindow(ref bitplane, windowSize, ref window, y, x);

                    // Contrast Limit Histogram equalization on window
                    CLHE(ref window, contrastLimit);
                    
                    // Replace pixel from windowHE
                    newBitplane.SetPixel(x, y, window.GetPixel(windowSize / 2, windowSize / 2));
                }
            }

            // Copy
            for (y = 0; y < bitplane.Height; ++y)
                for (x = 0; x < bitplane.Width; ++x)
                    bitplane.SetPixel(x, y, newBitplane.GetPixel(x, y));
        }

        /// <summary>
        /// Create window on current bitplane
        /// Edges are mirrored
        /// </summary>
        /// <param name="bitplane">bitplane of current image</param>
        /// <param name="windowSize">size of window</param>
        /// <param name="window">window reference filled from bitmap</param>
        /// <param name="y">current y position on bitplane</param>
        /// <param name="x">current x position on bitplane</param>
        private static void CreateWindow(ref MyBitplane bitplane, int windowSize, ref MyBitplane window, int y, int x)
        {
            int jIndex = 0;
            int iIndex;
            int i;
            for (int j = 0 - (windowSize / 2); j < (windowSize / 2); ++j)
            {
                iIndex = 0;
                for (i = 0 - (windowSize / 2); i < (windowSize / 2); ++i)
                {
                    int xx = x + i;
                    if (xx < 0)
                        xx = Math.Abs(xx);
                    if (xx >= bitplane.Width)
                        xx = (bitplane.Width - 1) + ((bitplane.Width) - (xx + windowSize));
                    int yy = y + j;
                    if (yy < 0)
                        yy = Math.Abs(yy);
                    if (yy >= bitplane.Height)
                        yy = (bitplane.Height - 1) + ((bitplane.Height) - (yy + windowSize));

                    window.SetPixel(iIndex, jIndex, bitplane.GetPixel(xx, yy));
                    ++iIndex;
                }
                ++jIndex;
            }
        }

        /// <summary>
        /// Calculates histogram based on input bitplane
        /// </summary>
        /// <param name="bitplane">bitplane of current image</param>
        /// <returns>double array histogram of input bitplane</returns>
        public static double[] calculateHistogram(MyBitplane bitplane)
        {
            double[] histogram = new double[256];
            int x;
            for (int y = 0; y < bitplane.Height; ++y)
                for (x = 0; x < bitplane.Width; ++x)
                    ++histogram[bitplane.GetPixel(x, y)];
            return histogram;
        }

        /// <summary>
        /// Calculate comulative frequency of input array
        /// </summary>
        /// <param name="array">double array of frequencies</param>
        /// <returns>double array for comulative frequencies</returns>
        public static double[] calculateComulativeFrequency(double[] array)
        {
            int size = array.Length;
            double[] comulativeFreq = new double[size];
            comulativeFreq[0] = array[0];
            for (int i = 1; i < size; ++i)
                comulativeFreq[i] = comulativeFreq[i - 1] + array[i];
            return comulativeFreq;
        }
    }
}
