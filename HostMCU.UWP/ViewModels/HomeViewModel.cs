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

        private double? temp_Value { get; set; }
        public double? Temp_Value
        {
            get => temp_Value;
            set
            {
                temp_Value = value;
                NeedShowTempWarning(value);
            }
        }

        private double min_Temp { get; set; }
        public double Min_Temp
        {
            get => min_Temp;
            set
            {
                min_Temp = value;
                NeedShowTempWarning(Temp_Value);
            }
        }

        private double max_Temp { get; set; }
        public double Max_Temp
        {
            get => max_Temp;
            set
            {
                max_Temp = value;
                NeedShowTempWarning(Temp_Value);
            }
        }

        private double? mois_Value { get; set; }
        public double? Mois_Value
        {
            get => mois_Value;
            set
            {
                mois_Value = value;
                NeedShowMoisWarning(value);
            }
        }

        private double min_Mois { get; set; }
        public double Min_Mois
        {
            get => min_Mois;
            set
            {
                min_Mois = value;
                NeedShowMoisWarning(Mois_Value);
            }
        }

        private double max_Mois { get; set; }
        public double Max_Mois
        {
            get => max_Mois;
            set
            {
                max_Mois = value;
                NeedShowMoisWarning(Mois_Value);
            }
        }

        private void NeedShowTempWarning(double? value)
        {
            if (value < Min_Temp || value > Max_Temp)
            {
                new NotificationServer().ShowTempWarning(value);
            }
        }

        private void NeedShowMoisWarning(double? value)
        {
            if (value < Min_Temp || value > Max_Temp)
            {
                new NotificationServer().ShowMoisWarning(value);
            }
        }

        private readonly DispatcherTimer _UpdateSerialPortStatusTimer = new() { Interval = TimeSpan.FromSeconds(2) };

        public HomeViewModel()
        {
            IsOnlyInstructionInfo = true;

            _UpdateSerialPortStatusTimer.Tick += UpdateSerialPortStatusTimer_Tick;
            _UpdateSerialPortStatusTimer.Start();

            Max_Mois = 100;
            Max_Temp = 100;
        }

        private async void UpdateSerialPortStatusTimer_Tick(object sender, object e)
        {
            DataProcessor dataProcessor = new();
            IsSerialPortOpen = serialPortServere.IsSerialPortOpen;

            var data = await serialPortServere.ReadDataAsync();
            Temp_Value = dataProcessor.GetValueFromPattern(data, @"(?<=WD:)\d+");
            Mois_Value = dataProcessor.GetValueFromPattern(data, @"(?<=SD:)\d+");
            if (!IsOnlyInstructionInfo)
            {
                Content_Text += data;
            }
        }
    }
}
