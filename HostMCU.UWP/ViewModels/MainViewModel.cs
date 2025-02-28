using HostMCU.UWP.Servers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;

namespace HostMCU.UWP.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        SerialPortServer serialPortServere = ((App)Application.Current).serialPortServere;
        public bool? IsSerialPortOpen { get; set; }

        public string ToggleButton_Content => IsSerialPortOpen switch
        {
            true => "已连接",
            false => "未连接",
            _ => "打开串口",
        };

        public ObservableCollection<ComboBoxOption> PortNameCollection { get; set; } = [];
        public string TheLastPortPlaceholderText => PortNameCollection.Count == 0 ? "无" : PortNameCollection[PortNameCollection.Count - 1].Name;

        private readonly DispatcherTimer _UpdateSerialPortStatusTimer = new() { Interval = TimeSpan.FromSeconds(1) };

        public MainViewModel()
        {
            _UpdateSerialPortStatusTimer.Tick += _UpdateSerialPortStatusTimer_Tick;
            _UpdateSerialPortStatusTimer.Start();
        }

        private async void _UpdateSerialPortStatusTimer_Tick(object sender, object e)
        {
            IsSerialPortOpen = serialPortServere.IsSerialPortOpen;

            var availablePorts = await serialPortServere.EnumerateSerialPortsAsync();
            if (PortNameCollection.Count != availablePorts.Count)
            {
                // 如果列表发生变化，更新 PortName
                PortNameCollection = availablePorts;
            }
        }

    }

    public class ComboBoxOption(string name)
    {
        public string Name { get; set; } = name;
    }
}
