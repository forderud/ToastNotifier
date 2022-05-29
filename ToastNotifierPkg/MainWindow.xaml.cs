using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

namespace ToastNotifierWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ToastMessage m_toast;

        public MainWindow()
        {
            InitializeComponent();

            // position window bottom-right to mimic in-built toast notifier
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;

            var uwp_checker = new Helpers();
            if (!uwp_checker.IsRunningAsUwp())
                MessageBox.Show("Not running as UWP", "UWP error", MessageBoxButton.OK, MessageBoxImage.Error);

            InitializeListener();
        }

        private async void InitializeListener()
        {
            try
            {
                m_toast = new ToastMessage(UpdateUI);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            m_toast.Generate();
        }

        private void UpdateUI (UserNotification notif)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                // access UI from main thread
                appName.Content = "";
                textBox.Clear();

                // accessing notif.Notification.Visual.Bindings directly will just give the same "ToastGeneric" NotificationBinding
                NotificationBinding toastBinding = notif.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);
                if (toastBinding == null)
                    return;

                appName.Content = notif.AppInfo.DisplayInfo.DisplayName; // application name
                IReadOnlyList<AdaptiveNotificationText> elms = toastBinding.GetTextElements();
                foreach (var elm in elms)
                    textBox.AppendText(elm.Text + Environment.NewLine);
            }));
        }
    }

    // Copied from https://github.com/qmatteoq/DesktopBridgeHelpers/blob/master/DesktopBridge.Helpers/Helpers.cs
    public class Helpers
    {
        const long APPMODEL_ERROR_NO_PACKAGE = 15700L;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);

        public bool IsRunningAsUwp()
        {
            int length = 0;
            StringBuilder sb = new StringBuilder(0);
            int result = GetCurrentPackageFullName(ref length, sb);

            sb = new StringBuilder(length);
            result = GetCurrentPackageFullName(ref length, sb);

            return result != APPMODEL_ERROR_NO_PACKAGE;
        }
    }
}
