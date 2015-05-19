using System.Collections.ObjectModel;
using EmptyKeys.UserInterface.Mvvm;

namespace EmptyKeys.GearsetModel
{
    /// <summary>
    /// MVVM View Model for the Finder Window.
    /// </summary>
    public class FinderWindowViewModel : WindowViewModel
    {
        public delegate FinderResult SearchFunction(string queryString);

        public class FinderResult : ObservableCollection<ObjectDescription> { }

        FinderResult _items = new FinderResult();
        public FinderResult Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        string _searchText = string.Empty;
        public string SearchText
        {
            get { return _searchText; }
            set 
            {
                SearchTextEmpty = (value ?? string.Empty).Length <= 0;

                SetProperty(ref _searchText, value); 
            }
        }

        bool _searchTextEmpty;
        public bool SearchTextEmpty
        {
            get { return _searchTextEmpty; }
            private set { SetProperty(ref _searchTextEmpty, value); }
        }

        public FinderWindowViewModel()
        {
            Title = "Finder";
        }
    }
}
