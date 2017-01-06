using System.Windows.Controls;

namespace Mp3Utilities.Views
{
    /// <summary>
    /// Interaction logic for TrimmerView.xaml
    /// </summary>
    public partial class TrimmerView : UserControl
    {
        public ViewModels.TrimmerViewModel ViewModel { get; }

        public TrimmerView()
        {
            InitializeComponent();
            this.ViewModel = new ViewModels.TrimmerViewModel();
            this.DataContext = this.ViewModel;
        }
    }
}
