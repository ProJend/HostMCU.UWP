using Microsoft.Toolkit.Uwp.Notifications;

namespace HostMCU.UWP.Controls
{
    public class ToastTemplates
    {
        public static ToastContent Reconnecting() => new ToastContentBuilder()
            .AddArgument("conversationId", 98143)
            .AddText("串口意外失去连接")
            .AddText("尝试重连中...")
            .AddButton(new ToastButton()
                .SetContent("关闭")
                .SetDismissActivation())
            .GetToastContent();

        public static ToastContent TempWarning(double? value) => new ToastContentBuilder()
           .AddArgument("conversationId", 98144)
           .AddText("检测到温度不正常")
           .AddText($"当前温度：{value} ℃")
           .AddButton(new ToastButton()
               .SetContent("关闭")
               .SetDismissActivation())
           .GetToastContent();

        public static ToastContent MoisWarning(double? value) => new ToastContentBuilder()
           .AddArgument("conversationId", 98145)
           .AddText("检测到湿度不正常")
           .AddText($"当前湿度：{value} %")
           .AddButton(new ToastButton()
               .SetContent("关闭")
               .SetDismissActivation())
           .GetToastContent();
    }
}
