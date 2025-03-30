using HostMCU.UWP.Servers;
using HostMCU.UWP.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace HostMCU.UWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private readonly SerialPortServer serialPortServere = ((App)Application.Current).serialPortServere;
        private readonly HomeViewModel homeViewModel = new();

        public HomePage()
        {
            this.InitializeComponent();
        }

        private async void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = sender as FrameworkElement;
            switch (button.Tag as string)
            {
                case "SwitchLED":
                    await serialPortServere.WriteDataAsync("1");
                    Content.Text += "Started 1\r\n";
                    break;
                case "SwitchBuzzer":
                    await serialPortServere.WriteDataAsync("2");
                    Content.Text += "Started 2\r\n";
                    break;
                case "SwitchFan":
                    await serialPortServere.WriteDataAsync("3");
                    Content.Text += "Started 3\r\n";
                    break;
            }
        }

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await serialPortServere.WriteDataAsync("4");
            Content.Text += "Started 4\r\n";
            SwitchFanDirection.IsDoubleTapEnabled = !SwitchFanDirection.IsDoubleTapEnabled;
        }
    }
}
