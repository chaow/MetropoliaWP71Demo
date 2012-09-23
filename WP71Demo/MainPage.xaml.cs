using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
using WP71Demo.UserControls;
using WP71Demo.ViewModel;
using WP71Demo.Model;
using Microsoft.Phone.Shell;

namespace WP71Demo
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Popup mPopup = null;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.button1.Click += new RoutedEventHandler(button_Click);
            this.button2.Click += new RoutedEventHandler(button_Click);

            // once the page loaded completed, create a thread 
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
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

        void button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (button1.Name.Equals(b.Name))
            {
                if (mPopup != null)
                {
                    mPopup.IsOpen = false;
                    mPopup = null;
                }
                mPopup = new Popup() { IsOpen = true, Child = new DemoPopup() };
            }
            else if (button2.Name.Equals(b.Name))
            {
                GoToSecondPage();
            }
        }

        // application level
        PhoneApplicationService service = PhoneApplicationService.Current;
        private void GoToSecondPage()
        { 
            Computer c = new Computer();
            c.Brand = "Metropolia";
            c.Price = 100;

            service.State[Computer.KEY] = DataContractSerializerHelper.Serialize(c);

            NavigationService.Navigate(new Uri("/View/SecondPage.xaml", UriKind.Relative));       
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if ((mPopup != null) && mPopup.IsOpen)
            {
                mPopup.IsOpen = false;
                mPopup = null;
                
                // need to cancel the event
                e.Cancel = true;
            }
            base.OnBackKeyPress(e);
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


    }
}