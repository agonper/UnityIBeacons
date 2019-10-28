using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBeacons.Internal;

namespace IBeacons
{
    public class IBeaconsEventSystem : MonoBehaviour
    {
        public string appUUID;
        public List<IBeaconDescriptor> beaconDescriptors = new List<IBeaconDescriptor>();
        public int beaconLostTimeout;

        public static IBeaconsEventSystem Instance { get; private set; }

        public event Action<IBeacon> OnBeaconFound
        {
            add => AddEventListener(Event.Found, value);
            remove => RemoveEventListener(Event.Found, value);
        }
        public event Action<IBeacon> OnBeaconChanged
        {
            add => AddEventListener(Event.Changed, value);
            remove => RemoveEventListener(Event.Changed, value);
        }
        public event Action<IBeacon> OnBeaconLost
        {
            add => AddEventListener(Event.Lost, value);
            remove => RemoveEventListener(Event.Lost, value);
        }
        public event Action<string> OnError;

        public List<IBeacon> Beacons { get => new List<IBeacon>(_beacons.Values); }

        private IBeaconsProvider _beaconsProvider;
        private IBeaconsParser _beaconsParser;

        private Dictionary<string, IBeacon> _beacons = new Dictionary<string, IBeacon>();
        private Coroutine _lostBeaconWatcher;
        private bool _scanRunning;

        private event Action<IBeacon> _onBeaconFound, _onBeaconChanged, _onBeaconLost;
        private int _listenersCounter;

        void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void OnScanResult(string encodedResult)
        {
            if (!_scanRunning) return;
            var beacons = _beaconsParser.Parse(encodedResult);
            foreach (var beacon in beacons)
            {
                if (!_beacons.ContainsKey(beacon.Tag))
                {
                    _beacons[beacon.Tag] = beacon;
                    Notify(_onBeaconFound, beacon);
                } else
                {
                    var updatedBeacon = _beacons[beacon.Tag].Update(beacon);
                    Notify(_onBeaconChanged, updatedBeacon);
                }
            }
        }

        public void OnProviderFailure(string error)
        {
            OnError?.Invoke(error);
        }

        #region private
        private void Initialize()
        {
            Instance = this;
            _beaconsProvider = IBeaconsProviderFactory.CreateProvider(appUUID);
            _beaconsParser = new IBeaconsParser(beaconDescriptors);
        }

        private void AddEventListener(Event evt, Action<IBeacon> listener)
        {
            lock (this)
            {
                switch (evt)
                {
                    case Event.Found:
                        _onBeaconFound += listener;
                        break;
                    case Event.Changed:
                        _onBeaconChanged += listener;
                        break;
                    case Event.Lost:
                        _onBeaconLost += listener;
                        break;
                }

                if (_listenersCounter == 0)
                {
                    StartScan();
                }
                _listenersCounter++;
            }
        }

        private void RemoveEventListener(Event evt, Action<IBeacon> listener)
        {
            lock (this)
            {
                switch (evt)
                {
                    case Event.Found:
                        _onBeaconFound -= listener;
                        break;
                    case Event.Changed:
                        _onBeaconChanged -= listener;
                        break;
                    case Event.Lost:
                        _onBeaconLost -= listener;
                        break;
                }

                if (_listenersCounter == 1)
                {
                    StopScan();
                }
                _listenersCounter--;
            }
        }

        private void StartScan()
        {
            if (_scanRunning) return;
            _scanRunning = true;
            _beacons.Clear();
            _beaconsProvider.StartScan();
            StartWatchingLostBeacons();
        }

        private void StopScan()
        {
            if (!_scanRunning) return;
            _scanRunning = false;
            _beaconsProvider.StopScan();
            StopWatchingLostBeacons();
        }

        private void StartWatchingLostBeacons()
        {
            _lostBeaconWatcher = StartCoroutine(LostBeaconWatcher());
        }

        private void StopWatchingLostBeacons()
        {
            StopCoroutine(_lostBeaconWatcher);
            _lostBeaconWatcher = null;
        }

        private IEnumerator LostBeaconWatcher()
        {
            while (_scanRunning)
            {
                yield return new WaitForSeconds(1);
                var now = DateTime.Now;
                foreach (var beacon in Beacons)
                {
                    var timeDiff = now - beacon.LastSeen;
                    if (timeDiff.TotalSeconds > beaconLostTimeout)
                    {
                        _beacons.Remove(beacon.Tag);
                        Notify(_onBeaconLost, beacon);
                    }
                }
            }
        }

        private void Notify(Action<IBeacon> action, IBeacon message)
        {
            action?.Invoke(message);
        }

        private enum Event
        {
            Found, Changed, Lost
        }
        #endregion
    }
}