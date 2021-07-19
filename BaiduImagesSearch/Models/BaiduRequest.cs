using BaiduImagesSearch.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace BaiduImagesSearch.Models
{
    public class BaiduRequest : IImageRequest
    {
        public int Count { get; set; }
        public int Offset { get; set; }
        public BaiduRequest()
        {
            Count = 30;
            Offset = 0;
        }

        public string Host => "image.baidu.com";

        private string _searchTerm;
        public string SearchTerm { get => _searchTerm; set => _searchTerm = value; }

        public string Url => "https://image.baidu.com/search/acjson?tn=resultjson_com&logid=11497653579379518727&ipn=rj&ct=201326592&is=&fp=result" +
            $"&queryWord={System.Web.HttpUtility.UrlEncode(_searchTerm)}&cl=2&lm=-1&ie=utf-8&oe=utf-8&adpicid=&st=-1&z=&ic=&hd=&latest=&copyright=&word={System.Web.HttpUtility.UrlEncode(_searchTerm)}" +
            $"&s=&se=&tab=&width=&height=&face=&istype=&qc=&nc=1&fr=&expermode=&nojc=&pn={Count + Offset}&rn={Count}&gsm=1e&{DateTime.Now.Ticks}=";

        public IEnumerable<SearchItemResult> Parse(string json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(BaiduImageJsonResult));

            List<SearchItemResult> itemList = new List<SearchItemResult>();

            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    BaiduImageJsonResult imageResult = serializer.ReadObject(ms) as BaiduImageJsonResult;

                    if (imageResult != null)
                    {
                        foreach(var item in imageResult.data)
                        {
                            itemList.Add(new SearchItemResult
                            {
                                Title = item.fromPageTitleEnc,
                                ThumbnailUrl = item.thumbURL,
                                Url = item.thumbURL,
                                Source = item.thumbURL
                            });
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("error");
            }

            return itemList;
        }
    }
}
