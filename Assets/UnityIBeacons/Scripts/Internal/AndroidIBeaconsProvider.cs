using System;
using UnityEngine;
using UnityEngine.Android;

namespace IBeacons.Internal
{
    public class AndroidIBeaconsProvider : IBeaconsProvider
    {
        private string _appUUID;
        private AndroidJavaObject _iBeacons;

        public AndroidIBeaconsProvider(string appUUID)
        {
            _appUUID = appUUID;
            Init();
        }

        public void StartScan()
        {
            Init();
            _iBeacons?.Call("startScan");
        }

        public void StopScan()
        {
            _iBeacons?.Call("stopScan");
        }

        private void Init()
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
                return;
            }
            if (_iBeacons != null) return;
            _iBeacons = new AndroidJavaObject("es.agonper.androiduib.UnityIBeacons", _appUUID);
        }
    }
}

