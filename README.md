Custom Windows "toast" notification application. Intended as replacement for the in-built "toast" viewer on embedded systems where `explorer.exe` is not running due to a custom shell. Based on the Toast [Notification listener](https://docs.microsoft.com/en-us/windows/apps/design/shell/tiles-and-notifications/notification-listener) article.

**LIMITATION**: Actions like buttons are _not_ supported, since do not appear to be accessible through the API.

Related forum questions:
* https://techcommunity.microsoft.com/t5/microsoft-intune/install-amp-restart-notifications-when-using-custom-shell/m-p/3300210
* https://docs.microsoft.com/en-us/answers/questions/836756/how-to-access-actionsbuttons-from-windowsuinotific.html
* https://github.com/microsoft/WindowsAppSDK/discussions/2471


## Example screenshots
Original toast notification:

![OriginalToast](OriginalToast.png) 

Custom display of the same toast notification:

![CustomToast](CustomToast.png) 
