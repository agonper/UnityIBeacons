using System;
using System.Collections.Generic;
using System.Text;

namespace IBeacons.Internal
{
    public class VirtualIBeaconsProvider : IBeaconsProvider
    {
        public static VirtualIBeaconsProvider Instance { get; } = new VirtualIBeaconsProvider();

        public event Action<string> OnScanResult;

        private Dictionary<string, IBeacon> _beacons = new Dictionary<string, IBeacon>();
        private bool _scanRunning;
        private Random _random = new Random();


        private VirtualIBeaconsProvider() { }

        public void StartScan()
        {
            if (_scanRunning) return;
            _scanRunning = true;
            NotifyScanResulted();
        }

        public void StopScan()
        {
            if (!_scanRunning) return;
            _scanRunning = false;
        }

        internal void BeaconAdded(IBeaconDescriptor beaconDescriptor)
        {
            var beacon = new IBeacon(beaconDescriptor);
            UpdateOldBeacon(beacon);
            _beacons[beacon.Tag] = beacon;
            NotifyScanResulted();
        }

        internal void BeaconRemoved(IBeaconDescriptor beaconDescriptor)
        {
            var beacon = _beacons[beaconDescriptor.tag];
            _beacons.Remove(beacon.Tag);
            NotifyScanResulted();
        }

        internal void SimulateVisibleBeaconChanges()
        {
            foreach (var entry in _beacons)
            {
                UpdateOldBeacon(entry.Value);
            }
            NotifyScanResulted();
        }

        private void NotifyScanResulted()
        {
            if (_beacons.Count == 0) return;
            var encodedBeacons = EncodeBeacons();
            Notify(OnScanResult, encodedBeacons);
        }

        private string EncodeBeacons()
        {
            var beacons = new List<IBeacon>(_beacons.Values);
            var lastIdx = beacons.Count-1;

            var encodedBeacons = new StringBuilder();
            for (int i = 0; i < lastIdx; i++)
            {
                encodedBeacons.Append(EncodeBeacon(beacons[i]) + ";");
            }
            encodedBeacons.Append(EncodeBeacon(beacons[lastIdx]));
            return encodedBeacons.ToString();
        }

        private string EncodeBeacon(IBeacon beacon)
        {
            return $"{beacon.Major}#{beacon.Minor}#{beacon.Distance}#{beacon.Rssi}";
        }

        private void Notify(Action<string> action, string encodedMessage)
        {
            if (!_scanRunning) return;
            action?.Invoke(encodedMessage);
        }

        private void UpdateOldBeacon(IBeacon oldBeacon)
        {
            var newDistance = _random.NextDouble() * 500;
            var newSignal = _random.Next(-127, 127);
            oldBeacon.Update(newDistance, newSignal);
        }
    }
}