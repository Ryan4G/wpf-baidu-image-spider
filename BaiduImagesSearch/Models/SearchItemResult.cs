using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduImagesSearch.Models
{
    public class SearchItemResult : BindableBase
    {
        private string _title;

        private string _url;

        private string _thumbnailUrl;

        private string _source;

        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string Url { get => _url; set => SetProperty(ref _url, value); }
        public string ThumbnailUrl { get => _thumbnailUrl; set => SetProperty(ref _thumbnailUrl, value); }
        public string Source { get => _source; set => SetProperty(ref _source, value); }
    }
}
