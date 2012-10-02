using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using ExifLib;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using WP71Demo.Model;

namespace WP71Demo.Views
{
    public partial class SecondPage : PhoneApplicationPage
    {
        private Stream mCapturedImage;
        private CameraCaptureTask mCameraCaptureTask = null;
        private int _width;
        private int _height;
        private ExifLib.ExifOrientation _orientation;
        private int _angle;

        public SecondPage()
        {
            InitializeComponent();
            this.Loaded += new System.Windows.RoutedEventHandler(SecondPage_Loaded);
        }

        void SecondPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitSession6();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("---OnNavigatedTo---");
            GetNavigationPassingObjects();
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

        #region Session 4

        PhoneApplicationService service = PhoneApplicationService.Current;
        private void GetNavigationPassingObjects()
        {
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
        }

        #endregion

        #region Session 6

        private void InitSession6()
        {
            // create application bar
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = true;
            ApplicationBarIconButton captureButton = new ApplicationBarIconButton(new Uri("/Images/appbar.money.png", UriKind.Relative));
            captureButton.Text = "capture";
            captureButton.Click += new EventHandler(captureButton_Click);
            ApplicationBarIconButton pinButton = new ApplicationBarIconButton(new Uri("/Images/appbar.pin.png", UriKind.Relative));
            pinButton.Text = "pin";
            pinButton.Click += new EventHandler(pinButton_Click);
            ApplicationBarIconButton deleteButton = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Delete.png", UriKind.Relative));
            deleteButton.Text = "delete";
            deleteButton.Click += new EventHandler(deleteButton_Click);
            ApplicationBar.Buttons.Add(captureButton);
            ApplicationBar.Buttons.Add(pinButton);
            ApplicationBar.Buttons.Add(deleteButton);
        }

        private void CreatTile()
        {

            /**
            * http://msdn.microsoft.com/en-us/library/windowsphone/develop/hh202979(v=vs.92).aspx
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
                ShellTile.Create(new Uri("/View/SecondPage.xaml?pinned=MyImage.jpg", UriKind.Relative), NewTileData);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("--- A tile is already existed.---");
                MessageBox.Show("A tile is already existed.");
            }
        }

        void deleteButton_Click(object sender, EventArgs e)
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

        void pinButton_Click(object sender, EventArgs e)
        {
            if (mCapturedImage == null)
            {
                MessageBox.Show("Take a picutre first.");
            }
            else
            {
                SaveImage();
                CreatTile();
            }
        }

        /**
         * I/O operations 
         * 
         * http://www.windowsphonegeek.com/tips/All-about-WP7-Isolated-Storage---Read-and-Save-Captured-Image
         * 
         */

        private void SaveImage()
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
                bitmap.SetSource(mCapturedImage);

                WriteableBitmap wb = new WriteableBitmap(bitmap);
                wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                fileStream.Close();
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

        #region Image handling
        void captureButton_Click(object sender, EventArgs e)
        {
            if (mCameraCaptureTask != null)
            {
                mCameraCaptureTask.Completed -= CameraCaptureTask_Completed;
                mCameraCaptureTask = null;
            }
            mCameraCaptureTask = new CameraCaptureTask();
            mCameraCaptureTask.Completed += new EventHandler<PhotoResult>(CameraCaptureTask_Completed);
            mCameraCaptureTask.Show();
        }

        void CameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                HandleImage(e);
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

        /**
         * Handling picture orientation in CameraCaptureTask in Windows Phone 7
         * 
         * reference:
         * http://timheuer.com/blog/archive/2010/09/23/working-with-pictures-in-camera-tasks-in-windows-phone-7-orientation-rotation.aspx
         * 
         * 
         * ExifReader DLL is available
         * http://www.codeproject.com/Articles/36342/ExifLib-A-Fast-Exif-Data-Extractor-for-NET-2-0
         * 
         */
        private void HandleImage(PhotoResult e)
        {
            // firgure out the orientation from EXIF format
            e.ChosenPhoto.Position = 0;
            
            JpegInfo info = ExifReader.ReadJpeg(e.ChosenPhoto, e.OriginalFileName);

            _width = info.Width;
            _height = info.Height;
            _orientation = info.Orientation;

            switch (info.Orientation)
            {
                case ExifOrientation.TopLeft:
                case ExifOrientation.Undefined:
                    _angle = 0;
                    break;
                case ExifOrientation.TopRight:
                    _angle = 90;
                    break;
                case ExifOrientation.BottomRight:
                    _angle = 180;
                    break;
                case ExifOrientation.BottomLeft:
                    _angle = 270;
                    break;
            }

            if (_angle > 0d)
            {
                mCapturedImage = RotateStream(e.ChosenPhoto, _angle);
            }
            else
            {
                mCapturedImage = e.ChosenPhoto;
            }

            BitmapImage bmp = new BitmapImage();
            bmp.SetSource(mCapturedImage);

            MyImage.Source = bmp;
        }

        private Stream RotateStream(Stream stream, int angle)
        {
            stream.Position = 0;
            if (angle % 90 != 0 || angle < 0) throw new ArgumentException();
            if (angle % 360 == 0) return stream;

            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(stream);
            WriteableBitmap wbSource = new WriteableBitmap(bitmap);

            WriteableBitmap wbTarget = null;
            if (angle % 180 == 0)
            {
                wbTarget = new WriteableBitmap(wbSource.PixelWidth, wbSource.PixelHeight);
            }
            else
            {
                wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
            }

            for (int x = 0; x < wbSource.PixelWidth; x++)
            {
                for (int y = 0; y < wbSource.PixelHeight; y++)
                {
                    switch (angle % 360)
                    {
                        case 90:
                            wbTarget.Pixels[(wbSource.PixelHeight - y - 1) + x * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                        case 180:
                            wbTarget.Pixels[(wbSource.PixelWidth - x - 1) + (wbSource.PixelHeight - y - 1) * wbSource.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                        case 270:
                            wbTarget.Pixels[y + (wbSource.PixelWidth - x - 1) * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                    }
                }
            }
            MemoryStream targetStream = new MemoryStream();
            wbTarget.SaveJpeg(targetStream, wbTarget.PixelWidth, wbTarget.PixelHeight, 0, 100);
            return targetStream;
        }
        #endregion

        #endregion

    }
}