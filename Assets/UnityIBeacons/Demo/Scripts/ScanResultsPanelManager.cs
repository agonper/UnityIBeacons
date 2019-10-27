using System.Collections.Generic;
using UnityEngine;
using IBeacons;
using IBeacons.Demo;

public class ScanResultsPanelManager : MonoBehaviour
{

    public GameObject scanResultsPanel;
    public GameObject beaconsContainer;
    public GameObject beaconInfoPrefab;

    private IBeaconsEventSystem _iBeacons;
    public Dictionary<string, GameObject> _beaconInfoPrefabs = new Dictionary<string, GameObject>();

    public void Show()
    {
        _iBeacons = IBeaconsEventSystem.Instance;
        _iBeacons.OnBeaconFound += RenderBeacon;
        _iBeacons.OnBeaconChanged += UpdateBeacon;
        _iBeacons.OnBeaconLost += RemoveBeacon;
        DisplayBeacons(_iBeacons.Beacons);
        scanResultsPanel.SetActive(true);
    }

    public void Hide()
    {
        scanResultsPanel.SetActive(false);
        _iBeacons.OnBeaconFound -= RenderBeacon;
        _iBeacons.OnBeaconChanged -= UpdateBeacon;
        _iBeacons.OnBeaconLost -= RemoveBeacon;
    }

    private void DisplayBeacons(List<IBeacon> beacons)
    {
        ClearBeacons();
        foreach (var beacon in beacons)
        {
            RenderBeacon(beacon);
        }
    }

    private void RenderBeacon(IBeacon beacon)
    {
        var prefabInstance = Instantiate(beaconInfoPrefab, beaconsContainer.transform);
        _beaconInfoPrefabs.Add(beacon.Tag, prefabInstance);

        UpdateBeacon(beacon);
    }

    private void UpdateBeacon(IBeacon beacon)
    {
        var beaconInfo = _beaconInfoPrefabs[beacon.Tag];
        beaconInfo.GetComponent<IBeaconInfo>().SetBeacon(beacon);
    }

    private void RemoveBeacon(IBeacon beacon)
    {
        var beaconInfo = _beaconInfoPrefabs[beacon.Tag];
        Destroy(beaconInfo);
        _beaconInfoPrefabs.Remove(beacon.Tag);
    }

    private void ClearBeacons()
    {
        foreach (var prefab in _beaconInfoPrefabs)
        {
            Destroy(prefab.Value);
        }
        _beaconInfoPrefabs.Clear();
    }
}
