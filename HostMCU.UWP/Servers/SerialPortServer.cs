using HostMCU.UWP.Models.Type;
using HostMCU.UWP.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace HostMCU.UWP.Servers
{
    public class SerialPortServer
    {
        public SerialDevice serialPort;

        public bool? IsSerialPortOpen;
        private string FullPortName;
        private string PortName;
        private uint BaudRate;

        public async Task InitializeSerialPortAsync(string fullPortName, string portName, uint baudRate, bool isNeedRefrushParameter = false)
        {
            if (isNeedRefrushParameter)
            {
                FullPortName = fullPortName;
                PortName = portName;
                BaudRate = baudRate;
            }

            string aqs = SerialDevice.GetDeviceSelector(portName);
            var devices = await DeviceInformation.FindAllAsync(aqs);
            if (devices.Count > 0)
            {
                var deviceInfo = devices[0];
                serialPort = await SerialDevice.FromIdAsync(deviceInfo.Id);
                if (serialPort != null)
                {
                    serialPort.BaudRate = baudRate;
                    serialPort.Parity = SerialParity.None;
                    serialPort.DataBits = 8;
                    serialPort.StopBits = SerialStopBitCount.One;
                    serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                    serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);

                    if (isNeedRefrushParameter)
                    {
                        NotificationServer.ShowMessageToast("串口已打开", FullPortName, 3);
                    }
                    else
                    {
                        NotificationServer.ShowMessageToast("串口已重连", FullPortName, 3);
                    }
                    IsSerialPortOpen = true;
                }
                else
                {
                    NotificationServer.ShowMessageToast("无法打开串口", FullPortName, 3);
                }
            }
            else
            {
                NotificationServer.ShowMessageToast("未找到串口设备", FullPortName, 3);
            }
        }

        public void CloseSerialPort()
        {
            if (serialPort != null)
            {
                serialPort.Dispose();
                serialPort = null;
                NotificationServer.ShowMessageToast("串口已关闭", FullPortName, 3);
                IsSerialPortOpen = false;
            }
            else
            {
                NotificationServer.ShowMessageToast("串口未打开", FullPortName, 3);
            }
        }

        public async Task<string> ReadDataAsync()
        {
            if (serialPort != null)
            {
                var dataReader = new DataReader(serialPort.InputStream);

                try
                {
                    // 读取数据
                    uint bytesRead = await dataReader.LoadAsync(20);
                    if (bytesRead > 0)
                    {
                        string data = dataReader.ReadString(bytesRead);
                        Debug.WriteLine("收到数据: " + data);
                        data = data.Replace("\r\n", Environment.NewLine);
                        return data;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("读取数据时出错: " + ex.Message);
                    NotificationServer.ShowToast(ToastType.Reconnecting);

                    await InitializeSerialPortAsync(FullPortName, PortName, BaudRate);
                }
            }
            else
            {
                Debug.WriteLine("串口未打开");
            }
            return "";
        }

        public async Task WriteDataAsync(string data)
        {
            try
            {
                if (serialPort != null)
                {
                    var dataWriter = new DataWriter(serialPort.OutputStream);
                    dataWriter.WriteString(data);
                    await dataWriter.StoreAsync();
                    dataWriter.DetachStream();
                    Debug.WriteLine("数据已发送: " + data);
                }
                else
                {
                    Debug.WriteLine("串口未打开");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("发送数据时出错: " + ex.Message);
                NotificationServer.ShowToast(ToastType.Reconnecting);

                await InitializeSerialPortAsync(FullPortName, PortName, BaudRate);
            }

        }

        public async Task<ObservableCollection<ComboBoxOption>> EnumerateSerialPortsAsync()
        {
            string aqsFilter = SerialDevice.GetDeviceSelector();

            var deviceCollection = await DeviceInformation.FindAllAsync(aqsFilter);
            DeviceWatcher watcher = DeviceInformation.CreateWatcher(aqsFilter);
            //watcher.Added += OnDeviceAdded;
            watcher.Removed += OnDeviceRemoved;
            watcher.Start();

            ObservableCollection<ComboBoxOption> serialPortOptions = [];
            foreach (var device in deviceCollection)
            {
                serialPortOptions.Add(new ComboBoxOption(device.Name));
            }

            return serialPortOptions;
        }

        private void OnDeviceAdded(DeviceWatcher sender, DeviceInformation args)
        {

        }

        private void OnDeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (IsSerialPortOpen == true)
                IsSerialPortOpen = false;
        }
    }
}
