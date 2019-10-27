using UnityEngine;
using IBeacons;
using IBeacons.Internal;

public class VirtualIBeaconsManager : MonoBehaviour
{
    private static readonly float TIME_BETWEEN_BEACON_CHANGES = 1.0f;

    public GameObject virtualIBeaconsContainer;
    public GameObject virtualIBeaconPrefab;

    private IBeaconsEventSystem _iBeacons;
    private VirtualIBeaconsProvider _virtualIBeaconsProvider;

    private float timeUntilNextBeaconUpdate;

    void Start()
    {
        _iBeacons = IBeaconsEventSystem.Instance;
        _virtualIBeaconsProvider = VirtualIBeaconsProvider.Instance;
        _virtualIBeaconsProvider.OnScanResult += OnProviderScanResult;
        DisplayBeacons();
    }

    void Update()
    {
        timeUntilNextBeaconUpdate -= Time.deltaTime;
        if (timeUntilNextBeaconUpdate > 0) return;

        timeUntilNextBeaconUpdate = TIME_BETWEEN_BEACON_CHANGES;
        _virtualIBeaconsProvider.SimulateVisibleBeaconChanges();
    }

    private void OnProviderScanResult(string encodedBeacons)
    {
        _iBeacons.OnScanResult(encodedBeacons);
    }

    private void DisplayBeacons()
    {
        foreach (var beaconDescriptor in _iBeacons.beaconDescriptors)
        {
            DisplayBeacon(beaconDescriptor);
        }
    }

    private void DisplayBeacon(IBeaconDescriptor beaconDescriptor)
    {
        var virtualIBeaconItem = Instantiate(virtualIBeaconPrefab, virtualIBeaconsContainer.transform)
            .GetComponent<VirtualIBeaconBehaviour>();
        virtualIBeaconItem.SetVirtualIBeacon(beaconDescriptor);
        virtualIBeaconItem.SetOnToggleAction(OnBeaconToogle);
    }

    private void OnBeaconToogle(bool active, IBeaconDescriptor beaconDescriptor)
    {
        if (active)
        {
            _virtualIBeaconsProvider.BeaconAdded(beaconDescriptor);
            return;
        }
        _virtualIBeaconsProvider.BeaconRemoved(beaconDescriptor);
    }
}
