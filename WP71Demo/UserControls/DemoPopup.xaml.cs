using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WP71Demo.UserControls
{
    public partial class DemoPopup : UserControl
    {
        public DemoPopup()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(DemoPopup_Loaded);
        }

        void DemoPopup_Loaded(object sender, RoutedEventArgs e)
        {
            button1.Click += new RoutedEventHandler(button1_Click);
        }

        void button1_Click(object sender, RoutedEventArgs e)
        {
            Popup thisPopup = this.Parent as Popup;
            thisPopup.IsOpen = false;
        }

    }
}
