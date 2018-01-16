using System;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using System.Drawing.Imaging;

namespace Vaja1_CLAHE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string originalFilename;

        public MainWindow()
        {
            InitializeComponent();

            // Load default image
            string defaultImagePath = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\..\\..\\Resources\\Pic3.jpg";
            processImage(defaultImagePath, Algorithm.LOAD_IMAGE);
        }

        private void processImage(string filename, Algorithm algorithm)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            labelTimeTaken.Content = "";

            // save filename 
            originalFilename = filename;

            // Load image
            using (Bitmap image = new Bitmap(filename))
            {
                Console.WriteLine("Filename: " + filename);
                Console.WriteLine("Width: " + image.Width);
                labelTimeTaken.Content += "Width: " + image.Width;
                Console.WriteLine("Height: " + image.Height);
                labelTimeTaken.Content += " Height: " + image.Height;

                // Convert to MyImage
                MyImage myImage = new MyImage(image);

                // Calculate each channel separated
                double[][] histograms = new double[myImage.NumCh][];
                double[][] comulativeFrequencies = new double[myImage.NumCh][];
                Bitmap[] channelImages = new Bitmap[myImage.NumCh];

                int windowSize = int.Parse(textboxAHEWindowSize.Text);
                double clipLimit = Double.Parse(textboxClipLimit.Text);

                // Do paralel image process on each channel 
                Parallel.ForEach(myImage.Bitplane, (bitplane, state, ch) =>
                {
                    // Process current channel
                    ImageProcessing.processImage(ref bitplane, algorithm, windowSize, clipLimit);

                    // Calculate Histogram
                    histograms[ch] = ImageProcessing.calculateHistogram(bitplane);

                    // Calculate Comulative Histogram
                    comulativeFrequencies[ch] = ImageProcessing.calculateComulativeFrequency(histograms[ch]);

                    // Get image by channels
                    channelImages[ch] = ImageRendering.getChannelImage(bitplane, (int)ch, myImage.NumCh);
                });

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

            watch.Stop();
            string timeTaken = String.Format(" {0} {1} ms", algorithm.ToString(), watch.ElapsedMilliseconds.ToString());
            Console.WriteLine(timeTaken);
            labelTimeTaken.Content += timeTaken;
        }
        
        /// <summary>
        /// Draw histogram, comulative frequencies and channel images on screen
        /// </summary>
        private void DrawOnScreen(int numCh, Bitmap[] channelImages)
        {
            channelStack.Children.Clear();

            for (int ch = 0; ch < numCh; ch++)
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
                processImage(dialog.FileName, Algorithm.LOAD_IMAGE);
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
            processImage(originalFilename, Algorithm.LOAD_IMAGE);
        }

        private void buttonHE_Click(object sender, RoutedEventArgs e)
        {
            processImage(originalFilename, Algorithm.HE);
        }

        private void buttonAHE_Click(object sender, RoutedEventArgs e)
        {
            processImage(originalFilename, Algorithm.AHE);
        }

        private void buttonCLHE_Click(object sender, RoutedEventArgs e)
        {
            processImage(originalFilename, Algorithm.CLHE);
        }

        private void buttonCLAHE_Click(object sender, RoutedEventArgs e)
        {
            processImage(originalFilename, Algorithm.CLAHE);
        }

        private void imageOriginal_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            saveCurrentImage();
        }
    }
}
