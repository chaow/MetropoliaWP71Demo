using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoTest
{
    [TestClass]
    public class UnitTest : SilverlightTest
    {
        [TestInitialize]
        public void Initialize()
        {
            System.Diagnostics.Debug.WriteLine("----Starting test----");
		}

        [TestMethod]
        [Description("This test always pass intentionally")]
        public void AlwaysPass()
        {
            Assert.IsTrue(true, "method intended to always pass");
        }

        [TestMethod]
        [Description("Check ReminderPage")]
        public void CheckReminderPage()
        {
            WP71Demo.View.ReminderPage page = new WP71Demo.View.ReminderPage();
            Assert.IsNotNull(page);
        }

        [TestMethod]
        [Description("Check ReminderPage CreatButton")]
        public void CheckReminderPageCreatButton()
        {
            WP71Demo.View.ReminderPage page = new WP71Demo.View.ReminderPage();
            // fake click event
            page.CreateButton_Click(page.TestButton, new System.Windows.RoutedEventArgs());
        }
    }
}
