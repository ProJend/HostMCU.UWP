using HostMCU.UWP.Servers;
using HostMCU.UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace HostMCU.UWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly SerialPortServer serialPortServere = ((App)Application.Current).serialPortServere;
        private readonly MainViewModel mainViewModel = new();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested; // 订阅系统返回事件
            Window.Current.CoreWindow.PointerPressed += Mouse_BackRequested; // 订阅鼠标返回事件
            Window.Current.SetTitleBar(AppTitleBar); // 设置新的标题栏

            // Add keyboard accelerators for backwards navigation.
            var goBack = new KeyboardAccelerator { Key = VirtualKey.Escape };
            goBack.Invoked += Keyboard_BackRequested;
            KeyboardAccelerators.Add(goBack);
        }

        #region NavigationView
        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {   // Add handler for frame navigation.
            frame.Navigated += frame_Navigated;

            // NavView doesn't load any page by default, so load home page.
            NavView.SelectedItem = NavView.MenuItems[0];

            // If navigation occurs on SelectionChanged, this isn't needed.
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            NavView_Navigate("home", new SuppressNavigationTransitionInfo());
        }

        // List of ValueTuple holding the Navigation Tag and the relative Navigation Page
        private readonly List<(string Tag, Type Page)> _pages =
        [
            ("home", typeof(HomePage)),
            ("history", typeof(HistoryPage)),
            ("algorithm", typeof(AlgorithmPage)),
        ];

        /// <summary>
        /// 应用内返回页面
        /// </summary>
        private void NavView_BackRequested(muxc.NavigationView sender, muxc.NavigationViewBackRequestedEventArgs args) => TryGoBack();

        /// <summary>
        /// 键盘外返回页面
        /// </summary>
        private void Keyboard_BackRequested(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) => TryGoBack();

        /// <summary>
        /// 系统外返回页面
        /// </summary>
        private void System_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!frame.CanGoBack) return;
            frame.GoBack();
            e.Handled = true;
        }

        /// <summary>
        /// 鼠标外返回页面
        /// Invoked on every mouse click, touch screen tap, or equivalent interaction.
        /// Used to detect browser-style next and previous mouse button clicks
        /// to navigate between pages.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void Mouse_BackRequested(CoreWindow sender, PointerEventArgs e)
        {
            var properties = e.CurrentPoint.Properties;
            // Ignore button chords with the left, right, and middle buttons
            if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed) return;

            // If back or forward are pressed (but not both) navigate appropriately
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                e.Handled = true;
                if (backPressed) TryGoBack();
                //if (forwardPressed) this.TryGoForward();
            }
        }

        /// <summary>
        /// 尝试执行返回页面操作
        /// </summary>
        private bool TryGoBack()
        {
            if (!frame.CanGoBack) return false;

            // Don't go back if the nav pane is overlayed.
            if (NavView.IsPaneOpen &&
                (NavView.DisplayMode == muxc.NavigationViewDisplayMode.Compact ||
                 NavView.DisplayMode == muxc.NavigationViewDisplayMode.Minimal))
                return false;
            frame.GoBack();
            return true;
        }

        private void NavView_ItemInvoked(muxc.NavigationView sender, muxc.NavigationViewItemInvokedEventArgs args)
        {
            var navItemTag = args.InvokedItemContainer.Tag.ToString();
            if (args.IsSettingsInvoked) navItemTag = "settings";
            if (args.InvokedItemContainer != null)
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
        }

        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "settings")
            {
                _page = typeof(SettingsPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }

            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = frame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (!(_page is null) && !Equals(preNavPageType, _page)) frame.Navigate(_page, null, transitionInfo);
        }

        private void frame_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = frame.CanGoBack;
            if (frame.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                NavView.SelectedItem = (muxc.NavigationViewItem)NavView.SettingsItem;
                NavView.Header = "设置";
            }
            else if (frame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);
                NavView.SelectedItem = NavView.MenuItems.OfType<muxc.NavigationViewItem>().First(n => n.Tag.Equals(item.Tag));
                var header = ((muxc.NavigationViewItem)NavView.SelectedItem)?.Content?.ToString().ToUpper();
                NavView.AlwaysShowHeader = header != "主页";
                NavView.Header = header;
            }
        }
        #endregion

        private async void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (serialPortServere.IsSerialPortOpen.GetValueOrDefault())
            {
                serialPortServere.CloseSerialPort();
            }
            else
            {
                if (PortName.SelectedIndex == -1)
                {
                    PortName.SelectedIndex = mainViewModel.PortNameCollection.Count - 1;
                }
                if (PortName.SelectedItem is ComboBoxOption selectedItem && BaudRate.SelectedItem is ComboBoxItem selectedItem2)
                {
                    string pattern = @"COM\d+";
                    var match = Regex.Match(selectedItem.Name, pattern);
                    if (!match.Success)
                    {
                        ToggleButton.IsChecked = false;
                        return;
                    }
                    string portName = match.Value;
                    uint.TryParse(selectedItem2.Content as string, out uint baudRate);

                    await serialPortServere.InitializeSerialPortAsync(selectedItem.Name, portName, baudRate, true);
                }
                else
                {
                    ToggleButton.IsChecked = false;
                }
            }
        }
    }
}
