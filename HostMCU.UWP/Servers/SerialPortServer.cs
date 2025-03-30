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
        private string PortName;
        private uint BaudRate;

        public async Task InitializeSerialPortAsync(string portName, uint baudRate, bool isNeedRefrushParameter = false)
        {
            if (isNeedRefrushParameter)
            {
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

                    Debug.WriteLine("串口已打开: " + portName);
                    IsSerialPortOpen = true;
                }
                else
                {
                    Debug.WriteLine("无法打开串口: " + portName);
                }
            }
            else
            {
                Debug.WriteLine("未找到串口设备: " + portName);
            }
        }

        public void CloseSerialPort()
        {
            if (serialPort != null)
            {
                serialPort.Dispose();
                serialPort = null;
                Debug.WriteLine("串口已关闭");
                IsSerialPortOpen = false;
            }
            else
            {
                Debug.WriteLine("串口未打开");
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

                    await InitializeSerialPortAsync(PortName, BaudRate);
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

                await InitializeSerialPortAsync(PortName, BaudRate);

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
