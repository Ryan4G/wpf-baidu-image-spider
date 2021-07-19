using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduImagesSearch.Models
{
    public class BaiduImageJsonResult
    {
        public string queryExt { get; set; }

        public int listNum { get; set; }

        public IEnumerable<BaiduImageData> data { get; set; }
    }

    public class BaiduImageData
    {
        public string thumbURL { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string type { get; set; }
        public string fromPageTitleEnc { get; set; }
    }
}
