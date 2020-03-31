using System.Drawing;

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
                    setPixel(bitmap, i, j);

            bitmap.Save("jea.png");
        }

        void setPixel(Bitmap bitmap, int x, int y)
        {
            Color originalColor = bitmap.GetPixel(x, y);
            int r = (int)(originalColor.R * RED_FACTOR);
            int g = (int)(originalColor.G * GREEN_FACTOR);
            int b = (int)(originalColor.B * BLUE_FACTOR);
            int val = (r + g + b) % 255;

            Color newColor = Color.FromArgb(255, val, val, val);
            bitmap.SetPixel(x, y, newColor);
        }
    }
}
