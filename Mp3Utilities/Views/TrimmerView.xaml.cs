using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
