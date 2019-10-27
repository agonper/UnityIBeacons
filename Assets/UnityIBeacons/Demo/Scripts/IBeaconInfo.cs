using UnityEngine;
using UnityEngine.UI;

namespace IBeacons.Demo
{
    public class IBeaconInfo : MonoBehaviour
    {
        public Text beaconTag,
            beaconMajor, beaconMinor,
            beaconDistance, beaconSignal;

        public void SetBeacon(IBeacon beacon)
        {
            DisplayBeaconTag(beacon.Tag);
            DisplayBeaconMajor(beacon.Major);
            DisplayBeaconMinor(beacon.Minor);
            DisplayBeaconDistance(beacon.Distance);
            DisplayBeaconSignal(beacon.Rssi);
        }

        private void DisplayBeaconTag(string bTag)
        {
            beaconTag.text = $"Tag: {bTag}";
        }

        private void DisplayBeaconMajor(int major)
        {
            beaconMajor.text = $"Major: {major}";
        }

        private void DisplayBeaconMinor(int minor)
        {
            beaconMinor.text = $"Minor: {minor}";
        }

        private void DisplayBeaconDistance(double distance)
        {
            beaconDistance.text = $"Distance: {distance:F2}m";
        }

        private void DisplayBeaconSignal(int rssi)
        {
            beaconSignal.text = $"Rssi: {rssi} dBm";
        }
    }
}