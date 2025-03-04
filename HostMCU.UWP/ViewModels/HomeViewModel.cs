using HostMCU.UWP.Servers;
using System;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;

namespace HostMCU.UWP.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly SerialPortServer serialPortServere = ((App)Application.Current).serialPortServere;
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

            var data = await serialPortServere.ReadDataAsync();

            string pattern = @"(?<=WD:)\d+";
            Match match = Regex.Match(data, pattern);
            if (match.Success && match.Value != "00")
            {
                Temp_Text = match.Value;
            }
            else
            {
                Temp_Text = "NULL";
            }

            Content_Text += data;
        }
    }
}
