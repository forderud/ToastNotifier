using Microsoft.Toolkit.Uwp.Notifications;
using System;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

internal class ToastMessage
{
    static uint s_counter = 0;

    private UserNotificationListener m_listener;

    public ToastMessage(TypedEventHandler<UserNotificationListener, UserNotificationChangedEventArgs> listener)
    {
        Initialize(listener);
    }

    private async void Initialize(TypedEventHandler<UserNotificationListener, UserNotificationChangedEventArgs> listener) {
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

        m_listener.NotificationChanged += listener; // 'Element not found' exception if not running as a UWP process
    }

    public UserNotification GetNotification(uint UserNotificationId)
    {
        return m_listener.GetNotification(UserNotificationId);
    }

    public static void Generate()
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
