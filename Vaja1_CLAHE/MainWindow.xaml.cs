using System;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Diagnostics;

namespace Vaja1_CLAHE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private readonly BackgroundWorker backgroundWorker = new BackgroundWorker();
        private Stopwatch watch;

        private string originalFilename;

        private MyImage myImage;
        private double[][] histograms;
        private double[][] comulativeFrequencies;
        private Bitmap[] channelImages;

        public MainWindow()
        {
            InitializeComponent();

            backgroundWorker.DoWork += backgroundWorker_DoWork; 
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // get rid of managed resources
                backgroundWorker.Dispose();
            }
            // get rid of unmanaged resources
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Parameters parameters = (Parameters)e.Argument;
                watch = Stopwatch.StartNew();

                // Load image
                using (Bitmap image = new Bitmap(parameters.filename))
                {
                    // Convert to MyImage
                    myImage = new MyImage(image);
                    histograms = new double[myImage.NumCh][];
                    comulativeFrequencies = new double[myImage.NumCh][];
                    channelImages = new Bitmap[myImage.NumCh];

                    // Do paralel image process on each channel 
                    Parallel.ForEach(myImage.Bitplane, (bitplane, state, ch) =>
                    {
                        // Process current channel
                        ImageProcessing.processImage(ref bitplane, parameters.algorithm, parameters.windowSize, parameters.contrastLimit);

                        // Calculate Histogram
                        histograms[ch] = ImageProcessing.calculateHistogram(bitplane);

                        // Calculate Comulative Histogram
                        comulativeFrequencies[ch] = ImageProcessing.calculateComulativeFrequency(histograms[ch]);

                        // Get image by channels
                        channelImages[ch] = ImageRendering.getChannelImage(bitplane, (int)ch, myImage.NumCh);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            watch.Stop();
            string timeTaken = String.Format("Width: {0} Height: {1} Time taken: {2} ms", myImage.Width, myImage.Height, watch.ElapsedMilliseconds.ToString());
            Console.WriteLine(timeTaken);
            labelTimeTaken.Content = timeTaken;

            if (e.Cancelled)
            {
            }
            else if (e.Error != null)
            {
                Console.WriteLine(e.Error);
            }
            else
            {
                // Draw histogram and image for each channel
                DrawOnScreen(myImage.NumCh, channelImages);

                // Draw image on screen
                imageOriginal.Source = Utils.getSource(myImage.GetBitmap());

                // Draw graphs
                histogramR.PlotBars(histograms[2]);
                histogramG.PlotBars(histograms[1]);
                histogramB.PlotBars(histograms[0]);

                comulativeHistogramR.PlotY(comulativeFrequencies[2]);
                comulativeHistogramG.PlotY(comulativeFrequencies[1]);
                comulativeHistogramB.PlotY(comulativeFrequencies[0]);
            }
        }

        /// <summary>
        /// Draw histogram, comulative frequencies and channel images on screen
        /// </summary>
        private void DrawOnScreen(int numCh, Bitmap[] channelImages)
        {
            channelStack.Children.Clear();

            for (int ch = numCh - 1; ch >= 0; ch--)
            {
                channelStack.Children.Add(Utils.GetImageView(channelImages[ch], channelStack.Width / 3, channelStack.Height));
                channelImages[ch].Dispose();
            }
        }

        /// <summary>
        /// Save current displayed image
        /// </summary>
        private void saveCurrentImage()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (Bitmap bmp = Utils.ImageSourceToBytes(imageOriginal))
                {
                    bmp.Save(dialog.FileName, ImageFormat.Jpeg);
                }
            }
        }

        /// <summary>
        /// Load image from file
        /// </summary>
        private void loadImage()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif) | *.jpg; *.jpeg; *.jpe; *.jfif",
                Title = "Prosim izberite sliko"
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                originalFilename = dialog.FileName;
                backgroundWorker.RunWorkerAsync(new Parameters(originalFilename, Algorithm.LOAD_IMAGE, Double.Parse(textboxClipLimit.Text), Int32.Parse(textboxAHEWindowSize.Text)));
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Double click
            if (e.ClickCount == 2)
                loadImage();
        }

        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            loadImage();
        }

        private void buttonHE_Click(object sender, RoutedEventArgs e)
        {
            backgroundWorker.RunWorkerAsync(new Parameters(originalFilename, Algorithm.HE, Double.Parse(textboxClipLimit.Text), Int32.Parse(textboxAHEWindowSize.Text)));
        }

        private void buttonAHE_Click(object sender, RoutedEventArgs e)
        {
            backgroundWorker.RunWorkerAsync(new Parameters(originalFilename, Algorithm.AHE, Double.Parse(textboxClipLimit.Text), Int32.Parse(textboxAHEWindowSize.Text)));
        }

        private void buttonCLHE_Click(object sender, RoutedEventArgs e)
        {
            backgroundWorker.RunWorkerAsync(new Parameters(originalFilename, Algorithm.CLHE, Double.Parse(textboxClipLimit.Text), Int32.Parse(textboxAHEWindowSize.Text)));
        }

        private void buttonCLAHE_Click(object sender, RoutedEventArgs e)
        {
            backgroundWorker.RunWorkerAsync(new Parameters(originalFilename, Algorithm.CLAHE, Double.Parse(textboxClipLimit.Text), Int32.Parse(textboxAHEWindowSize.Text)));
        }

        private void imageOriginal_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            saveCurrentImage();
        }
    }
}
