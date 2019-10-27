using System;

namespace IBeacons.Internal
{
    public static class IBeaconsProviderFactory
    {
        public static IBeaconsProvider CreateProvider(string appUUID)
        {
#if UNITY_EDITOR
            return VirtualIBeaconsProvider.Instance;
#elif UNITY_ANDROID
            return new AndroidIBeaconsProvider(appUUID);
#elif UNITY_IOS
            return new IOSIBeaconsProvider(appUUID);
#else
            throw new InvalidOperationException("There is no nearby provider implementation for your platform");
#endif
        }
    }
}

