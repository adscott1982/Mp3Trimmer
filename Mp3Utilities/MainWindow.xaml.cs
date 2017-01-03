using System.Windows;
using System.Windows.Controls;

namespace Mp3Utilities
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
            this.CreateTabs();
            this.CreateTabs();
            this.CreateTabs();

        }

        private void CreateTabs()
        {
            var trimmerTab = new TabItem();
            var trimmerView = new Mp3Trimmer.TrimmerView();
            trimmerTab.Content = trimmerView;
            trimmerTab.Header = trimmerView.Name;

            this.TabControl.Items.Add(trimmerTab);
        }
    }
}
