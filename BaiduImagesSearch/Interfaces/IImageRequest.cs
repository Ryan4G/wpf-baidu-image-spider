using BaiduImagesSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduImagesSearch.Interfaces
{
    public interface IImageRequest
    {
        string Host { get; }
        string SearchTerm { get; set; }

        string Url { get; }

        IEnumerable<SearchItemResult> Parse(string json);
    }
}
