using HostMCU.UWP.Servers;
using System;
using Windows.UI.Xaml;

namespace HostMCU.UWP.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        readonly SerialPortServer serialPortServere = ((App)Application.Current).serialPortServere;
        public bool? IsSerialPortOpen { get; set; }

        public string Content_Text { get; set; }

        private readonly DispatcherTimer _UpdateSerialPortStatusTimer = new() { Interval = TimeSpan.FromSeconds(60) };

        public HomeViewModel()
        {
            _UpdateSerialPortStatusTimer.Tick += UpdateSerialPortStatusTimer_Tick;
            _UpdateSerialPortStatusTimer.Start();
        }

        private async void UpdateSerialPortStatusTimer_Tick(object sender, object e)
        {
            IsSerialPortOpen = serialPortServere.IsSerialPortOpen;

            Content_Text += await serialPortServere.ReadDataAsync();
        }
    }
}
