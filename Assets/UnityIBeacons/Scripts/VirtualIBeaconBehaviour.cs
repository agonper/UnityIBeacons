using System;
using IBeacons;
using UnityEngine;
using UnityEngine.UI;

public class VirtualIBeaconBehaviour : MonoBehaviour
{
    public Text beaconTag;
    public Toggle toggle;

    public IBeaconDescriptor Beacon { get; private set; }

    public void SetVirtualIBeacon(IBeaconDescriptor beacon)
    {
        Beacon = beacon;
        DisplayBeaconTag(beacon.tag);
    }

    public void SetOnToggleAction(Action<bool, IBeaconDescriptor> onToggle)
    {
        toggle.onValueChanged.AddListener((active) => onToggle(active, Beacon));
    }

    private void DisplayBeaconTag(string bTag)
    {
        beaconTag.text = bTag;
    }
}
