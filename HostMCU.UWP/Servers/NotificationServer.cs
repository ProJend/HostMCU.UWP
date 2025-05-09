using HostMCU.UWP.Controls;
using HostMCU.UWP.Models.Type;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace HostMCU.UWP.Servers
{
    public class NotificationServer
    {
        private readonly SerialPortServer serialPortServere = ((App)Application.Current).serialPortServere;

        public static void ShowToast(ToastType name, double? value = null)
        {
            ToastContent ms = new();
            switch (name)
            {
                case ToastType.Reconnecting:
                    ms = ToastTemplates.Reconnecting();
                    break;
                case ToastType.TempWarning:
                    ms = ToastTemplates.TempWarning(value);
                    break;
                case ToastType.MoisWarning:
                    ms = ToastTemplates.MoisWarning(value);
                    break;
            }

            // Create the notification
            var notif = new ToastNotification(ms.GetXml())
            {
                ExpirationTime = DateTime.Now.AddMinutes(1)
            };

            // And show it!
            ToastNotificationManager.CreateToastNotifier().Show(notif);
        }

        public static async Task ShowDialogType(DialogType name)
        {
            //await ms.ShowAsync();
        }

        public static async Task<bool> ShowMessageDialog(string title, string content, Uri uri = null)
        {
            MessageDialog messageDialog = new(content, title);
            messageDialog.Commands.Add(new UICommand()
            {
                Label = "确定",
                Id = true,
                Invoked = async (cmd) =>
                {
                    if (uri != null)
                        await Launcher.LaunchUriAsync(uri);
                }
            });
            messageDialog.Commands.Add(new UICommand() { Label = "取消", Id = false });
            var result = await messageDialog.ShowAsync();
            return (bool)result.Id;
        }

        public static void ShowMessageToast(TeachingTipType name, int seconds = 0)
        {
            //ms.IsOpen = true;
        }

        public static async void ShowMessageToast(string title, string subtitle, int seconds = 0)
        {
            var ms = new TeachingTipTemplates().DefaultTeachingTip;
            ms.Title = title;
            ms.Subtitle = subtitle;

            ms.IsOpen = true;
            if (seconds != 0)
            {
                var milliseconds = seconds * 1000;
                await Task.Delay(milliseconds);
                ms.IsOpen = false;
            }
        }

        public void ShowTempWarning(double? value = null)
        {
            if (serialPortServere.IsSerialPortOpen.GetValueOrDefault())
                serialPortServere.SwitchBuzzer(100);
            ShowToast(ToastType.TempWarning, value);
        }

        public void ShowMoisWarning(double? value = null)
        {
            if (serialPortServere.IsSerialPortOpen.GetValueOrDefault())
                serialPortServere.SwitchBuzzer(100);
            ShowToast(ToastType.TempWarning, value);
        }
    }
}
