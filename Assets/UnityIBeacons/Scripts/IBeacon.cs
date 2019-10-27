using System;

namespace IBeacons
{
    [Serializable]
    public class IBeacon
    {
        public string Tag { get; }
        public int Major { get; }
        public int Minor { get; }
        public double Distance { get; private set; }
        public int Rssi { get; private set; }
        public DateTime LastSeen { get; private set; }

        public IBeacon(string tag, int major, int minor)
        {
            Tag = tag;
            Major = major;
            Minor = minor;
            LastSeen = DateTime.Now;
        }

        public IBeacon(string tag, int major, int minor, double distance, int rssi): this(tag, major, minor)
        {
            Distance = distance;
            Rssi = rssi;
        }

        public IBeacon(IBeaconDescriptor beacon): this(beacon.tag, beacon.major, beacon.minor) { }

        public IBeacon Update(IBeacon beacon)
        {
            if (Tag != beacon.Tag) return this;
            Distance = beacon.Distance;
            Rssi = beacon.Rssi;
            LastSeen = beacon.LastSeen;
            return this;
        }

        internal IBeacon Update(double distance, int rssi)
        {
            Distance = distance;
            Rssi = rssi;
            LastSeen = DateTime.Now;
            return this;
        }
    }
}