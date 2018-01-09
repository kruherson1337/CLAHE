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
        /*
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
                Bitmap[] histograms = new Bitmap[myImage.NumCh];
                Bitmap[] comulativeFrequencies = new Bitmap[myImage.NumCh];
                Bitmap[] channelImages = new Bitmap[myImage.NumCh];

                int AHEWindowSize = int.Parse(textboxAHEWindowSize.Text);
                MyBitplane bitplane = myImage.Bitplane[0];
                ImageProcessing.processImage(ref bitplane, algorithm, AHEWindowSize);

                // Calculate Histogram
                double[] histogram = ImageProcessing.calculateHistogram(bitplane);

                // Get images
                channelImages[0] = ImageRendering.getChannelImage(bitplane, (int)0, myImage.NumCh);
                histograms[0] = ImageRendering.drawHistogram(histogram);
                comulativeFrequencies[0] = ImageRendering.drawComulativeFrequency(ImageProcessing.calculateComulativeFrequency(histogram));

                myImage.Bitplane[0] = bitplane;

                // Draw histogram and image for each channel
                DrawOnScreen(1, histograms, comulativeFrequencies, channelImages);

                // Draw image on screen
                imageOriginal.Source = Utils.getSource(myImage.GetBitmap());
            }

            watch.Stop();
            string timeTaken = String.Format(" {0} {1} ms", algorithm.ToString(), watch.ElapsedMilliseconds.ToString());
            Console.WriteLine(timeTaken);
            labelTimeTaken.Content += timeTaken;
        }
        */
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
                Bitmap[] histograms = new Bitmap[myImage.NumCh];
                Bitmap[] comulativeFrequencies = new Bitmap[myImage.NumCh];
                Bitmap[] channelImages = new Bitmap[myImage.NumCh];

                int windowSize = int.Parse(textboxAHEWindowSize.Text);
                double clipLimit = Double.Parse(textboxClipLimit.Text);

                Parallel.ForEach(myImage.Bitplane, (bitplane, state, ch) =>
                {
                    ImageProcessing.processImage(ref bitplane, algorithm, windowSize, clipLimit);

                    // Calculate Histogram
                    double[] histogram = ImageProcessing.calculateHistogram(bitplane);

                    // Get images
                    channelImages[ch] = ImageRendering.getChannelImage(bitplane, (int)ch, myImage.NumCh);
                    histograms[ch] = ImageRendering.drawHistogram(histogram);
                    comulativeFrequencies[ch] = ImageRendering.drawComulativeFrequency(ImageProcessing.calculateComulativeFrequency(histogram));
                });

                // Draw histogram and image for each channel
                DrawOnScreen(myImage.NumCh, histograms, comulativeFrequencies, channelImages);

                // Draw image on screen
                imageOriginal.Source = Utils.getSource(myImage.GetBitmap());
            }

            watch.Stop();
            string timeTaken = String.Format(" {0} {1} ms", algorithm.ToString(), watch.ElapsedMilliseconds.ToString());
            Console.WriteLine(timeTaken);
            labelTimeTaken.Content += timeTaken;
        }
        
        private void DrawOnScreen(int numCh, Bitmap[] histograms, Bitmap[] comulativeFrequencies, Bitmap[] channelImages)
        {
            channelStack.Children.Clear();
            histogramStack.Children.Clear();
            stackComulativeFrequency.Children.Clear();

            for (int ch = 0; ch < numCh; ch++)
            {
                histogramStack.Children.Add(Utils.GetImageView(histograms[ch], histogramStack.Width / 3, histogramStack.Height));
                histograms[ch].Dispose();

                channelStack.Children.Add(Utils.GetImageView(channelImages[ch], channelStack.Width / 3, channelStack.Height));
                channelImages[ch].Dispose();

                stackComulativeFrequency.Children.Add(Utils.GetImageView(comulativeFrequencies[ch], stackComulativeFrequency.Width / 3, stackComulativeFrequency.Height));
                comulativeFrequencies[ch].Dispose();
            }
        }

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
