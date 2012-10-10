using System;
using System.Device.Location;

namespace WP71Demo.Util
{
    public delegate void LocationServiceStatusCallback(GeoPositionStatus status);
    public delegate void LocationServiceValueCallback(GeoPositionChangedEventArgs<GeoCoordinate> value);

    /// <summary>
    /// example: http://msdn.microsoft.com/en-us/library/ff431782%28VS.92%29.aspx
    /// </summary>
    public class LocationServiceInfo
    {
        private static LocationServiceInfo _instance = null;

        private static string mGPSlatitude = string.Empty;
        private static string mGPSlongitude = string.Empty;

        private static GeoCoordinateWatcher watcher = null;
        private static bool mIsRunning = false;

        LocationServiceStatusCallback mGPSCallback;
        LocationServiceValueCallback mValueCallback;

        private LocationServiceInfo()
        { 
        }

        public static LocationServiceInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocationServiceInfo();
                }
                return _instance;
            }
        }

        public static string Latitude
        {
            get
            {
                return mGPSlatitude;
            }
        }

        public static string Longitude
        {
            get
            {
                return mGPSlongitude;            
            }
        }

        private void Start()
        {
            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default); // using default accuracy
                watcher.MovementThreshold = 0.4f; // 0.4f use MovementThreshold to ignore noise in the signal
                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                watcher.Start();
                mIsRunning = true;
                System.Diagnostics.Debug.WriteLine("Location service start.");
            }
        }

        /// <summary>
        /// passing callbacks for event handling
        /// </summary>
        /// <param name="callback"></param>
        public void StartFetchLocationData(LocationServiceStatusCallback callback, LocationServiceValueCallback valueCallback = null)
        {
            if (mGPSCallback != null)
            {
                mGPSCallback = null;
            }
            mGPSCallback = callback;

            if (valueCallback != null)
            {
                if (mValueCallback != null)
                {
                    mValueCallback = null;
                }
                mValueCallback = valueCallback;
            }
            if (!IsServiceRunning)
            {
                Start();
            }            
        }

        public void StartLocationService()
        {
            if (!IsServiceRunning)
            {
                Start();
            }
        }

        public bool IsServiceRunning
        {
            get
            {
                return mIsRunning;
            }            
        }

        public bool IsPermissionDenied
        {
            get
            {
                if (watcher == null)
                {
                    return false;
                }
                return (watcher.Permission == GeoPositionPermission.Denied);
            }
        }

        // Event handler for the GeoCoordinateWatcher.StatusChanged event.
        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // The Location Service is disabled or unsupported.
                    // Check to see whether the user has disabled the Location Service.
                    if (watcher.Permission == GeoPositionPermission.Denied)
                    {
                        // The user has disabled the Location Service on their device.
                        System.Diagnostics.Debug.WriteLine("you have disabled this application access to location.");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("location is not functioning on this device");
                    }
                    UpdateStatus(GeoPositionStatus.Disabled);
                    break;

                case GeoPositionStatus.Initializing:
                    // The Location Service is initializing.
                    System.Diagnostics.Debug.WriteLine("The Location Service is initializing.");
                    UpdateStatus(GeoPositionStatus.Initializing);
                    break;

                case GeoPositionStatus.NoData:
                    // The Location Service is working, but it cannot get location data.
                    System.Diagnostics.Debug.WriteLine("The Location Service no data.");
                    UpdateStatus(GeoPositionStatus.NoData);
                    break;

                case GeoPositionStatus.Ready:
                    // The Location Service is working and is receiving location data.
                    System.Diagnostics.Debug.WriteLine("The Location Service is available.");
                    UpdateStatus(GeoPositionStatus.Ready);
                    break;
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            UpdateLocationValue(e);
            mGPSlatitude = e.Position.Location.Latitude.ToString("0.000");
            mGPSlongitude = e.Position.Location.Longitude.ToString("0.000");

            System.Diagnostics.Debug.WriteLine(mGPSlatitude + "-- - " + mGPSlongitude);
        }

        void UpdateStatus(GeoPositionStatus status)
        {
            if (mGPSCallback != null)
            {
                mGPSCallback(status);
            }
        }

        void UpdateLocationValue(GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (mValueCallback != null)
            {
                mValueCallback(e);
            }
        }

        public void Stop()
        {
            if (watcher != null)
            {
                watcher.Stop();
                watcher = null;
                mIsRunning = false;
                mGPSlatitude = string.Empty;
                mGPSlongitude = string.Empty;
                System.Diagnostics.Debug.WriteLine("Location service stop.");
            }
        }


    }
}
