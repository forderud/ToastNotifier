using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

namespace ToastNotifierWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserNotificationListener m_listener;

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

            if (!ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener"))
                MessageBox.Show("UserNotificationListener not supported (too old Windows version).", "UWP error", MessageBoxButton.OK, MessageBoxImage.Error);


            // get listener for the current user
            m_listener = UserNotificationListener.Current;

            InitializeListener();
        }

        private async void InitializeListener()
        {
            // request access to the user's notifications (must be called from UI thread)
            UserNotificationListenerAccessStatus accessStatus = await m_listener.RequestAccessAsync();
            switch (accessStatus)
            {
                case UserNotificationListenerAccessStatus.Allowed:
                    break; // success

                case UserNotificationListenerAccessStatus.Denied:
                    MessageBox.Show("UserNotificationListenerAccessStatus.Denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;

                case UserNotificationListenerAccessStatus.Unspecified:
                    MessageBox.Show("UserNotificationListenerAccessStatus.Unspecified. Please retry.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }

            try
            {
                m_listener.NotificationChanged += Listener_NotificationChanged; // 'Element not found' exception if not running as a UWP process
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "NotificationChanged error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Listener_NotificationChanged(UserNotificationListener sender, UserNotificationChangedEventArgs args)
        {
            UserNotification notif = m_listener.GetNotification(args.UserNotificationId);
            if (notif == null)
                return;

            Application.Current.Dispatcher.Invoke(new Action(() => {
                // access UI from main thread
                appName.Content = notif.AppInfo.DisplayInfo.DisplayName; // application name
                textBox.Clear();
                UpdateTextBox(notif);
            }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ToastMessage.Generate();
        }

        private void UpdateTextBox (UserNotification notif)
        {
            // accessing notif.Notification.Visual.Bindings directly will just give the same "ToastGeneric" NotificationBinding
            NotificationBinding toastBinding = notif.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);
            if (toastBinding == null)
                return;

            IReadOnlyList<AdaptiveNotificationText> elms = toastBinding.GetTextElements();
            foreach (var elm in elms)
                textBox.AppendText(elm.Text + Environment.NewLine);
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
