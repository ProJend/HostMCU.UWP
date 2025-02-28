using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using HostMCU.UWP.ViewModels;

namespace HostMCU.UWP.Servers
{
    public class SerialPortServer
    {
        public SerialDevice serialPort;
        public bool? IsSerialPortOpen { get; set; }

        public async Task InitializeSerialPortAsync(string portName, uint baudRate)
        {
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

                    Console.WriteLine("串口已打开: " + portName);
                    IsSerialPortOpen = true;
                }
                else
                {
                    Console.WriteLine("无法打开串口: " + portName);
                }
            }
            else
            {
                Console.WriteLine("未找到串口设备: " + portName);
            }
        }

        public void CloseSerialPort()
        {
            if (serialPort != null)
            {
                serialPort.Dispose();
                serialPort = null;
                Console.WriteLine("串口已关闭");
                IsSerialPortOpen = false;
            }
            else
            {
                Console.WriteLine("串口未打开");
            }
        }

        public async Task<string> ReadDataAsync()
        {
            if (serialPort != null)
            {
                var dataReader = new DataReader(serialPort.InputStream);

                while (IsSerialPortOpen.GetValueOrDefault())
                {
                    try
                    {
                        // 读取数据
                        uint bytesRead = await dataReader.LoadAsync(1024);
                        if (bytesRead > 0)
                        {
                            string data = dataReader.ReadString(bytesRead);
                            Console.WriteLine("收到数据: " + data);
                            return data;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("读取数据时出错: " + ex.Message);
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("串口未打开");
            }
            return "";
        }

        public async Task WriteDataAsync(string data)
        {
            if (serialPort != null)
            {
                var dataWriter = new DataWriter(serialPort.OutputStream);
                dataWriter.WriteString(data);
                await dataWriter.StoreAsync();
                dataWriter.DetachStream();
                Console.WriteLine("数据已发送: " + data);
            }
            else
            {
                Console.WriteLine("串口未打开");
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
            Debug.WriteLine(2);
        }

        private void OnDeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (IsSerialPortOpen == true)
                IsSerialPortOpen = false;
        }
    }
}
