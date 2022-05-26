using Microsoft.Toolkit.Uwp.Notifications;

internal class ToastMessage
{
    static uint s_counter = 0;

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
