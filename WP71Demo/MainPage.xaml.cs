using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using WP71Demo.Model;
using WP71Demo.UserControls;

namespace WP71Demo
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Popup mPopup = null;

        private bool isFirstLoad = true;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        #region Session 4
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // get to know the page is loaded once
            if (isFirstLoad)
            {
                isFirstLoad = false;
            }


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
        #endregion


        #region Seesion 5

        #region Tombstoning
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            //before leave this page, save the state 
            //this.State["pageLoaded"] = isFirstLoad;
            //this.State["PersonViewModel"] = App.PersonViewModel;
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                if (this.State.ContainsKey("pageLoaded"))
                {
                    WP71Demo.ViewModel.PersonViewModel pvl = this.State["PersonViewModel"] as WP71Demo.ViewModel.PersonViewModel;
                    this.PersonList.ItemsSource = pvl.ItemViewModel; 
                }
            }
            catch (ArgumentNullException)
            {
            } 
            base.OnNavigatedTo(e);
        }

        #endregion

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
             catch(Exception e)
             { 
                Deployment.Current.Dispatcher.BeginInvoke(delegate() {

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
                myIndicator.IsIndeterminate =false;
                myIndicator.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        void worker_Sleep(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(10000);
        }
        #endregion

        #endregion

    }
}