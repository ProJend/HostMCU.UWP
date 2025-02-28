using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HostMCU.UWP.Servers;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace HostMCU.UWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page
    {
        SerialPortServer serialPortServere = ((App)Application.Current).serialPortServere;

        public HomePage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await serialPortServere.WriteDataAsync("1");
        }
    }
}
