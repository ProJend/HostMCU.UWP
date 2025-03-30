using System;
using Windows.UI.Xaml.Data;

namespace HostMCU.UWP.Converters
{
    public class FeatureStateToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter == null)
            {
                return "";
            }
            var isClicked = (bool)value;
            return parameter.ToString() switch
            {
                "SwitchLED" => isClicked ? "关闭 LED 灯" : "打开 LED 灯",
                "SwitchBuzzer" => isClicked ? "关闭蜂鸣器" : "打开蜂鸣器",
                "SwitchFan" => isClicked ? "关闭风扇" : "打开风扇",
                "SwitchFanDirection" => isClicked ? "正向" : "反向",
                _ => "",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
