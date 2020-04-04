using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
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

        public StartViewModel()
        {
            ConvertButtonClickCommand = new ClickCommand(handleConvertButtonClicked, canClickConvertButton);
            ConvertAsyncButtonClickCommand = new ClickCommand(handleConvertAsyncButtonClicked, canClickConvertButton);
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
            converter.Convert(ImagePath);
            stopwatch.Stop();
            SyncTime = $"{stopwatch.ElapsedMilliseconds}ms";
        }

        private void handleConvertAsyncButtonClicked(object parameter)
        {
            const int THREADS_COUNT = 20;
            ImageConverter converter = new ImageConverter();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            converter.ConvertAsync(ImagePath, THREADS_COUNT).Wait();
            stopwatch.Stop();
            AsyncTime = $"{stopwatch.ElapsedMilliseconds}ms";
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