using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vaja1_CLAHE
{
    public class MyBitplane
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public byte[,] PixelData { get; set; }

        public MyBitplane(MyBitplane bitplane)
        {
            this.Width = bitplane.Width;
            this.Height = bitplane.Height;

            for (int y = 0; y < this.Height; ++y)
                for (int x = 0; x < this.Width; ++x)
                    SetPixel(x, y, bitplane.GetPixel(x,y));
        }

        public MyBitplane(int w, int h)
        {
            Width = w;
            Height = h;

            PixelData = new byte[Height, Width];
        }

        public byte GetPixel(int x, int y)
        {
            return PixelData[y, x];
        }

        public void SetPixel(int x, int y, byte value)
        {
            PixelData[y, x] = value;
        }
    }
}
