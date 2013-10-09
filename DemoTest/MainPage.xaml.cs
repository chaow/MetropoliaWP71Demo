using Microsoft.Phone.Controls;

namespace DemoTest
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            MainWindowExtensions.StartTestRunner(this);
        }
    }
}