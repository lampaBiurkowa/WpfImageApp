using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfImhApp
{
    public class StartViewModel : INotifyPropertyChanged
    {
        public ICommand ConvertButtonClickCommand { get; set; }
        public ICommand ConvertAsyncButtonClickCommand { get; set; }
        public ICommand SelectButtonClickCommand { get; set; }

        private bool inited = false;
        public bool Inited
        {
            get { return inited; }
            set
            {
                inited = value;
                OnPropertyChange(nameof(Inited));
            }
        }

        private string imagePath;

        public string ImagePath
        {
            get { return imagePath; }
            set
            {
                imagePath = value;
                OnPropertyChange(nameof(ImagePath));
            }
        }

        private string asyncTime;

        public string AsyncTime
        {
            get { return asyncTime; }
            set
            {
                asyncTime = value;
                OnPropertyChange(nameof(AsyncTime));
            }
        }

        private string syncTime;

        public string SyncTime
        {
            get { return syncTime; }
            set
            {
                syncTime = value;
                OnPropertyChange(nameof(SyncTime));
            }
        }

        private string heightMask = "100";

        public string HeightMask
        {
            get { return heightMask; }
            set
            {
                if (new Regex("[^0-9]+").IsMatch(value) || value.ToString().StartsWith("0") || value.ToString().Length == 0)
                    return;

                heightMask = value;
                OnPropertyChange(nameof(HeightMask));
            }
        }

        public int Height => int.Parse(HeightMask);

        private string widthMask = "100";

        public string WidthMask
        {
            get { return widthMask; }
            set
            {
                if (new Regex("[^0-9]+").IsMatch(value) || value.ToString().StartsWith("0") || value.ToString().Length == 0)
                    return;

                widthMask = value;
                OnPropertyChange(nameof(WidthMask));
            }
        }

        public int Width => int.Parse(WidthMask);

        public StartViewModel()
        {
            ConvertButtonClickCommand = new ClickCommand(handleConvertButtonClicked, canClickConvertButton);
            ConvertAsyncButtonClickCommand = new ClickCommandAsync(handleConvertAsyncButtonClicked, canClickConvertButton);
            SelectButtonClickCommand = new ClickCommand(handleSelectButtonClicked, canClickSelectButton);
        }

        private bool canClickConvertButton(object parameter)
        {
            return Inited;
        }

        private void handleConvertButtonClicked(object parameter)
        {
            ImageConverter converter = new ImageConverter();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Bitmap bitmap = converter.Convert(ImagePath);
            stopwatch.Stop();
            SyncTime = $"{stopwatch.ElapsedMilliseconds}ms";
            bitmap = converter.Resize(bitmap, Width, Height);
            bitmap.Save("jea.png");
        }

        private async Task handleConvertAsyncButtonClicked()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Bitmap bitmap = await getGrayscaleBitmapAsync();
            stopwatch.Stop();
            AsyncTime = $"{stopwatch.ElapsedMilliseconds}ms";
            ImageConverter converter = new ImageConverter();
            bitmap = converter.Resize(bitmap, Width, Height);
            bitmap.Save("jeaasync.png");
        }

        private async Task<Bitmap> getGrayscaleBitmapAsync()
        {
            ImageConverter converter = new ImageConverter();
            return await Task.Factory.StartNew(() => converter.ConvertParallelly(ImagePath));
        }

        private bool canClickSelectButton(object parameter)
        {
            return true;
        }

        private void handleSelectButtonClicked(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (!(bool)openFileDialog.ShowDialog())
                return;

            ImagePath = openFileDialog.FileName;
            Inited = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}