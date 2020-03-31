using System.Windows;

namespace WpfImhApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StartViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new StartViewModel();
            DataContext = viewModel;
        }
    }
}