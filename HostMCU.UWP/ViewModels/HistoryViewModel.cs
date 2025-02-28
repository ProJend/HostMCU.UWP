using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using HostMCU.UWP.Models;

namespace HostMCU.UWP.ViewModels
{
    public class HistoryViewModel : ObservableCollection<HistoryModel>, ISupportIncrementalLoading
    {
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count) => AsyncInfo.Run(c => LoadMoreItemsAsyncCore(c, count));


        public bool HasMoreItems => true;

        async Task<LoadMoreItemsResult> LoadMoreItemsAsyncCore(CancellationToken cancel, uint count)
        {
            var res = new LoadMoreItemsResult();

            // 如果操作已处于取消状态，则不再加载项
            if (cancel.IsCancellationRequested)
            {
                res.Count = 0;
            }
            else
            {
                var latestItem = new HistoryModel { Temperature = 23, Date = "2025-01-01" };
                Add(latestItem);
            }
            return res;
        }
    }
}
