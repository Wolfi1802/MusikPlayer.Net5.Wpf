using MusikPlayer.Model;
using MusikPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikPlayer.Repo
{
    public class PlayListClass
    {

        public PlayListItemModel CreateItem(SoundItem soundItem)
        {
            PlayListItemModel playListItem = new PlayListItemModel();
            playListItem.Name = soundItem.Name;
            playListItem.Duration = soundItem.Duration;
            playListItem.IsSelected = false;
            playListItem.DurationString = soundItem.DurationString;

            return playListItem;
        }

        public ObservableCollection<PlayListListItem> GetFilteredPlayLists(string searchWord)
        {
            ObservableCollection<PlayListListItem> filteredSounds = new ObservableCollection<PlayListListItem>();

            if (string.IsNullOrEmpty(searchWord))
            {
                //zeige alle an 
                return new ObservableCollection<PlayListListItem>(ListsRepository.Instance.PlayListItemsSource);
            }
            else
            {
                foreach (PlayListListItem soundItem in ListsRepository.Instance.PlayListItemsSource)
                {
                    if (soundItem.Name.ToLower().Contains(searchWord.ToLower()))
                    {
                        filteredSounds.Add(soundItem);
                    }
                }

                return new ObservableCollection<PlayListListItem>(filteredSounds);
            }
        }
    }
}
