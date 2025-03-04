using HostMCU.UWP.Servers;
using System;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;

namespace HostMCU.UWP.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        readonly SerialPortServer serialPortServere = ((App)Application.Current).serialPortServere;
        public bool? IsSerialPortOpen { get; set; }

        public string Content_Text { get; set; }
        public string Temp_Text { get; set; }


        private readonly DispatcherTimer _UpdateSerialPortStatusTimer = new() { Interval = TimeSpan.FromSeconds(5) };

        public HomeViewModel()
        {
            _UpdateSerialPortStatusTimer.Tick += UpdateSerialPortStatusTimer_Tick;
            _UpdateSerialPortStatusTimer.Start();
        }

        private async void UpdateSerialPortStatusTimer_Tick(object sender, object e)
        {
            IsSerialPortOpen = serialPortServere.IsSerialPortOpen;

            var a = await serialPortServere.ReadDataAsync();

            string pattern = @"(?<=WD:)\d+";
            Match match = Regex.Match(a, pattern);
            if (match.Success)
            {
                Temp_Text = match.Value;
            }

            Content_Text += a;
        }
    }
}
