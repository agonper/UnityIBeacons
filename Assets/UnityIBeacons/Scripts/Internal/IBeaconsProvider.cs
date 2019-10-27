namespace IBeacons.Internal
{
    public interface IBeaconsProvider
    {
        void StartScan();
        void StopScan();
    }
}