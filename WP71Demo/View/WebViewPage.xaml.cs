using Microsoft.Phone.Controls;
using System;

namespace WP71Demo.View
{
    public partial class WebViewPage : PhoneApplicationPage
    {
        public WebViewPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            webBrowser1.Navigate(new Uri("http://www.google.com"));

            base.OnNavigatedTo(e);
        }
    }
}