using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace WpfImhApp
{
    class ImageConverter
    {
        /* Operating on pixels inspired by L.B 
        * https://stackoverflow.com/questions/21497537/allow-an-image-to-be-accessed-by-several-threads */

        const float RED_FACTOR = 0.3f;
        const float GREEN_FACTOR = 0.58f;
        const float BLUE_FACTOR = 0.11f;

        public Bitmap Convert(string sourcePath)
        {
            Bitmap bitmap = new Bitmap(sourcePath);
            int w = bitmap.Width, h = bitmap.Height;

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            int bytesPerPixel = Image.GetPixelFormatSize(data.PixelFormat) / 8;
            byte[] buffer = new byte[w * h * bytesPerPixel];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

            for (int j = 0; j < h; j++)
                convertRow(buffer, j, w, h, bytesPerPixel);

            Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);
            bitmap.UnlockBits(data);
            return bitmap;
        }

        private void convertRow(byte[] data, int y, int w, int h, int d)
        {
            if (y >= h)
                return;

            for (int i = 0; i < w; i++)
            {
                int index = (y * w + i) * d;
                Color colorful = Color.FromArgb(255, data[index], data[index + 1], data[index + 2]);
                Color grayScale = getGrayscaleColor(colorful);
                data[index] = grayScale.R;
                data[index + 1] = grayScale.G;
                data[index + 2] = grayScale.B;
                data[index + 3] = 255;
            }
        }

        Color getGrayscaleColor(Color originalColor)
        {
            int r = (int)(originalColor.R * RED_FACTOR);
            int g = (int)(originalColor.G * GREEN_FACTOR);
            int b = (int)(originalColor.B * BLUE_FACTOR);
            int val = (r + g + b) % 255;

            return Color.FromArgb(255, val, val, val);
        }

        public Bitmap ConvertParallelly(string sourcePath)
        {
            Bitmap bitmap = new Bitmap(sourcePath);
            int w = bitmap.Width, h = bitmap.Height;

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            int bytesPerPixel = Image.GetPixelFormatSize(data.PixelFormat) / 8;
            byte[] buffer = new byte[w * h * bytesPerPixel];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

            Parallel.ForEach(Enumerable.Range(0, h), i => convertRow(buffer, i, w, h, bytesPerPixel));

            Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);
            bitmap.UnlockBits(data);
            return bitmap;
        }

        public Bitmap Resize(Bitmap bitmap, int wScale, int hScale)
        {
            return new Bitmap(bitmap, new Size((int)(bitmap.Width * (wScale / 100f)), (int)(bitmap.Height * (hScale / 100f))));
        }
    }
}
