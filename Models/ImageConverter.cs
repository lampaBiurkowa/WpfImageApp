using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace WpfImhApp
{
    class ImageConverter
    {
        const float RED_FACTOR = 0.3f;
        const float GREEN_FACTOR = 0.58f;
        const float BLUE_FACTOR = 0.11f;

        public void Convert(string sourcePath)
        {
            Bitmap bitmap = new Bitmap(sourcePath);
            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                    bitmap.SetPixel(i, j, getGrayscaleColor(bitmap.GetPixel(i, j)));

            bitmap.Save("jea.png");
        }

        Color getGrayscaleColor(Color originalColor)
        {
            int r = (int)(originalColor.R * RED_FACTOR);
            int g = (int)(originalColor.G * GREEN_FACTOR);
            int b = (int)(originalColor.B * BLUE_FACTOR);
            int val = (r + g + b) % 255;

            return Color.FromArgb(255, val, val, val);
        }

        /* Inspired by L.B 
        * https://stackoverflow.com/questions/21497537/allow-an-image-to-be-accessed-by-several-threads */
        public async Task AConvert(string sourcePath)
        {
            Bitmap bitmap = new Bitmap(sourcePath);
            int w = bitmap.Width;
            int h = bitmap.Height;
            Rectangle rect = new Rectangle(0, 0, w, h);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            int bytesPerPixel = Image.GetPixelFormatSize(data.PixelFormat) / 8;
            byte[] buffer = new byte[data.Width * data.Height * bytesPerPixel];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
            
            for (int i = 0; i < bitmap.Height; i += 4)
            {
                Task t1 = Task.Factory.StartNew(() => aConvertRow(buffer, i, w, h));
                Task t2 = Task.Factory.StartNew(() => aConvertRow(buffer, i + 1, w, h));
                Task t3 = Task.Factory.StartNew(() => aConvertRow(buffer, i + 2, w, h));
                Task t4 = Task.Factory.StartNew(() => aConvertRow(buffer, i + 3, w, h));
                Task.WaitAll(t1, t2, t3, t4);
            }
            Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);
            bitmap.UnlockBits(data);
            bitmap.Save("jeaa2.png");
        }

        private void aConvertRow(byte[] data, int y, int w, int h)
        {
            if (y >= h)
                return;

            for (int i = 0; i < w; i += 4)
            {
                Color colorful = Color.FromArgb(255, data[y * w + i], data[y * w + i + 1], data[y * w + i + 2]);
                Color grayScale = getGrayscaleColor(colorful);
                data[y * w + i] = grayScale.R;
                data[y * w + i + 1] = grayScale.G;
                data[y * w + i + 2] = grayScale.B;
            }
        }
    }
}
