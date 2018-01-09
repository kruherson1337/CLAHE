using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Vaja1_CLAHE
{
    public class MyImage
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int NumCh { get; set; }
        public string ImageFileName { get; set; }

        public List<MyBitplane> Bitplane = new List<MyBitplane>();

        public MyImage(Bitmap bmp)
        {             
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                  ImageLockMode.ReadOnly, bmp.PixelFormat);

             switch (bmp.PixelFormat)
             {
                 case PixelFormat.Format8bppIndexed: NumCh = 1; break;
                 case PixelFormat.Format16bppGrayScale: NumCh = 2; break;
                 case PixelFormat.Format24bppRgb: NumCh = 3; break;
                 case PixelFormat.Format32bppArgb: NumCh = 4; break;
                 default: NumCh = 1; break;
             }

             byte[] pixels = new byte[bmp.Width * bmp.Height * NumCh];
             Marshal.Copy(bd.Scan0, pixels, 0, pixels.Length);
             bmp.UnlockBits(bd);

             Width = bmp.Width;
             Height = bmp.Height;

             for (int i = 0; i < NumCh; i++)
                 Bitplane.Add(new MyBitplane(Width, Height));

             int pos = 0;
             for (int j = 0; j < Height; ++j)
                 for (int i = 0; i < Width; ++i)
                     for (int ch = 0; ch < NumCh; ++ch)                     
                         Bitplane[ch].SetPixel(i, j, pixels[pos++]);
                     

             bmp.Dispose();
        }

        public MyImage(int w, int h, int ch)
        {
            NumCh = ch;
            Width = w;
            Height = h;
            ImageFileName = "";

            for (int i = 0; i < NumCh; i++)
                Bitplane.Add(new MyBitplane(Width, Height));
        }

        public Bitmap GetBitmap()
        {
            Bitmap bmp;
            switch (NumCh)
            {
                case 1: bmp = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed); break;
                case 2: bmp = new Bitmap(Width, Height, PixelFormat.Format16bppGrayScale); break;
                case 3: bmp = new Bitmap(Width, Height, PixelFormat.Format24bppRgb); break;
                case 4: bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb); break;
                default: bmp = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed); break;
            }
            byte[] pixels = new byte[Width * Height * NumCh];

            int pos = 0;
            for (int y = 0; y < Height; ++y)
                for (int x = 0; x < Width; ++x)
                    for (int ch = 0; ch < NumCh; ++ch)
                        pixels[pos++] = Bitplane[ch].GetPixel(x, y);


            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

            Marshal.Copy(pixels, 0, bd.Scan0, pixels.Length);

            bmp.UnlockBits(bd);

            return bmp;
        }
    }
}
