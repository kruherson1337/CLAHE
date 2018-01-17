namespace Vaja1_CLAHE
{
    public class Parameters
    {
        public string filename { get; set; }
        public Algorithm algorithm { get; set; }
        public double contrastLimit { get; set; }
        public int windowSize { get; set; }

        public Parameters(string filename, Algorithm algorithm, double contrastLimit, int windowSize)
        {
            this.filename = filename;
            this.algorithm = algorithm;
            this.contrastLimit = contrastLimit;
            this.windowSize = windowSize;
        }

    }
}
