using Microsoft.Toolkit.Uwp.Notifications;

internal class ToastMessage
{
    public static void Generate()
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
