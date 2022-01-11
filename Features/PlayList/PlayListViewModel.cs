using MusikPlayer.Commands;
using MusikPlayer.Model;
using MusikPlayer.Repo;
using MusikPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusikPlayer.Features.PlayList
{
    public class PlayListViewModel : ViewModelBase
    {
        private Action CloseWindow;
        private ListsRepository ListRepo;
        private bool isEdit;
        private PlayListListItem toEditplItem;

        public PlayListViewModel(AddEditPlayListWindow window, bool isEdit, PlayListListItem playListListItem = null)
        {
            this.CloseWindow = () =>
                {
                    window.Close();
                };

            this.SetTitleWindow(isEdit);
            this.ListRepo = ListsRepository.Instance;
            this.AllAviableSounds = new ObservableCollection<PlayListItemViewModel>(ListRepo.GetAviableSounds());
            this.listOfSelectedSounds = new List<PlayListItemViewModel>();
            this.toEditplItem = playListListItem;
            this.isEdit = isEdit;

            if (isEdit)
                this.LoadDatas(playListListItem);
        }

        #region Propertys

        private List<PlayListItemViewModel> listOfSelectedSounds { set; get; }

        public string TitleWindow
        {
            get => base.GetProperty<string>(nameof(this.TitleWindow));
            set => base.SetProperty(nameof(this.TitleWindow), value);
        }

        public string TitelPlayList
        {
            get => base.GetProperty<string>(nameof(this.TitelPlayList));
            set => base.SetProperty(nameof(this.TitelPlayList), value);
        }

        public ObservableCollection<PlayListItemViewModel> AllAviableSounds { set; get; }

        #endregion

        #region Methoden

        public void SaveDatas()
        {
            this.listOfSelectedSounds.AddRange(this.GetAllSelectedSounds());

            //kapseln
            PlayListListItem listItem = new PlayListListItem();
            listItem.Name = this.TitelPlayList;
            listItem.ListOfSounds = this.listOfSelectedSounds;
            listItem.DurationOfPlayList = this.GetDurationOfList(this.listOfSelectedSounds);
            listItem.CountOfSongs = this.listOfSelectedSounds.Count;
            //

            if (this.isEdit)
                this.ListRepo.PlayListItemsSource.Remove(this.toEditplItem);

            this.ListRepo.PlayListItemsSource.Add(listItem);
        }

        private void LoadDatas(PlayListListItem playListListItem)
        {
            this.AllAviableSounds.Clear();
            this.toEditplItem = playListListItem;
            this.TitelPlayList = playListListItem.Name;
            bool allowToAdd;

            foreach (var item in playListListItem.ListOfSounds)
            {
                this.AllAviableSounds.Add(item);
            }

            foreach (var aviableSong in this.ListRepo.GetAviableSounds())
            {
                if (!this.ExistOnSoundList(aviableSong))
                    this.AllAviableSounds.Add(aviableSong);
            }
        }

        private bool ExistOnSoundList(PlayListItemViewModel aviableSong)
        {
            foreach (var currentSong in this.AllAviableSounds)
            {
                if (aviableSong.NameToShow.Equals(currentSong.NameToShow))
                    return true;
            }
            return false;
        }

        private ObservableCollection<PlayListItemViewModel> GetAllSelectedSounds()
        {
            var temp = new ObservableCollection<PlayListItemViewModel>();

            foreach (var item in this.AllAviableSounds)
            {
                if (item.IsSelected)
                    temp.Add(item);
            }

            return temp;
        }


        private void SetTitleWindow(bool isEdit)
        {
            this.TitleWindow = isEdit ? "Ändere die PlayList" : "Erstelle eine neue PlayList";
        }

        private TimeSpan GetDurationOfList(List<PlayListItemViewModel> ListOfSounds)
        {
            TimeSpan temp = new TimeSpan();

            foreach (var item in ListOfSounds)
            {
                temp += item.Model.Duration;
            }

            return temp;
        }

        #endregion

        #region Commands

        public ICommand AcceptCommand => new DelegateCommand(() =>
        {
            this.SaveDatas();
            this.CloseWindow?.Invoke();
        });

        public ICommand CancelCommand => new DelegateCommand(() =>
        {
            this.CloseWindow?.Invoke();
        });

        #endregion

    }
}
