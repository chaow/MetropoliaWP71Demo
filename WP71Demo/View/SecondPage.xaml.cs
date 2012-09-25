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
                    string target = string.Empty;
                    target = service.State[Computer.KEY] as string;
                    if (target != string.Empty)
                    {
                        Computer c = DataContractSerializerHelper.Deserialize<Computer>(target);

                        System.Diagnostics.Debug.WriteLine(c.ToString());

                        // IsolatedStorageSettings
                        var setting = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings;
                        if (setting.Contains(Computer.KEY))
                        {
                            // if already saved, remove it
                            setting.Remove(Computer.KEY);
                            System.Diagnostics.Debug.WriteLine("Computer object is removed.");
                        }
                        setting.Add(Computer.KEY, target);
                        setting.Save();
                        System.Diagnostics.Debug.WriteLine("Computer object is saved.");
                    }
                }
            }
            catch (System.ArgumentNullException)
            {
                System.Diagnostics.Debug.WriteLine("No such key.");
            }
            catch (System.NullReferenceException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
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