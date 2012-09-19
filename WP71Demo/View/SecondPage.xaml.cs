using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WP71Demo.Model;

namespace WP71Demo.Views
{
    public partial class SecondPage : PhoneApplicationPage
    {
        public SecondPage()
        {
            InitializeComponent();
        }

        PhoneApplicationService service = PhoneApplicationService.Current;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("---OnNavigatedTo---");
            try
            {
                if (service.State.ContainsKey(Computer.KEY))
                {
                    Computer c = DataContractSerializerHelper.Deserialize<Computer>(service.State[Computer.KEY] as string);
                    System.Diagnostics.Debug.WriteLine(c.ToString());
                }
            }
            catch (System.ArgumentNullException)
            {
                System.Diagnostics.Debug.WriteLine("No such key.");
            }

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