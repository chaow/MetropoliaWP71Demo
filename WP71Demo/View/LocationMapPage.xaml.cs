using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using System.Net.NetworkInformation;
using WP71Demo.Util;

namespace WP71Demo.View
{
    public partial class LocationMapPage : PhoneApplicationPage
    {
        public LocationMapPage()
        {
            InitializeComponent();
            Loaded += LocationMapPage_Loaded;
        }

        void LocationMapPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MapDemo.Visibility == System.Windows.Visibility.Visible)
            {
                // start location service in background
                LocationService.Instance.StartFetchLocationData(LocationServiceStatusCallback, LocationServiceValueCallback);
                // enable zoom bar
                MapDemo.ZoomBarVisibility = System.Windows.Visibility.Visible;
                MapDemo.ZoomLevel = 15;
            }
        }

        // get the broadcast event
        private void LocationServiceStatusCallback(GeoPositionStatus status)
        {
            switch (status)
            {
                case GeoPositionStatus.Disabled:
                    break;
                case GeoPositionStatus.Initializing:
                    break;
                case GeoPositionStatus.NoData:
                    break;
                case GeoPositionStatus.Ready:
                    break;
            }
        }

        private void LocationServiceValueCallback(GeoPositionChangedEventArgs<GeoCoordinate> value)
        {
            MapDemo.Children.Clear();
            MapDemo.Center = new GeoCoordinate(value.Position.Location.Latitude, value.Position.Location.Longitude);
            Pushpin pin = new Pushpin();
            pin.Location = new GeoCoordinate(value.Position.Location.Latitude, value.Position.Location.Longitude);
            MapDemo.Children.Add(pin);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                //MapDemo.Visibility = System.Windows.Visibility.Collapsed;
                System.Diagnostics.Debug.WriteLine("You do not have Internet connectivity.");
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (LocationService.Instance.IsServiceRunning)
            {
                // stop location service in background
                LocationService.Instance.Stop();
            }
            base.OnNavigatedFrom(e);
        }

    }
}