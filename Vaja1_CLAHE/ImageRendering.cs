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
    }
}
