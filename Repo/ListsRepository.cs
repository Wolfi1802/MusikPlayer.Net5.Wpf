using MusikPlayer.Features.PlayList;
using MusikPlayer.Model;
using MusikPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikPlayer.Repo
{
    /// <summary>
    /// Diese Klasse beinhällt alle Listen mit denen Lgobal geabreitet werden soll.
    /// Sie darf nicht vererbt werden!
    /// </summary>
    public sealed class ListsRepository
    {
        public static ListsRepository Instance { get { return lazy.Value; } }

        private static readonly Lazy<ListsRepository> lazy = new Lazy<ListsRepository>(() => new ListsRepository());

        private SongListClass slc;
        private PlayListClass plc;

        private ListsRepository()
        {
            this.SoundItemsSource = new ObservableCollection<SoundItemViewModel>();
            this.PlayListItemsSource = new ObservableCollection<PlayListListItem>();
            this.slc = new SongListClass();
            this.plc = new PlayListClass();
        }

        public ObservableCollection<SoundItemViewModel> SoundItemsSource { private set; get; }

        public ObservableCollection<PlayListListItem> PlayListItemsSource { private set; get; }

        #region Methoden

        public void ReplaceSoundItemsSourceBy(ObservableCollection<SoundItemViewModel> list)
        {
            this.SoundItemsSource = list;
        }

        public void ReplacePlayListItemsSourceBy(ObservableCollection<PlayListListItem> list)
        {
            this.PlayListItemsSource = list;
        }

        public SongListClass GetInstanceSongs()
        {
            return this.slc;
        }
        public PlayListClass GetInstancePlayLists()
        {
            return this.plc;
        }

        public ObservableCollection<SoundItemViewModel> GetAllFavorites()
        {
            var favList = new ObservableCollection<SoundItemViewModel>();

            foreach (SoundItemViewModel soundItem in this.SoundItemsSource)
            {
                if (soundItem.IsFavorite)
                    favList.Add(soundItem);
            }

            return favList;
        }

        public ObservableCollection<PlayListItemViewModel> GetAviableSounds()
        {
            var newList = new ObservableCollection<PlayListItemViewModel>();

            foreach (var item in this.SoundItemsSource)
            {
                newList.Add(new PlayListItemViewModel(this.plc.CreateItem(item.Model)));
            }

            return newList;
        }

        #endregion
    }
}
