using System.Windows;
using System.Windows.Controls;

namespace Mp3Utilities.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            this.CreateTabs();
        }

        private void CreateTabs()
        {
            var trimmerTab = new TabItem();
            var trimmerView = new TrimmerView();
            trimmerTab.Content = trimmerView;
            trimmerTab.Header = trimmerView.Name;

            this.TabControl.Items.Add(trimmerTab);
        }
    }
}
