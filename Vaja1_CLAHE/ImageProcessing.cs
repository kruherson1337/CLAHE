using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vaja1_CLAHE
{
    public class ImageProcessing
    {
        public static void processImage(ref MyBitplane bitplane, Algorithm algorithm, int AHEWindowSize, double clipLimit)
        {
            switch (algorithm)
            {
                case Algorithm.HE:
                    // Histogram Equalization
                    HE(ref bitplane);
                    break;
                case Algorithm.AHE:
                    // Adaptive Histogram Equalization
                    AHE(ref bitplane, AHEWindowSize);
                    break;
                case Algorithm.CLHE:
                    // Contrast Limited Histogram Equalization
                    CLHE(ref bitplane, clipLimit);
                    break;
                case Algorithm.CLAHE:
                    // Contrast Limited Adaptive Histogram Equalization
                    CLAHE(ref bitplane, AHEWindowSize, clipLimit);
                    break;

                case Algorithm.LOAD_IMAGE:
                default:
                    break;
            }
        }

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

        private static void CL(ref MyBitplane bitplane, double clipLimit)
        {
            double cl = (clipLimit * (bitplane.Width * bitplane.Height)) / 256;
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
                for (i = 0; i < 256; i++)
                    if (histogram[i] > middle)
                        SUM += histogram[i] - middle;
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
                    bitplane.SetPixel(x, y, (byte)finalFreq[bitplane.GetPixel(x,y)]);

        }

        private static void CLHE(ref MyBitplane bitplane, double clipLimit)
        {
            CL(ref bitplane, clipLimit);
            HE(ref bitplane);
        }

        private static void CLAHE(ref MyBitplane bitplane, int windowSize, double clipLimit)
        {
            CL(ref bitplane, clipLimit);
            AHE(ref bitplane, windowSize);
        }

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

        public static double[] calculateHistogram(MyBitplane bitplane)
        {
            double[] histogram = new double[256];
            int x;
            for (int y = 0; y < bitplane.Height; ++y)
                for (x = 0; x < bitplane.Width; ++x)
                    ++histogram[bitplane.GetPixel(x, y)];
            return histogram;
        }

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
