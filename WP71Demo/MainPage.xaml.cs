using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Json;
using System.Text;
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

        private ApplicationBarIconButton _appBarButtonCat = null;
        private ApplicationBarIconButton _appBarButtonDog = null;
        private ApplicationBarIconButton _appBarButtonMoney = null;

        private ApplicationBarMenuItem _menuItemMetropolia = null;
        private ApplicationBarMenuItem _menuItemEvtek = null;

        public MainPage()
        {
            InitializeComponent();
            InitializeApplicationBar();
            Loaded += MainPage_Loaded;
        }

        /// <summary>
        /// Create application bar
        /// </summary>
        private void InitializeApplicationBar()
        {
            if (_appBarButtonCat == null)
            {
                _appBarButtonCat = new ApplicationBarIconButton();
                _appBarButtonCat.IconUri = new Uri("/Icons/appbar.animal.cat.png", UriKind.Relative);
                _appBarButtonCat.Text = AppResources.AppBarIconButtonCat;
                _appBarButtonCat.Click += _appBarButtonCat_Click;
            }

            if (_appBarButtonDog == null)
            {
                _appBarButtonDog = new ApplicationBarIconButton();
                _appBarButtonDog.IconUri = new Uri("/Icons/appbar.animal.dog.png", UriKind.Relative);
                _appBarButtonDog.Text = AppResources.AppBarIconButtonDog;
            }

            if (_appBarButtonMoney == null)
            {
                _appBarButtonMoney = new ApplicationBarIconButton();
                _appBarButtonMoney.IconUri = new Uri("/Icons/appbar.money.png", UriKind.Relative);
                _appBarButtonMoney.Text = AppResources.AppBarIconButtonMoney;
            }

            if (_menuItemMetropolia == null)
            {
                _menuItemMetropolia = new ApplicationBarMenuItem();
                _menuItemMetropolia.IsEnabled = true;
                _menuItemMetropolia.Text = "Metropolia";
                _menuItemMetropolia.Click += _menuItemMetropolia_Click;
            }

            if (_menuItemEvtek == null)
            {
                _menuItemEvtek = new ApplicationBarMenuItem();
                _menuItemEvtek.IsEnabled = true;
                _menuItemEvtek.Text = "Evtek";
            }

            if (!ApplicationBar.Buttons.Contains(_appBarButtonCat))
            {
                ApplicationBar.Buttons.Add(_appBarButtonCat);
            }
            if (!ApplicationBar.Buttons.Contains(_appBarButtonDog))
            {
                ApplicationBar.Buttons.Add(_appBarButtonDog);
            }
            if (!ApplicationBar.Buttons.Contains(_appBarButtonMoney))
            {
                ApplicationBar.Buttons.Add(_appBarButtonMoney);
            }

            ApplicationBar.IsMenuEnabled = true;

            if (!ApplicationBar.MenuItems.Contains(_menuItemMetropolia))
            {
                ApplicationBar.MenuItems.Add(_menuItemMetropolia);
            }

            if (!ApplicationBar.MenuItems.Contains(_menuItemEvtek))
            {
                ApplicationBar.MenuItems.Add(_menuItemEvtek);
            }
        }

        void _menuItemMetropolia_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Application bar menu item: metropolia.");
        }

        void _appBarButtonCat_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Application bar icon button: cat.");
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

            button71.Click += button71_Click;
            button72.Click += button72_Click;
            button73.Click += button73_Click;
            button81.Click += button81_Click;
        }

        void button81_Click(object sender, RoutedEventArgs e)
        {
            WideTileCreator.UpdateFlipTile (
             "test",
             "test back",
             "test back content",
             "test wide back content",
             20,
             new Uri("/View/FirstPage.xaml?pinned=MyImage.jpg", UriKind.Relative), // tile id
             new Uri("ApplicationIcon.png", UriKind.Relative),
             new Uri("Background.png", UriKind.Relative),
             new Uri("ApplicationIcon.png", UriKind.Relative),
             new Uri("ApplicationIcon.png", UriKind.Relative),
             new Uri("ApplicationIcon.png", UriKind.Relative)
            );
        }

        void button73_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/ReminderPage.xaml", UriKind.Relative));
        }

        void button72_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/LocationMapPage.xaml", UriKind.Relative));
        }

        void button71_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/MotionPage.xaml", UriKind.Relative));
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

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            {
                System.Diagnostics.Debug.WriteLine("Navigated To: back from other page.");
            }
            else if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                System.Diagnostics.Debug.WriteLine("Navigated To: new page.");
            }

            base.OnNavigatedTo(e);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FireNetworking(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b.Name.Equals(button_WebClient.Name))
            {
                // before do networking stuff, 
                // make sure, there is network available
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    DoWebClientWork();
                }
                else
                {
                    MessageBox.Show("Sorry, network is unavailable.");
                }
            }
            else if (b.Name.Equals(button_HttpWebRequest.Name))
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    DoHttpWebRequest();
                }
                else
                {
                    MessageBox.Show("Sorry, network is unavailable.");
                }
            }
        }

        #region WebClient
        private void DoWebClientWork()
        {
            // create an instance
            WebClient client = new WebClient();
            // add an event handler
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.Headers["getHeader"] = "wp7";
            // fire the event 
            client.DownloadStringAsync(new Uri("http://users.metropolia.fi/~chaow/wp7.php"));
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // make sure everything is working correctly
            if ((e.Result != null) && (e.Error == null))
            {
                MessageBox.Show(e.Result.ToString());
            }
            else if (e.Error != null)
            {
                System.Diagnostics.Debug.WriteLine(e.Error.Message);
            }
        }
        #endregion

        #region HttpWebRequest
        private void DoHttpWebRequest()
        {
            HttpWebRequest request = WebRequest.Create(new Uri("http://www.google.com/")) as HttpWebRequest;
            request.Method = "GET";
            request.Headers["getHeader"] = "google";
            request.BeginGetResponse(new AsyncCallback(HttpGetCallback), request);
        }

        private void HttpGetCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)asynchronousResult.AsyncState;
                response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string returnData = String.Empty;
                        returnData = streamReader.ReadToEnd();
                        Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            MessageBox.Show(returnData);
                        }));
                    }
                }
                else
                {
                    // bad request
                    System.Diagnostics.Debug.WriteLine("HTTP status code: " + response.StatusCode);
                }
            }
            catch (Exception e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {

                    System.Diagnostics.Debug.WriteLine(e.Message);

                });
            }
        }
        #endregion

        #region JSON
        private void HandleJSON(object sender, RoutedEventArgs e)
        {
            // create an instance
            WebClient client = new WebClient();
            // add an event handler
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(JSON_DownloadStringCompleted);
            // fire the event 
            client.DownloadStringAsync(new Uri("http://users.metropolia.fi/~chaow/json.php"));
        }

        void JSON_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            List<Computer> computerList;
            // make sure everything is working correctly
            if ((e.Result != null) && (e.Error == null))
            {
                string jsonString = e.Result.ToString();

                //load into memory stream
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
                {
                    ComputerList obj = null;

                    //parse into jsonser
                    // note that to using System.Runtime.Serialization.Json
                    // need to add reference System.Servicemodel.Web
                    var ser = new DataContractJsonSerializer(typeof(ComputerList));

                    try
                    {
                        obj = (ComputerList)ser.ReadObject(ms);
                        computerList = obj.computerList;
                        foreach (Computer c in computerList)
                        {
                            System.Diagnostics.Debug.WriteLine(c.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }
            }
            else if (e.Error != null)
            {
                System.Diagnostics.Debug.WriteLine(e.Error.Message);
            }
        }
        #endregion

        #region ProgressBar indicator
        private void FireIndicator(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_Sleep);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_SleepCompleted);
            worker.RunWorkerAsync();

            myIndicator.IsIndeterminate = true;
            myIndicator.Visibility = System.Windows.Visibility.Visible;
        }

        void worker_SleepCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // need to switch to UI thread to refresh UI
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                myIndicator.IsIndeterminate = false;
                myIndicator.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        void worker_Sleep(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(10000);
        }
        #endregion

        /// <summary>
        /// Context menu event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // get context menu item header info
            string header = (sender as MenuItem).Header.ToString();

            ListBoxItem selectedListBoxItem
                = this.PersonList.ItemContainerGenerator.ContainerFromItem((sender as MenuItem).DataContext) as ListBoxItem;
            // check if the list item is null
            if (selectedListBoxItem == null)
            {
                return;
            }

            if (header == AppResources.Header1)
            {
                System.Diagnostics.Debug.WriteLine("--- context menu: " + AppResources.Header1 + "---");
            }
            else if (header == AppResources.Header2)
            {
                System.Diagnostics.Debug.WriteLine("--- context menu: " + AppResources.Header2 + "---");
            }
        }



    }
}