using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TrackActions.Core.TrackIR;
using TrackActions.UI.Annotations;

namespace TrackActions.UI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ICommand Update { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly TrackIrClient _client;

        public string TrackIrOutput => _client.client_TestTrackIRData();

        public MainWindowViewModel()
        {
            _client = new TrackIrClient();

            _client.TrackIR_Enhanced_Init();
        }

        public void OnShutdown()
        {
            _client.TrackIR_Shutdown();
        }

        private void UpdateTrackIr(object sender, ExecutedRoutedEventArgs e)
        {
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}