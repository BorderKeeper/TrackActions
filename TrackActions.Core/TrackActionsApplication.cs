using System;
using System.Threading.Tasks;
using TrackActions.Core.TrackIR;

namespace TrackActions.Core
{
    public class TrackActionsApplication
    {
        private TrackIrClient _trackIrClient;

        public string Text { get; set; }

        public TrackActionsApplication()
        {
            _trackIrClient = new TrackIrClient();
        }

        public void Start()
        {
            _trackIrClient.TrackIR_Enhanced_Init();

            Task.Run(() =>
            {
                while (true)
                {
                    Text = _trackIrClient.client_TestTrackIRData();
                }
            });
        }

        public void Shutdown()
        {
            _trackIrClient.TrackIR_Shutdown();
        }
    }
}