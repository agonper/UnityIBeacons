using System.Collections.Generic;
using System.Linq;

namespace IBeacons.Internal
{
    public class IBeaconsParser
    {
        private Dictionary<string, IBeaconDescriptor> _beaconDescriptors = new Dictionary<string, IBeaconDescriptor>();

        public IBeaconsParser(List<IBeaconDescriptor> beaconDescriptors)
        {
            foreach (var descriptor in beaconDescriptors)
            {
                var key = $"{descriptor.major}/{descriptor.minor}";
                _beaconDescriptors[key] = descriptor;
            }
        }

        public List<IBeacon> Parse(string scanResult)
        {
            var beacons = new List<IBeacon>();
            var encondedBeacons = ExtractEncodedBeacons(scanResult);

            foreach (var encodedBeacon in encondedBeacons)
            {
                var beaconParts = ExtractBeaconParts(encodedBeacon);
                var beacon = BeaconFromParts(beaconParts);
                beacons.Add(beacon);
            }

            var sortedBeacons = beacons.OrderBy(BeaconDistanceWeight).ToList();
            return sortedBeacons;
        }

        private string[] ExtractEncodedBeacons(string encodedResult)
        {
            return encodedResult.Split(';');
        }

        private string[] ExtractBeaconParts(string encodedBeacon)
        {
            return encodedBeacon.Split('#');
        }

        private IBeacon BeaconFromParts(string[] beaconParts)
        {
            var major = int.Parse(beaconParts[0]);
            var minor = int.Parse(beaconParts[1]);
            var distance = double.Parse(beaconParts[2]);
            var rssi = int.Parse(beaconParts[3]);

            var key = $"{major}/{minor}";
            var tag = _beaconDescriptors.ContainsKey(key) ? _beaconDescriptors[key].tag : key;
            return new IBeacon(tag, major, minor, distance, rssi);
        }

        private double BeaconDistanceWeight(IBeacon beacon)
        {
            return beacon.Distance < 0 ? beacon.Distance * -1000 : beacon.Distance;
        }
    }
}


