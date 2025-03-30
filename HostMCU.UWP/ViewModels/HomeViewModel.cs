using HostMCU.UWP.Helpers;
using HostMCU.UWP.Servers;
using System;
using Windows.UI.Xaml;

namespace HostMCU.UWP.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly SerialPortServer serialPortServere = ((App)Application.Current).serialPortServere;
        public bool? IsSerialPortOpen { get; set; }

        public bool IsOnlyInstructionInfo { get; set; }
        public string Content_Text { get; set; }
        public string Temp_Text { get; set; }
        public string Mois_Text { get; set; }

        private readonly DispatcherTimer _UpdateSerialPortStatusTimer = new() { Interval = TimeSpan.FromSeconds(2) };

        public HomeViewModel()
        {
            IsOnlyInstructionInfo = true;

            _UpdateSerialPortStatusTimer.Tick += UpdateSerialPortStatusTimer_Tick;
            _UpdateSerialPortStatusTimer.Start();
        }

        private async void UpdateSerialPortStatusTimer_Tick(object sender, object e)
        {
            DataProcessor dataProcessor = new();
            IsSerialPortOpen = serialPortServere.IsSerialPortOpen;

            var data = await serialPortServere.ReadDataAsync();
            Temp_Text = dataProcessor.GetValueFromPattern(data, @"(?<=WD:)\d+");
            Mois_Text = dataProcessor.GetValueFromPattern(data, @"(?<=SD:)\d+");
            if (!IsOnlyInstructionInfo)
            {
                Content_Text += data;
            }
        }
    }
}
