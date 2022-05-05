using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
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
        private UserNotificationListener m_listener;

        public MainWindow()
        {
            InitializeComponent();

            // position window bottom-right to mimic in-built toast notifier
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;

            // get listener for the current user
            m_listener = UserNotificationListener.Current;

            InitalizeListener();
        }

        private async void InitalizeListener()
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

            m_listener.NotificationChanged += Listener_NotificationChanged; // 'Element not found' exception if not running as a UWP process
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
            // generate a test toast
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
                .Show();
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
}
