using System.Runtime.InteropServices;

namespace IBeacons.Internal
{
    public class IOSIBeaconsProvider : IBeaconsProvider
    {
        [DllImport("__Internal")]
        private static extern void _uib_initialize(string appUID);

        [DllImport("__Internal")]
        private static extern void _uib_startScan();

        [DllImport("__Internal")]
        private static extern void _uib_stopScan();

        public IOSIBeaconsProvider(string appUUID)
        {
            _uib_initialize(appUUID);
        }

        public void StartScan()
        {
            _uib_startScan();
        }

        public void StopScan()
        {
            _uib_stopScan();
        }
    }
}
