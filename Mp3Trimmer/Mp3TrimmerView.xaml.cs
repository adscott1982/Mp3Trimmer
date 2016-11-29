namespace Mp3Trimmer
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for Mp3TrimmerView.xaml
    /// </summary>
    public partial class Mp3TrimmerView : Window
    {
        public Mp3TrimmerViewViewModel ViewModel { get; }

        public Mp3TrimmerView()
        {
            this.InitializeComponent();
            this.ViewModel = new Mp3TrimmerViewViewModel();
            this.DataContext = this.ViewModel;
        }
    }
}
