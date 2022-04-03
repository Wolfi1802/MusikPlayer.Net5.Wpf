using MusikPlayer.Enum;
using MusikPlayer.Logs;
using MusikPlayer.Model;
using MusikPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikPlayer.Helper
{
    public class Sorter
    {
        public ObservableCollection<SoundItemViewModel> GetSortedListBy(ObservableCollection<SoundItemViewModel> listToSort, Sorting sorting, SortableListViewHeader header)
        {
            var tempList = new ObservableCollection<SoundItemViewModel>(listToSort);
            var favoriteList = new ObservableCollection<SoundItemViewModel>();

            foreach (var item in tempList)
            {
                if (item.IsFavorite)
                {
                    try
                    {

                        favoriteList.Add(item);
                        listToSort.Remove(item);
                    }
                    catch(Exception ex)
                    {
                        Logger.Instance.ExceptionLogg(nameof(Sorter), nameof(GetSortedListBy), ex, "Sonderzeichen sind hier nicht erlaubt");
                    }
                }
            }

            if (sorting == Sorting.Ascending)
                favoriteList = this.GetSortedItemsListBy(favoriteList, Sorting.Descending, header);
            else
                favoriteList = this.GetSortedItemsListBy(favoriteList, Sorting.Ascending, header);

            var sortedList = this.GetSortedItemsListBy(listToSort, sorting, header);


            foreach (var item in favoriteList)
            {
                sortedList.Insert(0, item);
            }

            return sortedList;
        }
       
        private ObservableCollection<SoundItemViewModel> GetSortedItemsListBy(ObservableCollection<SoundItemViewModel> listToSort, Sorting sorting, SortableListViewHeader header)
        {
            var sortedList = new ObservableCollection<SoundItemViewModel>();

            switch (header)
            {
                case SortableListViewHeader.Duration:
                    if (sorting == Sorting.Ascending)
                        sortedList = this.GetDurationOrderByAscending(listToSort);
                    else
                        sortedList = this.GetDurationOrderByDescending(listToSort);
                    break;
                case SortableListViewHeader.Favorite:
                    if (sorting == Sorting.Ascending)
                        sortedList = this.GetFavoriteOrderByAscending(listToSort);
                    else
                        sortedList = this.GetFavoriteOrderByDescending(listToSort);
                    break;
                case SortableListViewHeader.Title:
                    if (sorting == Sorting.Ascending)
                        sortedList = this.GetTitleOrderByAscending(listToSort);
                    else
                        sortedList = this.GetTitleOrderByDescending(listToSort);
                    break;
                case SortableListViewHeader.Default:
                    break;
                default:
                    break;
            }

            return sortedList;
        }

        private ObservableCollection<SoundItemViewModel> GetDurationOrderByAscending(ObservableCollection<SoundItemViewModel> listToSort)
        {
            var sortedItemsSource = listToSort.OrderBy(x => x.DurationToShow);
            listToSort = new ObservableCollection<SoundItemViewModel>(sortedItemsSource);

            return listToSort;
        }

        private ObservableCollection<SoundItemViewModel> GetDurationOrderByDescending(ObservableCollection<SoundItemViewModel> listToSort)
        {
            var sortedItemsSource = listToSort.OrderByDescending(x => x.DurationToShow);
            listToSort = new ObservableCollection<SoundItemViewModel>(sortedItemsSource);

            return listToSort;
        }

        private ObservableCollection<SoundItemViewModel> GetTitleOrderByAscending(ObservableCollection<SoundItemViewModel> listToSort)
        {
            var sortedItemsSource = listToSort.OrderBy(x => x.NameToShow);
            listToSort = new ObservableCollection<SoundItemViewModel>(sortedItemsSource);

            return listToSort;
        }

        private ObservableCollection<SoundItemViewModel> GetTitleOrderByDescending(ObservableCollection<SoundItemViewModel> listToSort)
        {
            var sortedItemsSource = listToSort.OrderByDescending(x => x.NameToShow);
            listToSort = new ObservableCollection<SoundItemViewModel>(sortedItemsSource);

            return listToSort;
        }

        private ObservableCollection<SoundItemViewModel> GetFavoriteOrderByAscending(ObservableCollection<SoundItemViewModel> listToSort)
        {
            var sortedItemsSource = listToSort.OrderBy(x => x.IsFavorite);
            listToSort = new ObservableCollection<SoundItemViewModel>(sortedItemsSource);

            return listToSort;
        }

        private ObservableCollection<SoundItemViewModel> GetFavoriteOrderByDescending(ObservableCollection<SoundItemViewModel> listToSort)
        {
            var sortedItemsSource = listToSort.OrderByDescending(x => x.IsFavorite);
            listToSort = new ObservableCollection<SoundItemViewModel>(sortedItemsSource);

            return listToSort;
        }
    }
}
