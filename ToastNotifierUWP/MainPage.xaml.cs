using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
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
            InitializeListener();
        }

        private async void InitializeListener()
        {
            if (!ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener"))
            {
                var dialog = new ContentDialog
                {
                    Title = "UWP error",
                    Content = "UserNotificationListener not supported (too old Windows version).",
                    CloseButtonText = "Ok"
                };

                await dialog.ShowAsync();
                return;
            }

            // get listener for the current user
            m_listener = UserNotificationListener.Current;

            // request access to the user's notifications (must be called from UI thread)
            UserNotificationListenerAccessStatus accessStatus = await m_listener.RequestAccessAsync();
            switch (accessStatus)
            {
                case UserNotificationListenerAccessStatus.Allowed:
                    break; // success

                case UserNotificationListenerAccessStatus.Denied:
                    {
                        var dialog = new ContentDialog
                        {
                            Title = "Error",
                            Content = "UserNotificationListenerAccessStatus.Denied.",
                            CloseButtonText = "Ok"
                        };

                        await dialog.ShowAsync();
                    }
                    return;

                case UserNotificationListenerAccessStatus.Unspecified:
                    {
                        var dialog = new ContentDialog
                        {
                            Title = "Error",
                            Content = "UserNotificationListenerAccessStatus.Unspecified. Please retry.",
                            CloseButtonText = "Ok"
                        };

                        await dialog.ShowAsync();
                    }
                    return;
            }

            try
            {
                m_listener.NotificationChanged += Listener_NotificationChanged; // 'Element not found' exception if not running as a UWP process
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "NotificationChanged error",
                    Content = ex.ToString(),
                    CloseButtonText = "Ok"
                };

                await dialog.ShowAsync();
            }
        }

        private async void Listener_NotificationChanged(UserNotificationListener sender, UserNotificationChangedEventArgs args)
        {
            UserNotification notif = m_listener.GetNotification(args.UserNotificationId);
            if (notif == null)
                return;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                title.Text = notif.AppInfo.DisplayInfo.DisplayName; // application name

                // accessing notif.Notification.Visual.Bindings directly will just give the same "ToastGeneric" NotificationBinding
                NotificationBinding toastBinding = notif.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);
                if (toastBinding == null)
                    return;

                content.Text = "";
                IReadOnlyList<AdaptiveNotificationText> elms = toastBinding.GetTextElements();
                foreach (var elm in elms)
                    content.Text += elm.Text + Environment.NewLine;

            });
        }

        private void btnGenerate_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ToastMessage.Generate();
        }
    }
}
