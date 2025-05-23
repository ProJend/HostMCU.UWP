﻿using HostMCU.UWP.Models.Model;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

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
                Random random = new Random();

                double temperature = Math.Round(random.NextDouble() * (28 - 26) + 26, 2);
                double moisture = random.Next(70, 81);

                var latestItem = new HistoryModel { Temperature = temperature, Moisture = moisture, Date = "2025-01-01" };
                Add(latestItem);
            }
            return res;
        }
    }
}
