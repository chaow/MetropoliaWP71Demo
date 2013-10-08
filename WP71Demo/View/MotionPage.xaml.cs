using System;
using Microsoft.Devices.Sensors;
using Microsoft.Phone.Controls;
using System.Windows;
using System.Text;

namespace WP71Demo.View
{
    public partial class SensorPage : PhoneApplicationPage
    {
        private Accelerometer _accelerometer = null;
        private Motion _motion = null;

        public SensorPage()
        {
            InitializeComponent();
                      Loaded += LocationMapPage_Loaded;
        }

        void LocationMapPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            /**
             * Note that working with emulator, ONLY accelermeter sensor is available, so can not use motion API
             */
            System.Diagnostics.Debug.WriteLine("The device supports Compass: " + Compass.IsSupported);
            System.Diagnostics.Debug.WriteLine("The device supports Accelerometer: " + Accelerometer.IsSupported);
            System.Diagnostics.Debug.WriteLine("The device supports Gryoscope: " + Gyroscope.IsSupported);
            System.Diagnostics.Debug.WriteLine("The device supports Motion: " + Motion.IsSupported);

            base.OnNavigatedTo(e);
        }


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // we need to ensure we release the sensor
            StopAccelerometerSensor();
            StopMotion();
            base.OnNavigatedFrom(e);
        }

        void MotionPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Motion.IsSupported)
            {
                // hide the two buttons
                AccelerometerStartButton.Visibility = System.Windows.Visibility.Collapsed;
                AccelerometerStopButton.Visibility = System.Windows.Visibility.Collapsed;

                InitMotion();
            }
            else
            {
                // hide the two buttons
                MotionStartButton.Visibility = System.Windows.Visibility.Collapsed;
                MotionStopButton.Visibility = System.Windows.Visibility.Collapsed;

                if (Accelerometer.IsSupported)
                {
                    InitAccelerometer();
                }
            }
        }

        #region  Accelerometer

        private void AccelerometerStartButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_accelerometer == null)
            {
                InitAccelerometer();
            }
            try
            {
                _accelerometer.Start();
                System.Diagnostics.Debug.WriteLine("Start accelerometer sensor. ");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Can not start accelerometer sensor " + ex.Message);
            }
        }

        private void AccelerometerStopButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StopAccelerometerSensor();
        }

        private void StopAccelerometerSensor()
        {
            if (_accelerometer != null)
            {
                _accelerometer.Stop();
                System.Diagnostics.Debug.WriteLine("Stop accelerometer sensor. ");
                _accelerometer = null;
                DataValue.Text = string.Empty;
            }
        }

        private void InitAccelerometer()
        {
            if (_accelerometer == null)
            {
                // Instantiate the Accelerometer.
                _accelerometer = new Accelerometer();
                _accelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(300);
                _accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(accelerometer_CurrentValueChanged);
            }
        }

        void accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            // Call UpdateUI on the UI thread and pass the AccelerometerReading.
            // Note that when changing UI components, always do the job on UI thread. 
            Deployment.Current.Dispatcher.BeginInvoke(() => UpdateAccelerometerUI(e.SensorReading));
        }

        private void UpdateAccelerometerUI(AccelerometerReading accelerometerReading)
        {
            // have to use Vector3 class object
            Microsoft.Xna.Framework.Vector3 acceleration = accelerometerReading.Acceleration;

            StringBuilder sb = new StringBuilder();
            sb.Append("Accelerometer sensor, raw data X: " + acceleration.X.ToString("0.00"));
            sb.Append("\nAccelerometer sensor, raw data Y: " + acceleration.Y.ToString("0.00"));
            sb.Append("\nAccelerometer sensor, raw data Z: " + acceleration.Y.ToString("0.00"));
            System.Diagnostics.Debug.WriteLine(sb.ToString());
            DataValue.Text = sb.ToString();
        }

        #endregion

        #region Motion

        private void InitMotion()
        {
            if (_motion == null)
            {
                _motion = new Motion();
            }
            _motion.TimeBetweenUpdates = TimeSpan.FromMilliseconds(250);
            _motion.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<MotionReading>>(motion_CurrentValueChanged);
        }

        void motion_CurrentValueChanged(object sender, SensorReadingEventArgs<MotionReading> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => UpdateMotionUI(e.SensorReading));
        }

        private void UpdateMotionUI(MotionReading e)
        {
            float pitch = e.Attitude.Pitch;
            float yaw = e.Attitude.Yaw;
            float roll = e.Attitude.Roll;
            float accelerometerX = e.DeviceAcceleration.X;
            float accelerometerY = e.DeviceAcceleration.Y;
            float accelerometerZ = e.DeviceAcceleration.Z;

            float gyroscopeX = e.DeviceRotationRate.X;
            float gyroscopeY = e.DeviceRotationRate.Y;
            float gyroscopeZ = e.DeviceRotationRate.Z;

            float gravityX = e.Gravity.X;
            float gravityY = e.Gravity.Y;
            float gravityZ = e.Gravity.Z;

            StringBuilder sb = new StringBuilder();
            sb.Append("Motion, raw data pitch: " + pitch);
            sb.Append("\nMotion, raw data yaw: " + yaw);
            sb.Append("\nMotion, raw data roll: " + roll);
            System.Diagnostics.Debug.WriteLine(sb.ToString());
            DataValue.Text = sb.ToString();
        }

        private void MotionStartButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_motion == null)
            {
                InitMotion();
            }
            try
            {
                _motion.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Can not start motion " + ex.Message);
            }
        }

        private void MotionStopButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StopMotion();
        }

        private void StopMotion()
        {
            if (_motion != null)
            {
                _motion.Stop();
                System.Diagnostics.Debug.WriteLine("Stop motion. ");
                _motion = null;
                DataValue.Text = string.Empty;
            }
        }

        #endregion



    }
}