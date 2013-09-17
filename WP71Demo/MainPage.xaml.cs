using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WP71Demo.Model;
using WP71Demo.UserControls;
using WP71Demo.Util;

namespace WP71Demo
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Popup _popup = null;

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            button1.Click += button1_Click;
            button2.Click += button2_Click;

            // once the page loaded completed, create a thread 
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }

        void button1_Click(object sender, RoutedEventArgs e)
        {
            if (_popup != null)
            {
                _popup.IsOpen = false;
                _popup = null;
            }
            _popup = new Popup() { IsOpen = true, Child = new DemoPopup() };
        }

        // application level
        PhoneApplicationService service = PhoneApplicationService.Current;
        void button2_Click(object sender, RoutedEventArgs e)
        {
            School s = new School();
            s.Name = "Metropolia";
            s.Address = "Vanha maantie 6, Espoo";
            service.State[School.KEY] = DataContractSerializerHelper.Serialize(s);

            NavigationService.Navigate(new Uri("/View/FirstPage.xaml", UriKind.Relative));
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // BackgoundWork do nothing, but sleep three and half seconds
            // Note that at this point UI Thread is not blocked
            System.Threading.Thread.Sleep(3500);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Note that when not in main UI thread, in order to refresh UI,
            // we have to use Dispather object to manage the cross-thread-access issue,
            // back to main UI thread 
            // Note that Action object is a delegate
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                // assign the view model to the corresponding UI component
                this.PersonList.ItemsSource = App.PersonViewModel.ItemViewModel;
            });
        }

        /// <summary>
        /// list selected event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PersonList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // íf selected index is -1 (no selesction), do nothing
            if (PersonList.SelectedIndex == -1)
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine("---list index: " + PersonList.SelectedIndex + " ---");
            // do something here...

            // rest the selected index
            PersonList.SelectedIndex = -1;
        }


        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if ((_popup != null) && (_popup.IsOpen))
            {
                _popup.IsOpen = false;
                _popup = null;

                // need to cancel the event
                e.Cancel = true;
            }

            base.OnBackKeyPress(e);
        }

    }
}