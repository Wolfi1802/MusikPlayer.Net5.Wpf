using MusikPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusikPlayer.Repo
{
    public class SongListClass
    {

        public void UpdateDuration()
        {
            ObservableCollection<SoundItemViewModel> toUpdate = new ObservableCollection<SoundItemViewModel>(ListsRepository.Instance.SoundItemsSource);
            int countToUpdate = ListsRepository.Instance.SoundItemsSource.Count;

            while (toUpdate.Count != 0)
            {
                Thread.Sleep(1);
                toUpdate.Clear();

                foreach (var item in ListsRepository.Instance.SoundItemsSource)
                {
                    if (!item.UpdateDurationDone)
                    {
                        toUpdate.Add(item);
                    }
                }
            }
        }

        public bool ExistSoundName(SoundItemViewModel soundItemVM)
        {
            return ListsRepository.Instance.SoundItemsSource.Any(x => x.NameToShow == soundItemVM.NameToShow);
        }

        public SoundItemViewModel GetRandomSong(SoundItemViewModel soundSelectedItem)
        {

            Random random = new Random();
            bool whileActive = true;

            while (whileActive)
            {
                var randomNumber = random.Next(0, ListsRepository.Instance.SoundItemsSource.Count);
                if (ListsRepository.Instance.SoundItemsSource[randomNumber] != soundSelectedItem)
                {
                    whileActive = false;
                    return ListsRepository.Instance.SoundItemsSource[randomNumber];
                }
            }
            return null;
        }

        public ObservableCollection<SoundItemViewModel> GetFilteredSoundsBy(string searchWord)
        {
            ObservableCollection<SoundItemViewModel> filteredSounds = new ObservableCollection<SoundItemViewModel>();

            if (string.IsNullOrEmpty(searchWord))
            {
                //zeige alle an 
                return new ObservableCollection<SoundItemViewModel>(ListsRepository.Instance.SoundItemsSource);
            }
            else
            {
                foreach (SoundItemViewModel soundItem in ListsRepository.Instance.SoundItemsSource)
                {
                    if (soundItem.NameToShow.ToLower().Contains(searchWord.ToLower()))
                    {
                        filteredSounds.Add(soundItem);
                    }
                }

                return  new ObservableCollection<SoundItemViewModel>(filteredSounds);
            }
        }
    }
}
