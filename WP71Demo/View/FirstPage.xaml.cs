using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WP71Demo.Model;
using WP71Demo.Util;

namespace WP71Demo.Views
{
    public partial class FirstPage : PhoneApplicationPage
    {
        private CameraCaptureTask _cameraCaptureTask = null;
        private Stream _capturedImage = null;
        private ApplicationBarIconButton _appBarButtonPin = null;
        private ApplicationBarIconButton _appBarButtonAdd = null;
        private ApplicationBarIconButton _appBarButtonDelete = null;

        public FirstPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            anotherButton.Click += anotherButton_Click;


            if (_appBarButtonAdd == null)
            {
                _appBarButtonAdd = new ApplicationBarIconButton();
                _appBarButtonAdd.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Add.png", UriKind.Relative);
                _appBarButtonAdd.Text = "add";
                _appBarButtonAdd.Click += _appBarButtonAdd_Click;
            }

            if (_appBarButtonPin == null)
            {
                _appBarButtonPin = new ApplicationBarIconButton();
                _appBarButtonPin.IconUri = new Uri("/Icons/appbar.pin.png", UriKind.Relative);
                _appBarButtonPin.Text = "pin";
                _appBarButtonPin.Click += _appBarButtonPin_Click;
            }

            if (_appBarButtonDelete == null)
            {
                _appBarButtonDelete = new ApplicationBarIconButton();
                _appBarButtonDelete.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Delete.png", UriKind.Relative);
                _appBarButtonDelete.Text = "delete";
                _appBarButtonDelete.Click += _appBarButtonDelete_Click;
            }

            if (!ApplicationBar.Buttons.Contains(_appBarButtonPin))
            {
                ApplicationBar.Buttons.Add(_appBarButtonPin);
            }

            if (!ApplicationBar.Buttons.Contains(_appBarButtonAdd))
            {
                ApplicationBar.Buttons.Add(_appBarButtonAdd);
            }

            if (!ApplicationBar.Buttons.Contains(_appBarButtonDelete))
            {
                ApplicationBar.Buttons.Add(_appBarButtonDelete);
            }

            LoadFromLocalStorage();
        }

        /// <summary>
        /// Add image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _appBarButtonAdd_Click(object sender, EventArgs e)
        {
            if (_cameraCaptureTask != null)
            {
                _cameraCaptureTask.Completed -= CameraCaptureTask_Completed;
                _cameraCaptureTask = null;
            }
            _cameraCaptureTask = new CameraCaptureTask();
            _cameraCaptureTask.Completed += CameraCaptureTask_Completed;
            _cameraCaptureTask.Show();
        }

        /// <summary>
        /// Camera completed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                _capturedImage = e.ChosenPhoto;
                //Code to display the photo on the page in an image control named myImage.
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(_capturedImage);
                MyImage.Source = bmp;
            }
            else if (e.TaskResult == TaskResult.Cancel)
            {
                MessageBox.Show("Operation was cancelled", "Photo not captured", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Error while capturing photo:\n" + e.Error.Message, "Fail", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Delete tile and image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _appBarButtonDelete_Click(object sender, EventArgs e)
        {
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("pinned=MyImage.jpg"));

            if (TileToFind != null)
            {
                TileToFind.Delete();
                DeleteImageFile();
                MyImage.Source = null;
            }
            else
            {
                MessageBox.Show("No tile available.");
            }
        }

        /// <summary>
        /// Pin the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _appBarButtonPin_Click(object sender, EventArgs e)
        {
            if (_capturedImage == null)
            {
                MessageBox.Show("Take a picutre first.");
            }
            else
            {
                string fileName = "MyImage.jpg";
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (storage.FileExists(fileName))
                    {
                        storage.DeleteFile(fileName);
                    }

                    IsolatedStorageFileStream fileStream = storage.CreateFile(fileName);
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.SetSource(_capturedImage);

                    WriteableBitmap wb = new WriteableBitmap(bitmap);
                    wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                    fileStream.Close();
                }

                CreateTile();
            }
            
        }

        private void CreateTile()
        {
            /**
             * http://msdn.microsoft.com/en-us/library/windowsphone/develop/hh202979(v=vs.105).aspx
             */
            // Look to see whether the Tile already exists and if so, don't try to create it again.
            // need to use the reference Microsoft.Phone.Shell and System.Linq
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("pinned=MyImage.jpg"));

            // Create the Tile if we didn't find that it already exists.
            if (TileToFind == null)
            {

                // Create the Tile object and set some initial properties for the Tile.
                // A Count value of 0 indicates that the Count should not be displayed.
                StandardTileData NewTileData = new StandardTileData
                {
                    BackgroundImage = new Uri("ApplicationIcon.png", UriKind.Relative),
                    Title = "Tile demo",
                    Count = 0
                };

                // Create the Tile and pin it to Start. This will cause a navigation to Start and a deactivation of our application.
                ShellTile.Create(new Uri("/View/FirstPage.xaml?pinned=MyImage.jpg", UriKind.Relative), NewTileData);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("--- A tile is already existed.---");
                MessageBox.Show("A tile is already existed.");
            }
        }

        private void LoadFromLocalStorage()
        {
            try
            {
                WriteableBitmap bitmap = new WriteableBitmap(800, 800);
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("MyImage.jpg", FileMode.Open, FileAccess.Read))
                    {
                        // Decode the JPEG stream.
                        bitmap = PictureDecoder.DecodeJpeg(fileStream);
                    }
                }
                MyImage.Source = bitmap;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        private void DeleteImageFile()
        {
            string fileName = "MyImage.jpg";
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (storage.FileExists(fileName))
                {
                    storage.DeleteFile(fileName);
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("No such file.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void anotherButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBox.Show(AppResources.MessageBoxString);
            //MessageBoxResult msgResult = MessageBox.Show(AppResources.MessageBoxString, "", MessageBoxButton.OKCancel);
            //if (msgResult == MessageBoxResult.OK)
            //{
            //    System.Diagnostics.Debug.WriteLine("OK");

            //    // make sure that navigation is performed in UI thread.
            //    Deployment.Current.Dispatcher.BeginInvoke(() =>
            //    {
            //        NavigationService.Navigate(new Uri("/View/SecondPage.xaml?param=yes", UriKind.Relative));                    
            //    });
            //}
            //else if (msgResult == MessageBoxResult.Cancel)
            //{
            //    System.Diagnostics.Debug.WriteLine("Cancel");
            //}
        }

        /// <summary>
        /// Shared button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SharedButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender != null)
            {
                Button button = sender as Button;
                if (button != null)
                {
                    if (SharedButton1.Name.Equals(button.Name))
                    {
                        System.Diagnostics.Debug.WriteLine("Shared button 1");
                    }
                    else if (SharedButton2.Name.Equals(button.Name))
                    {
                        System.Diagnostics.Debug.WriteLine("Shared button 2");
                    }
                }            
            }
        }

        // application level
        PhoneApplicationService service = PhoneApplicationService.Current;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("---OnNavigatedTo---");
            try
            {
                if (service.State.ContainsKey(School.KEY))
                {
                    School s = DataContractSerializerHelper.Deserialize<School>(service.State[School.KEY] as string);
                    System.Diagnostics.Debug.WriteLine(s.ToString());
                }
            }
            catch (System.ArgumentNullException)
            {
                System.Diagnostics.Debug.WriteLine("No such key.");
            }

            if (NavigationContext.QueryString.ContainsKey("pinned"))
            {
                System.Diagnostics.Debug.WriteLine("Launching from a tile.");
            }

            LoadFromLocalStorage();

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