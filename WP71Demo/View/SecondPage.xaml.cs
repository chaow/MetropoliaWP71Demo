using Microsoft.Phone.Controls;

namespace WP71Demo.Views
{
    public partial class SecondPage : PhoneApplicationPage
    {

        public SecondPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("---OnNavigatedTo---");

            string val = string.Empty;
            if (NavigationContext.QueryString.ContainsKey("param"))
            {
                val = NavigationContext.QueryString["param"];
            }

            System.Diagnostics.Debug.WriteLine(string.IsNullOrEmpty(val)  ? "No such value." : val);
            base.OnNavigatedTo(e);
        }


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("---OnNavigatedFrom---");

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("---OnNavigatingFrom---");

            base.OnNavigatingFrom(e);
        }

    }
}