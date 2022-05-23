using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications.Management;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ToastNotifierUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private UserNotificationListener m_listener;

        public MainPage()
        {
            this.InitializeComponent();

            // get listener for the current user
            m_listener = UserNotificationListener.Current;
        }

        private void btnGenerate_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // generate a test toast
            // SetBackgroundActivation will prevent launching a new instance of this app when the user clicks on the toast
            new ToastContentBuilder()
                .AddText("SW installation")
                .AddText("Choose installation time")
                .AddButton(new ToastButton()
                    .SetContent("Now")
                    .AddArgument("action", "now")
                    .SetBackgroundActivation())
                .AddButton(new ToastButton()
                    .SetContent("Later")
                    .AddArgument("action", "later")
                    .SetBackgroundActivation())
                .SetBackgroundActivation()
                .Show();
        }
    }
}
