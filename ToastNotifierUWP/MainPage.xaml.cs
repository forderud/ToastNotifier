using System;
using System.Collections.Generic;
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
        private ToastMessage m_toast;

        public MainPage()
        {
            this.InitializeComponent();
            InitializeListener();
        }

        private async void InitializeListener()
        {
            try
            {
                m_toast = new ToastMessage(UpdateUI);
            } catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "Ok"
                };

                await dialog.ShowAsync();

            }
        }

        private void btnGenerate_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            m_toast.Generate();
        }

        private async void UpdateUI(UserNotification notif)
        {
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
    }
}
