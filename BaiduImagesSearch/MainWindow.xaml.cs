using BaiduImagesSearch.Interfaces;
using BaiduImagesSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BaiduImagesSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IImageRequest _imageRequest;

        private SearchInfo _searchInfo;

        private object _lockList = new object();

        private CancellationTokenSource _cts;
        public MainWindow()
        {
            InitializeComponent();

            _searchInfo = new SearchInfo();

            _imageRequest = new BaiduRequest();

            this.DataContext = _searchInfo;

            // Method3: On Background Thread Fill List
            BindingOperations.EnableCollectionSynchronization(_searchInfo.List, _lockList);
        }

        private void OnClear(object sender, RoutedEventArgs e)
        {
            _searchInfo.SearchTerm = string.Empty;
        }

        /// <summary>
        /// Search on sync pattern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearchAsync(object sender, RoutedEventArgs e)
        {
            _imageRequest.SearchTerm = _searchInfo.SearchTerm;

            if (string.IsNullOrWhiteSpace(_imageRequest.SearchTerm))
            {
                return;
            }

            var client = new WebClient();

            client.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            client.Headers.Add("Host", _imageRequest.Host);
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            var resp = client.DownloadString(_imageRequest.Url);

            IEnumerable<SearchItemResult> images = _imageRequest.Parse(resp);
            _searchInfo.List.Clear();

            foreach (var image in images)
            {
                _searchInfo.List.Add(image);
            }
        }

        private void OnSearchAsyncPattern(object sender, RoutedEventArgs e)
        {
            _imageRequest.SearchTerm = _searchInfo.SearchTerm;

            if (string.IsNullOrWhiteSpace(_imageRequest.SearchTerm))
            {
                return;
            }

            // declare a delegate
            Func<string, string> downloadMethod = url =>
            {
                var client = new WebClient();

                client.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
                client.Headers.Add("Host", _imageRequest.Host);
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                return client.DownloadString(url);
            };

            // list add item
            Action<SearchItemResult> addItem = item => _searchInfo.List.Add(item);

            {
                // WARNING: .netCore don't support the Begin/EndInvoke method.
                downloadMethod.BeginInvoke(_imageRequest.Url, ar =>
                {

                    // wait async return result
                    var resp = downloadMethod.EndInvoke(ar);

                    IEnumerable<SearchItemResult> images = _imageRequest.Parse(resp);
                    _searchInfo.List.Clear();

                    foreach (var image in images)
                    {
                        // UI Thread update list
                        this.Dispatcher.Invoke(addItem, image);
                    }
                }, null);
            }

            // Method2: convert Begin/EndInvoke to task based method..
            //{
            //    string resp = Task<string>.Factory.FromAsync<string>(
            //        downloadMethod.BeginInvoke, downloadMethod.EndInvoke, _imageRequest.Url, null
            //        ).GetAwaiter().GetResult();

            //    IEnumerable<SearchItemResult> images = _imageRequest.Parse(resp);
            //    _searchInfo.List.Clear();

            //    foreach (var image in images)
            //    {
            //        // UI Thread update list
            //        this.Dispatcher.Invoke(addItem, image);
            //    }
            //}
        }

        private void OnAsyncEventPattern(object sender, RoutedEventArgs e)
        {
            _imageRequest.SearchTerm = _searchInfo.SearchTerm;

            if (string.IsNullOrWhiteSpace(_imageRequest.SearchTerm))
            {
                return;
            }

            var client = new WebClient();

            client.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            client.Headers.Add("Host", _imageRequest.Host);
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");


            // subscribe the action event
            client.DownloadStringCompleted += (sender1, e1) =>
            {
                var resp = e1.Result;

                IEnumerable<SearchItemResult> images = _imageRequest.Parse(resp);
                _searchInfo.List.Clear();

                foreach (var image in images)
                {
                    _searchInfo.List.Add(image);
                }
            };

            client.DownloadStringAsync(new Uri(_imageRequest.Url));
        }

        /// <summary>
        /// use c# 6.0 new keyword async/await
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnAsyncTaskBasedPattern(object sender, RoutedEventArgs e)
        {
            _imageRequest.SearchTerm = _searchInfo.SearchTerm;

            if (string.IsNullOrWhiteSpace(_imageRequest.SearchTerm))
            {
                return;
            }

            _cts = new CancellationTokenSource();
            try
            {
                /*
                
                var client = new WebClient();

                client.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
                client.Headers.Add("Host", _imageRequest.Host);
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

                // await action completed
                var resp = await client.DownloadStringTaskAsync(_imageRequest.Url);

                */

                // with cancel token
                var clientHttp = new System.Net.Http.HttpClient();
                clientHttp.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
                clientHttp.DefaultRequestHeaders.Add("Host", _imageRequest.Host);
                clientHttp.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

                var respHttp = await clientHttp.GetAsync(_imageRequest.Url, _cts.Token);
                var resp = await respHttp.Content.ReadAsStringAsync();


                // Method1: Work in UI Thread, if serializer work long time, UI will not response other action..
                //{
                //    IEnumerable<SearchItemResult> images = _imageRequest.Parse(resp);
                //    _searchInfo.List.Clear();

                //    foreach (var image in images)
                //    {
                //        _searchInfo.List.Add(image);
                //    }
                //}

                // Method2: serializer work in background, UI Thread add list..
                // WARNING: can not add list item in background, will throw exception
                //{
                //    IEnumerable<SearchItemResult> images = null;

                //    await Task.Run(() =>
                //    {
                //        images = _imageRequest.Parse(resp);
                //    }); 

                //    _searchInfo.List.Clear();

                //    if (images != null)
                //    {
                //        foreach (var image in images)
                //        {
                //            _searchInfo.List.Add(image);
                //        }
                //    }
                //}

                await Task.Run(() =>
                {
                    IEnumerable<SearchItemResult> images = _imageRequest.Parse(resp);
                    _searchInfo.List.Clear();

                    foreach (var image in images)
                    {
                        _searchInfo.List.Add(image);
                    }
                });
            }
            catch(OperationCanceledException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnTaskCancel(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
        }
    }
}
