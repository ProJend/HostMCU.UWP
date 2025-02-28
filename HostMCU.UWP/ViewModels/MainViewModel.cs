using HostMCU.UWP.Servers;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;

namespace HostMCU.UWP.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        SerialPortServer serialPortServere = ((App)Application.Current).serialPortServere;

        public string ToggleButton_Content { get; set; }


        public ObservableCollection<ComboBoxOption> PortName { get; set; }

        public MainViewModel()
        {
            ToggleButton_Content = "打开串口";
            MainViewModelAsync();
        }

        private async void MainViewModelAsync()
        {
            PortName = await serialPortServere.EnumerateSerialPortsAsync();
        }
    }

    public class ComboBoxOption(string name)
    {
        public string Name { get; set; } = name;
    }
}
