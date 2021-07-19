using System.Collections.ObjectModel;

namespace BaiduImagesSearch.Models
{
    public class SearchInfo : BindableBase
    {
        private string _searchTerm;

        private ObservableCollection<SearchItemResult> _list;

        public SearchInfo()
        {
            _list = new ObservableCollection<SearchItemResult>();
            _list.CollectionChanged += delegate { OnPropertyChanged("List"); };
        }

        public string SearchTerm { get => _searchTerm; set => SetProperty(ref _searchTerm, value); }
        public ObservableCollection<SearchItemResult> List => _list;
    }
}
