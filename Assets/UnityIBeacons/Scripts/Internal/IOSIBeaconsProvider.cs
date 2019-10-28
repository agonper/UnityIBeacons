using System.Runtime.InteropServices;

namespace IBeacons.Internal
{
    public class IOSIBeaconsProvider : IBeaconsProvider
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void _uib_initialize(string appUID);

        [DllImport("__Internal")]
        private static extern void _uib_startScan();

        [DllImport("__Internal")]
        private static extern void _uib_stopScan();
#endif

        public IOSIBeaconsProvider(string appUUID)
        {
#if UNITY_IOS
            _uib_initialize(appUUID);
#endif
        }

        public void StartScan()
        {
#if UNITY_IOS
            _uib_startScan();
#endif
        }

        public void StopScan()
        {
#if UNITY_IOS
            _uib_stopScan();
#endif
        }
    }
}
