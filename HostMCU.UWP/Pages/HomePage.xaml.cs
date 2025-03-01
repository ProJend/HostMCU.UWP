using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HostMCU.UWP.Servers;
using System;
using HostMCU.UWP.ViewModels;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace HostMCU.UWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page
    {
        SerialPortServer serialPortServere = ((App)Application.Current).serialPortServere;
        HomeViewModel homeViewModel = new();

        public HomePage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = sender as FrameworkElement;
            switch (button.Tag as string)
            {
                case "SwitchLED": await serialPortServere.WriteDataAsync("1"); break;
                case "SwitchBuzzer": await serialPortServere.WriteDataAsync("2"); break;
                case "SwitchFan": await serialPortServere.WriteDataAsync("3"); break;
            }
        }

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await serialPortServere.WriteDataAsync("4");
        }
    }
}
