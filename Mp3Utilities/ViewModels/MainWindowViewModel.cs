using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using AndyTools.Wpf;
using Mp3Utilities.Views;

namespace Mp3Utilities.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<TabItem> TabItems { get; set; }

        public MainWindowViewModel()
        {
            this.LoadTabs();
        }

        private void LoadTabs()
        {
            this.TabItems = new ObservableCollection<TabItem>();

            var trimmerView = new TrimmerView();
            this.TabItems.Add(new TabItem { Header = "Trimmer", Content = trimmerView});

            var trackOrderView = new TrackOrderView();
            this.TabItems.Add(new TabItem { Header = "Track Order", Content = trackOrderView });
        }
    }
}
