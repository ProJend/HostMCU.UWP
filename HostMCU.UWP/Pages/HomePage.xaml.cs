using HostMCU.UWP.Servers;
using HostMCU.UWP.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

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

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var toggleButton = sender as ToggleButton;
            switch (toggleButton.Tag as string)
            {
                case "SwitchLED":
                    serialPortServere.SwitchLED(toggleButton.IsChecked.GetValueOrDefault() ? PWMSwitchLED.Value : 0);
                    break;
                case "SwitchBuzzer":
                    serialPortServere.SwitchBuzzer(toggleButton.IsChecked.GetValueOrDefault() ? PWMSwitchBuzzer.Value : 0);
                    break;
                case "SwitchFan":
                    serialPortServere.SwitchFan(toggleButton.IsChecked.GetValueOrDefault() ? PWMSwitchFan.Value : 0, IsDoubleTapEnabled);
                    break;
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            serialPortServere.SwitchFan(SwitchFan.IsChecked.GetValueOrDefault() ? PWMSwitchFan.Value : 0, SwitchFanDirection.IsDoubleTapEnabled);
            SwitchFanDirection.IsDoubleTapEnabled = !SwitchFanDirection.IsDoubleTapEnabled;
        }

        private void Slider_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            var slider = sender as Slider;
            switch (slider.Tag as string)
            {
                case "PWMSwitchLED":
                    serialPortServere.SwitchLED(PWMSwitchLED.Value);
                    break;
                case "PWMSwitchBuzzer":
                    serialPortServere.SwitchBuzzer(PWMSwitchBuzzer.Value);
                    break;
                case "PWMSwitchFan":
                    serialPortServere.SwitchFan(PWMSwitchFan.Value, IsDoubleTapEnabled);
                    break;
            }
        }
    }
}
