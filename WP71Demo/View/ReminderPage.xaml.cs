using System;
using System.Collections.Generic;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;

namespace WP71Demo.View
{
    public partial class ReminderPage : PhoneApplicationPage
    {
        public System.Windows.Controls.Button TestButton = null;

        /// <summary>
        /// 50 reminders per application
        /// </summary>

        public ReminderPage()
        {
            InitializeComponent();
            Loaded += ReminderPage_Loaded;
            TestButton = CreateButton;
        }

        public void CreateButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                string reminderName = "My reminder " + i;

                Reminder reminder = new Reminder(reminderName);
                // NOTE: setting the Title property is supported for reminders 
                reminder.Title = "My reminders .... " + i;
                reminder.Content = "My reminders .... My reminders ...." + i;

                //NOTE: the value of BeginTime must be after the current time
                //set the BeginTime time property in order to specify when the reminder should be shown
                reminder.BeginTime = DateTime.Now.AddSeconds(5.0D + i);

                // NOTE: ExpirationTime must be after BeginTime
                // the value of the ExpirationTime property specifies when the schedule of the reminder expires
                // very useful for recurring reminders, ex:
                // show reminder every day at 5PM but stop after 10 days from now
                reminder.ExpirationTime = reminder.BeginTime.AddSeconds(5.0 + i);
                reminder.RecurrenceType = RecurrenceInterval.Daily;

                // you can set a navigation uri that is passed to the application when it is launched from the reminder
                //reminder.NavigationUri = navigationUri;
                reminder.NavigationUri = new Uri("/View/FirstPage.xaml", UriKind.Relative);

                ScheduledAction sa = ScheduledActionService.Find(reminderName);
                if (sa == null)
                {
                    try
                    {
                        ScheduledActionService.Add(reminder);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Can not create the reminder: " + ex.Message);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("A reminder is set already.");
                    System.Windows.MessageBox.Show("A reminder is set already.");
                }
            }

            IEnumerable<Reminder> reminders = ScheduledActionService.GetActions<Reminder>();
            if (reminders != null)
            {
                foreach (Reminder re in reminders)
                {
                    System.Diagnostics.Debug.WriteLine("The name of the reminder: " + re.Name);
                    System.Diagnostics.Debug.WriteLine("The reminder is scheduled: " + re.IsScheduled);
                    System.Diagnostics.Debug.WriteLine("The tile of the reminder: " + re.Title);
                }
                System.Windows.MessageBox.Show("Reminders are scheduled!");
            }
        }

        private void DeleteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IEnumerable<Reminder> reminders = ScheduledActionService.GetActions<Reminder>();
            if (reminders != null)
            {
                foreach (Reminder re in reminders)
                {
                    string reminderName = re.Name;
                    ScheduledAction sa = ScheduledActionService.Find(reminderName);
                    if (sa != null)
                    {
                        ScheduledActionService.Remove(reminderName);
                        System.Diagnostics.Debug.WriteLine("Remove a reminder: " + reminderName);
                    }
                }
            }
        }

        void ReminderPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            MyDatePicker.ValueChanged += new EventHandler<DateTimeValueChangedEventArgs>(MyDatePicker_ValueChanged);
            MyTimePicker.ValueChanged += new EventHandler<DateTimeValueChangedEventArgs>(MyTimePicker_ValueChanged);
        }

        void MyTimePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            DateTime dt = (DateTime)e.NewDateTime;
            System.Diagnostics.Debug.WriteLine("Time is " + dt.ToShortTimeString());
        }

        void MyDatePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            DateTime dt = (DateTime)e.NewDateTime;
            System.Diagnostics.Debug.WriteLine("Date is " + dt.ToShortDateString());
        }

    }
}