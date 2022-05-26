using Microsoft.Toolkit.Uwp.Notifications;
using System;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

internal class ToastMessage
{
    public delegate void ToastNotificationCallback(UserNotification notif);

    private uint s_counter = 0;
    private UserNotificationListener m_listener;
    private ToastNotificationCallback m_callback;

    public ToastMessage(ToastNotificationCallback cb)
    {
        m_callback = cb; // must be done before calling Initialize()
        Initialize();
    }

    private async void Initialize() {
        if (!ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener"))
            throw new Exception("UserNotificationListener not supported (too old Windows version).");

        // get listener for the current user
        m_listener = UserNotificationListener.Current;

        // request access to the user's notifications (must be called from UI thread)
        UserNotificationListenerAccessStatus accessStatus = await m_listener.RequestAccessAsync();
        switch (accessStatus)
        {
            case UserNotificationListenerAccessStatus.Allowed:
                break; // success

            case UserNotificationListenerAccessStatus.Denied:
                throw new Exception("UserNotificationListenerAccessStatus.Denied.");

            case UserNotificationListenerAccessStatus.Unspecified:
                throw new Exception("UserNotificationListenerAccessStatus.Unspecified. Please retry.");
        }

        m_listener.NotificationChanged += Listener_NotificationChanged; // 'Element not found' exception if not running as a UWP process
    }

    private void Listener_NotificationChanged(UserNotificationListener sender, UserNotificationChangedEventArgs args)
    {
        UserNotification notif = m_listener.GetNotification(args.UserNotificationId);
        if (notif == null)
            return;

        m_callback(notif);
    }

    public UserNotification GetNotification(uint UserNotificationId)
    {
        return m_listener.GetNotification(UserNotificationId);
    }

    public void Generate()
    {
        // generate a test toast
        // SetBackgroundActivation will prevent launching a new instance of this app when the user clicks on the toast
        new ToastContentBuilder()
            .AddText($"SW installation {s_counter++}")
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
