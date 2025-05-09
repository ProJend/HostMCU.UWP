using HostMCU.UWP.Models.Model;
using HostMCU.UWP.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace HostMCU.UWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HistoryPage : Page
    {
        private readonly HistoryViewModel HistoryViewModel = [];
        public HistoryPage()
        {
            this.InitializeComponent();
        }

        // 获取菜单项的 DataContext，应该是 HistoryModel 对象
        void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            // 获取菜单项的 DataContext，应该是 HistoryModel 对象
            var historyModel = (sender as FrameworkElement)?.DataContext as HistoryModel;
            if (historyModel != null)
            {
                // 从 ViewModel 或集合中删除该项
                var model = (sender as FrameworkElement).DataContext as dynamic; // 获取您的 ViewModel，可能是一个 ViewModel 实例
                HistoryViewModel.Remove(historyModel);
            }
        }
    }
}
