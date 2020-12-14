
using System.Windows;
using TrackActions.UI.ViewModels;

namespace TrackActions.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
        }
    }
}
