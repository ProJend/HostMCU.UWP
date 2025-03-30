using HostMCU.UWP.Servers;
using System;
using Windows.UI.Xaml;

namespace HostMCU.UWP.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly SerialPortServer serialPortServere = ((App)Application.Current).serialPortServere;
        public bool? IsSerialPortOpen { get; set; }

        public string Content_Text { get; set; }
        public string Temp_Text { get; set; }
        public string Mois_Text { get; set; }

        private readonly DispatcherTimer _UpdateSerialPortStatusTimer = new() { Interval = TimeSpan.FromSeconds(1) };

        public HomeViewModel()
        {
            _UpdateSerialPortStatusTimer.Tick += UpdateSerialPortStatusTimer_Tick;
            _UpdateSerialPortStatusTimer.Start();
        }

        private async void UpdateSerialPortStatusTimer_Tick(object sender, object e)
        {
            IsSerialPortOpen = serialPortServere.IsSerialPortOpen;

            var data = await serialPortServere.ReadDataAsync();
            Temp_Text = serialPortServere.GetValueFromPattern(data, @"(?<=WD:)\d+");
            Mois_Text = serialPortServere.GetValueFromPattern(data, @"(?<=SD:)\d+");
            Content_Text += data;
        }
    }
}
