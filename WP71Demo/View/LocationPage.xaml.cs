using System.Device.Location;
using System.Net.NetworkInformation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using WP71Demo.Util;

namespace WP71Demo.View
{
    public partial class LocationPage : PhoneApplicationPage
    {
        LocationServiceValueCallback callback;
        public LocationPage()
        {
            InitializeComponent();
            this.Loaded += new System.Windows.RoutedEventHandler(LocationPage_Loaded);
        }

        void LocationPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MapDemo.Visibility == System.Windows.Visibility.Visible)
            {
                // start location service in background
                LocationServiceInfo.Instance.StartFetchLocationData(LocationServiceStatusCallback, LocationServiceValueCallback);
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
                MapDemo.Visibility = System.Windows.Visibility.Collapsed;
                System.Diagnostics.Debug.WriteLine("You do not have Internet connectivity.");
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (LocationServiceInfo.Instance.IsServiceRunning)
            {
                // stop location service in background
                LocationServiceInfo.Instance.Stop();
            }
            base.OnNavigatedFrom(e);
        }

    }
}