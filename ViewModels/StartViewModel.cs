using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace WpfImhApp
{
    public class StartViewModel : INotifyPropertyChanged
    {
        public ICommand ConvertButtonClickCommand { get; set; }
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

        public StartViewModel()
        {
            ConvertButtonClickCommand = new ClickCommand(handleConvertButtonClicked, canClickConvertButton);
            SelectButtonClickCommand = new ClickCommand(handleSelectButtonClicked, canClickSelectButton);
        }

        private bool canClickConvertButton(object parameter)
        {
            return Inited;
        }

        private void handleConvertButtonClicked(object parameter)
        {
            ImageConverter converter = new ImageConverter();
            converter.Convert(ImagePath);
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