using MusikPlayer.Commands;
using MusikPlayer.Enum;
using MusikPlayer.Features.PlayList;
using MusikPlayer.FileManager;
using MusikPlayer.Helper;
using MusikPlayer.Logs;
using MusikPlayer.Model;
using MusikPlayer.Repo;
using MusikPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MusikPlayer
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Version_Title { get { return $"Wolfi MP3 Version: {VERSION_INFO}"; } }

        private const double FONZ_SIZE_VALUE = 20d;
        private const string FILE_DATA_NAME = ".mp3";
        private const int DEFAULT_MSG_SHOW_TIME = 50000;
        private const int DEFAULT_VOLUME_VALUE = 20;
        /// <summary>
        /// <para>MAJOR jedes Fette Update(neues Design) eine X.0.0 ändern</para>
        /// <para>MINOR jedes Features eine 0.X.0 ändern</para>
        /// <para>BUILD jeder BugFix eine 0.0.X ändern</para>
        /// </summary>
        private const string VERSION_INFO = "2.3.3";

        private readonly List<string> mediaExtensions = new List<string> { FILE_DATA_NAME };

        private MediaPlayer mediaPlayer;
        private DispatcherTimer timer;
        private Sorting currentSort;
        private FileDirector fileDirector;
        private SoundItemFactory soundItemFactory;
        private SoundItemViewModel lastPlaySoundItem;
        private SoundItemViewModel currentPlaySoundItem;
        private Config config { get { return Config.Instance(); } }
        private SongListClass SongListClassInstance { get { return ListsRepository.Instance.GetInstanceSongs(); } }
        private PlayListClass PlayListClassInstance { get { return ListsRepository.Instance.GetInstancePlayLists(); } }

        public MainWindowViewModel()
        {
            this.LoadSetUp();
        }

        #region Propertys

        public ObservableCollection<SoundItemViewModel> SoundItemsSourceFilter
        {
            set; get;
        }

        public ObservableCollection<PlayListListItem> PlayListItemsSourceFilter
        {
            set; get;
        }

        public SoundItemViewModel SoundSelectedItem
        {
            get => base.GetProperty<SoundItemViewModel>(nameof(this.SoundSelectedItem));
            set
            {
                base.SetProperty(nameof(this.SoundSelectedItem), value);
            }
        }

        public TimeSpan CurrentPlayTimer
        {
            get => base.GetProperty<TimeSpan>(nameof(this.CurrentPlayTimer));
            set
            {
                base.SetProperty(nameof(this.CurrentPlayTimer), value);
                base.SetProperty(nameof(this.ProgressBarValue), TimeSpanConverter.GetSecondsFromTimeSpan(value));
                base.SetProperty(nameof(this.FormatedCurrentPlayTimer), value);
            }
        }

        public string FormatedCurrentPlayTimer
        {
            get { return this.CurrentPlayTimer.ToString(@"hh\:mm\:ss"); ; }
        }

        #region RunText

        public double FontSize { get { return FONZ_SIZE_VALUE; } }

        public string CurrentTrackName
        {
            get => base.GetProperty<string>(nameof(this.CurrentTrackName));
            set
            {
                this.SetRunTextWidths(value);
                base.SetProperty(nameof(this.CurrentTrackName), value);
            }
        }

        public double RunTextFrom
        {
            get => base.GetProperty<double>(nameof(this.RunTextFrom));
            set => base.SetProperty(nameof(this.RunTextFrom), value);
        }

        public double RunTextTo
        {
            get => base.GetProperty<double>(nameof(this.RunTextTo));
            set => base.SetProperty(nameof(this.RunTextTo), value);
        }

        public string RunTextAnimationDuration
        {
            get => base.GetProperty<string>(nameof(this.RunTextAnimationDuration));
            set => base.SetProperty(nameof(this.RunTextAnimationDuration), value);
        }

        #endregion

        public int ProgressBarMax
        {
            get => base.GetProperty<int>(nameof(this.ProgressBarMax));
            set
            {
                base.SetProperty(nameof(this.ProgressBarMax), value);
            }
        }

        public int ProgressBarValue
        {
            get => base.GetProperty<int>(nameof(this.ProgressBarValue));
            set
            {
                base.SetProperty(nameof(this.ProgressBarValue), value);
            }
        }

        public int SoundVolume
        {
            get => base.GetProperty<int>(nameof(this.SoundVolume));
            set
            {
                base.SetProperty(nameof(this.SoundVolume), value);
                this.SetMediaPlayerVolume(value);
                base.SetProperty(nameof(this.SoundSliderVolumeUnit), value);
                this.config.SoundVolume = value;
            }
        }

        public string SoundSliderVolumeUnit
        {
            get { return $"{this.SoundVolume} %"; }
        }

        public string SongTitelSearchWord
        {
            get => base.GetProperty<string>(nameof(this.SongTitelSearchWord));
            set
            {
                base.SetProperty(nameof(this.SongTitelSearchWord), value);
                this.StartSongTitleSearch(value);
            }
        }

        public string PlayListSearchWord
        {
            get => base.GetProperty<string>(nameof(this.PlayListSearchWord));
            set
            {
                base.SetProperty(nameof(this.PlayListSearchWord), value);
                this.StartPlayListSearch(value);
            }
        }


        public bool RepeatActive
        {
            get => base.GetProperty<bool>(nameof(this.RepeatActive));
            set
            {
                base.SetProperty(nameof(this.RepeatActive), value);
                base.OnPropertyChanged(nameof(this.RepeatActiveForView));

            }
        }

        public string RepeatActiveForView
        {
            get { return $"Wiederholung Aktiv: {BoolConverter.ConvertBoolToString(this.RepeatActive)}"; }
        }

        public bool ShuffleActive
        {
            get => base.GetProperty<bool>(nameof(this.ShuffleActive));
            set
            {
                base.SetProperty(nameof(this.ShuffleActive), value);
                base.OnPropertyChanged(nameof(this.ShuffleActiveForView));

            }
        }

        public string ShuffleActiveForView
        {
            get { return $"Zufalls Sequenz Aktiv: {BoolConverter.ConvertBoolToString(this.ShuffleActive)}"; }
        }

        public string ErrorMessageForUser
        {
            get => base.GetProperty<string>(nameof(this.ErrorMessageForUser));
            set
            {
                base.SetProperty(nameof(this.ErrorMessageForUser), value);

                Action<object> action = (object obj) =>
                {
                    Thread.Sleep(this.ShowUserMsgMs);
                    base.SetProperty(nameof(this.ErrorMessageForUser), string.Empty);
                };

                Task errorMessageTask = new Task(action, nameof(errorMessageTask));
                errorMessageTask.Start();
            }
        }

        /// <summary>
        /// bestimmt wv ms die Meldung angezeigt werden soll
        /// </summary>
        public int ShowUserMsgMs
        {
            get => base.GetProperty<int>(nameof(this.ShowUserMsgMs));
            set
            {
                base.SetProperty(nameof(this.ShowUserMsgMs), value);
                this.config.ShowUserMsgMs = value;
            }
        }

        #endregion

        #region Methoden

        #region LaufText Methoden

        private void SetRunTextWidths(string value)
        {
            this.RunTextTo = this.CalculcateRunTextToWidth(value);
            this.RunTextFrom = this.CalculcateRunTextToWidth(value, false);
            base.OnPropertyChanged(nameof(this.RunTextFrom));
            base.OnPropertyChanged(nameof(this.RunTextTo));
        }

        /// <summary>
        /// Gibt anhand der länge des Wortes und der gebidneten Fontsize (Konstante) die Width zurück
        /// </summary>
        /// <param name="value">LaufText</param>
        /// <param name="convertResult">
        /// False-> Ergebniss wird zurück gegeben<para/>
        /// True-> Ergebniss wird mit *-1 zurückgegeben<para/>
        /// </param>
        /// <returns></returns>
        private double CalculcateRunTextToWidth(string value, bool convertResult = true)
        {
            double calculatedRunTextWidth = ((value.Length * FONZ_SIZE_VALUE) / 2) + FONZ_SIZE_VALUE;//das hintere dient als puffer

            if (convertResult)
                return calculatedRunTextWidth * -1;
            else
                return calculatedRunTextWidth;
        }

        #endregion

        public static void OnProgrammShutDown()
        {
            Logger.Instance.RunLogg($"{nameof(MainWindowViewModel)}", $"{nameof(OnProgrammShutDown)}", $"{nameof(OnProgrammShutDown)} Sequenz");

            var soundItemList = new List<SoundItem>();

            foreach (SoundItemViewModel soundItemVM in ListsRepository.Instance.SoundItemsSource)
            {
                soundItemList.Add(soundItemVM.Model);
            }

            JsonDirector.Instance.TrySaveSongDataToJson(soundItemList);
            JsonDirector.Instance.TrySaveConfigToJson(Config.Instance());
            JsonDirector.Instance.TrySavePlayListsToJson(ListsRepository.Instance.PlayListItemsSource.ToList());

            Logger.Instance.RunLogg($"{nameof(MainWindowViewModel)}", $"{nameof(OnProgrammShutDown)}", $"{nameof(OnProgrammShutDown)} Sequenz Ende");

        }

        /// <summary>
        /// Lädt View, Daten und was sonst nocht gebraucht wird.
        /// </summary>
        public void LoadSetUp()
        {
            Logger.Instance.RunLogg(nameof(MainWindowViewModel), nameof(MainWindowViewModel), "Initalisiere Backend");
            this.mediaPlayer = new MediaPlayer();
            this.fileDirector = FileDirector.GetFileDirector();
            this.soundItemFactory = new SoundItemFactory();

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(0.5);
            this.timer.Tick += OnTimerTick;

            this.SoundItemsSourceFilter = new ObservableCollection<SoundItemViewModel>();
            this.PlayListItemsSourceFilter = new ObservableCollection<PlayListListItem>();

            this.ProgressBarMax = 100;
            this.ProgressBarValue = 0;

            this.SetConfig();
            this.FillItemsSource();
            this.FillPlayListRep();


            this.RunTextAnimationDuration = new TimeSpan(0, 0, 0, 15).ToString();

            Logger.Instance.RunLogg(nameof(MainWindowViewModel), nameof(MainWindowViewModel), "Initalisiere Backend Ende Erfolgreich");
        }

        private void OpenPlayList(bool isEdit = false, PlayListListItem playListListItem = null)
        {
            Logger.Instance.RunLogg(nameof(MainWindowViewModel), nameof(OpenPlayList), $"öffnet PlayList als {nameof(isEdit)}={isEdit}");
            AddEditPlayListWindow window = new AddEditPlayListWindow(isEdit, playListListItem);
            window.ShowDialog();

            this.PlayListItemsSourceFilter.Clear();

            foreach (var item in ListsRepository.Instance.PlayListItemsSource)
            {
                this.PlayListItemsSourceFilter.Add(item);
            }
            Logger.Instance.RunLogg(nameof(MainWindowViewModel), nameof(OpenPlayList), "öffnet PlayList Ende Erfolgreich");
        }

        private void StartSongTitleSearch(string searchWord)
        {
            this.SoundItemsSourceFilter = this.SongListClassInstance.GetFilteredSoundsBy(searchWord);

            this.SortItemsListBy(Sorting.Ascending, SortableListViewHeader.Favorite);
        }

        private void StartPlayListSearch(string searchWord)
        {
            this.PlayListItemsSourceFilter = this.PlayListClassInstance.GetFilteredPlayLists(searchWord);

            base.OnPropertyChanged(nameof(ListsRepository.Instance.PlayListItemsSource));
            base.OnPropertyChanged(nameof(this.PlayListItemsSourceFilter));
        }


        private void AddToObservableCollections(SoundItemViewModel soundItemViewModel)
        {
            ListsRepository.Instance.SoundItemsSource.Add(soundItemViewModel);
            this.SoundItemsSourceFilter.Add(soundItemViewModel);
        }

        private void SetConfig()
        {
            Logger.Instance.RunLogg(nameof(MainWindowViewModel), nameof(SetConfig), "Lade Einstellungen");
            var config = JsonDirector.Instance.TryLoadConfigFromJson();

            if (config != null)
            {
                this.SoundVolume = config.SoundVolume.HasValue ? config.SoundVolume.Value : DEFAULT_VOLUME_VALUE;
                this.ShowUserMsgMs = config.ShowUserMsgMs.HasValue ? config.SoundVolume.Value : DEFAULT_MSG_SHOW_TIME;
            }
            else
            {
                this.SoundVolume = DEFAULT_VOLUME_VALUE;
                this.ShowUserMsgMs = DEFAULT_MSG_SHOW_TIME;
            }
            Logger.Instance.RunLogg(nameof(MainWindowViewModel), nameof(SetConfig), "Lade Einstellungen Ende Erfolgreich");
        }

        private void FillItemsSource()
        {
            Logger.Instance.RunLogg(nameof(MainWindowViewModel), nameof(FillItemsSource), "Lade Songs");
            try
            {
                var loadedDatas = JsonDirector.Instance.LoadSongDataFromJson();

                if (loadedDatas == null || loadedDatas.Count == 0)
                {
                    this.LoadDatasByAlphaStart();
                }
                else
                {
                    this.LoadDatasBy(loadedDatas);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ExceptionLogg(nameof(MainWindowViewModel), nameof(FillItemsSource), ex);

                if (ListsRepository.Instance.SoundItemsSource.Count == 0 && SoundItemsSourceFilter.Count == 0)//wenn beim laden über json was falsch gelaufen ist mach mal normal
                {
                    this.LoadDatasByAlphaStart();
                }
            }



            base.OnPropertyChanged(nameof(this.SoundItemsSourceFilter));
            Logger.Instance.RunLogg(nameof(MainWindowViewModel), nameof(FillItemsSource), "Lade Songs Ende Erfolgreich");
        }

        /// <summary>
        /// Lade PLayListen aus Json
        /// </summary>
        private void FillPlayListRep()
        {
            Logger.Instance.RunLogg(nameof(MainWindowViewModel), nameof(FillPlayListRep), "Lade PlayListen");

            try
            {
                var loadedPlayLists = JsonDirector.Instance.TryLoadPlayListsFromJson();

                if (loadedPlayLists != null)
                {
                    ListsRepository.Instance.ReplacePlayListItemsSourceBy(new ObservableCollection<PlayListListItem>(loadedPlayLists));
                    this.PlayListItemsSourceFilter = new ObservableCollection<PlayListListItem>(loadedPlayLists);
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.ExceptionLogg(nameof(MainWindowViewModel), nameof(FillPlayListRep), ex);
            }

            Logger.Instance.RunLogg(nameof(MainWindowViewModel), nameof(FillPlayListRep), "Lade PlayListen Ende Erfolgreich");
        }

        /// <summary>
        /// Lädt erfolgreich geladene soundItems in die view und sortiert diese.
        /// </summary>
        private void LoadDatasBy(List<SoundItem> loadedDatas)
        {
            Logger.Instance.RunLogg(nameof(MainWindowViewModel), nameof(LoadDatasBy), "Lade Songs aus vorhandender Datei");
            foreach (SoundItem item in loadedDatas)
            {
                var soundItemVM = new SoundItemViewModel(item);
                soundItemVM.OnFavStateChanged += OnSoundItemFavStateChanged;

                if (this.fileDirector.ExistFile(soundItemVM.Model.FilePath))
                    this.AddToObservableCollections(soundItemVM);
            }

            this.SortItemsListBy(Sorting.Ascending, SortableListViewHeader.Favorite);
            Logger.Instance.RunLogg(nameof(MainWindowViewModel), nameof(LoadDatasBy), "Lade Songs aus vorhandender Datei Ende Erfolgreich");
        }

        /// <summary>
        /// Sucht aufm pc nach jeder mp3 Datei und fügt diese der View hinzu.(Dauert lange weil duration mit geupdatet werden muss)
        /// </summary>
        private void LoadDatasByAlphaStart()
        {
            Logger.Instance.RunLogg(nameof(MainWindowViewModel), nameof(LoadDatasByAlphaStart), "Durchsuche Pc nach Songs und lade Sie");
            this.AddSongsBy(fileDirector.GetAllFilesFromDevice(this.mediaExtensions), true);
            this.UpdateDurationOnViewThread();
            Logger.Instance.RunLogg(nameof(MainWindowViewModel), nameof(LoadDatasByAlphaStart), "Durchsuche Pc nach Songs und lade Sie Ende Erfolgreich");
        }

        private void ManageStartMusic(SoundItemViewModel soundSelectedItem)
        {
            this.CurrentTrackName = soundSelectedItem.NameToShow;
            this.SetMediaPlayerVolume(this.SoundVolume);
            this.mediaPlayer.MediaFailed += (o, e) =>
            {

                Logger.Instance.ExceptionLogg(nameof(MainWindowViewModel), nameof(ManageStartMusic), new Exception($"{o}, {e.ErrorException}"));
            };
            this.mediaPlayer.MediaEnded += (o, e) =>
            {
                if (!this.CheckShuffle(soundSelectedItem))//wenn shuffle false dann check repeat
                    this.CheckRepeat(soundSelectedItem);
            };
            this.mediaPlayer.MediaOpened += (o, e) =>
            {
                this.ProgressBarMax = (int)this.mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            };

            this.mediaPlayer.Open(soundSelectedItem.Model.Uri);

            this.mediaPlayer.Play();
            this.timer.Start();

            this.UpdateTimerUnit();
        }

        private void UpdateTimerUnit()
        {
            try
            {
                this.CurrentPlayTimer = this.mediaPlayer.Position;
            }
            catch (Exception ex)
            {
                Logger.Instance.ExceptionLogg(nameof(MainWindowViewModel), nameof(UpdateTimerUnit), ex);
            };
        }

        private void CheckRepeat(SoundItemViewModel soundSelectedItem)
        {
            if (soundSelectedItem == null)
                throw new NullReferenceException($"{nameof(MainWindowViewModel)},{nameof(CheckRepeat)},{nameof(SoundSelectedItem)} ist null");

            if (this.RepeatActive)
            {
                this.TryRepeatCurrentSong(soundSelectedItem);
            }
        }

        private bool CheckShuffle(SoundItemViewModel soundSelectedItem)
        {
            if (this.ShuffleActive)
            {
                this.ManageStartMusic(this.SongListClassInstance.GetRandomSong(soundSelectedItem));
                return true;
            }
            return false;
        }

        private void SetMediaPlayerVolume(int value)
        {
            try
            {
                //mediplayer has double value from 0.00 to 1.00

                double volume = (double)value / (double)100;
                this.mediaPlayer.Volume = volume;
            }
            catch (Exception ex)
            {
                Logger.Instance.ExceptionLogg(nameof(MainWindowViewModel), nameof(SetMediaPlayerVolume), ex);
            }
        }

        private void AddSongsBy(List<string> listOfFilePaths, bool isStartUp = false)
        {
            foreach (var item in listOfFilePaths)
            {
                this.AddSongBy(item, isStartUp);
            }
        }

        private void AddSongBy(string filePath, bool isStartUp = false)
        {
            SoundItemViewModel soundItemVM = this.soundItemFactory.CreateListItem(filePath);
            soundItemVM.OnFavStateChanged += OnSoundItemFavStateChanged;

            if (!this.SongListClassInstance.ExistSoundName(soundItemVM))
            {
                this.AddToObservableCollections(soundItemVM);
                this.SortItemsListBy(Sorting.Ascending, SortableListViewHeader.Duration);
            }
            else
            {
                if (!isStartUp)
                    this.ErrorMessageForUser = "You cant add same song who exist on list";
            }

            this.SoundSelectedItem = soundItemVM;
        }

        private void AddSongToPlayList()
        {
            //    Logger.Instance.RunLogg(nameof(MainWindowViewModel), nameof(AddSongToPlayList), $"Füge {nameof()} zur PlayList {nameof()} hinzu");

            //    Logger.Instance.RunLogg(nameof(MainWindowViewModel), nameof(AddSongToPlayList), $"Füge {nameof()} zur PlayList {nameof()} hinzu Ende Erfolgreich");
        }

        private void SortItemsListBy(Sorting sorting, SortableListViewHeader header)
        {
            this.currentSort = sorting;

            Sorter sorter = new Sorter();

            ListsRepository.Instance.ReplaceSoundItemsSourceBy(sorter.GetSortedListBy(ListsRepository.Instance.SoundItemsSource, sorting, header));
            this.SoundItemsSourceFilter = sorter.GetSortedListBy(this.SoundItemsSourceFilter, sorting, header);

            base.OnPropertyChanged(nameof(ListsRepository.Instance.SoundItemsSource));
            base.OnPropertyChanged(nameof(this.SoundItemsSourceFilter));
        }

        private void UpdateDurationOnViewThread()
        {
            Thread updateDurationThread = new Thread(this.UpdateDuration);
            updateDurationThread.Name = nameof(updateDurationThread);
            updateDurationThread.Start();
        }

        private void UpdateDuration()
        {
            this.SongListClassInstance.UpdateDuration();

            this.SortItemsListBy(Sorting.Ascending, SortableListViewHeader.Duration);
        }

        private bool TryRepeatCurrentSong(SoundItemViewModel soundSelectedItem)
        {
            try
            {
                this.ManageStartMusic(soundSelectedItem);
            }
            catch (Exception ex)
            {
                Logger.Instance.ExceptionLogg(nameof(MainWindowViewModel), nameof(TryRepeatCurrentSong), ex);
                return false;
            }
            return true;
        }


        #endregion

        #region Commands + Events 

        private void OnStartMusic(SoundItemViewModel soundItemViewModel)
        {
            if (soundItemViewModel == null)
                throw new NullReferenceException($"{nameof(MainWindowViewModel)},{nameof(OnStartMusic)},{nameof(soundItemViewModel)} ist null!");

            this.ManageStartMusic(soundItemViewModel);
        }

        private void OnSoundItemFavStateChanged(SoundItemViewModel obj)
        {
            this.SortItemsListBy(this.currentSort, SortableListViewHeader.Favorite);
            base.OnPropertyChanged(nameof(ListsRepository.Instance.SoundItemsSource));
        }

        private void OnBreakMusic()
        {
            if (!this.SoundSelectedItem.IsBreak)
            {
                if (this.mediaPlayer.HasAudio)
                {
                    this.timer.Stop();
                    this.mediaPlayer.Pause();
                    this.SoundSelectedItem.IsBreak = true;
                }
            }
            else
            {
                if (this.mediaPlayer.HasAudio)
                {
                    this.timer.Start();
                    this.mediaPlayer.Play();
                    this.SoundSelectedItem.IsBreak = false;
                }
            }
        }

        public ICommand StartMusic => new RelayCommand(o =>
        {
            try
            {
                if (o is SoundItemViewModel sivm)
                    this.OnStartMusic(sivm);
            }
            catch (Exception ex)
            {
                Logger.Instance.ExceptionLogg(nameof(MainWindowViewModel), nameof(StartMusic), ex);
            }
        });

        public ICommand BreakMusic => new RelayCommand(o =>
        {
            try
            {
                this.OnBreakMusic();
            }
            catch (Exception ex)
            {
                Logger.Instance.ExceptionLogg(nameof(MainWindowViewModel), nameof(BreakMusic), ex);
            }
        });

        public ICommand RepeatMusic => new RelayCommand(o =>
        {
            if (this.RepeatActive)
                this.RepeatActive = false;
            else
                this.RepeatActive = true;
        });

        public ICommand ShuffleMusic => new RelayCommand(o =>
        {
            if (this.ShuffleActive)
                this.ShuffleActive = false;
            else
                this.ShuffleActive = true;
        });

        public ICommand OpenSettings => new DelegateCommand(() =>
        {
            System.Diagnostics.Debug.WriteLine("Settings open");
        });

        #region PlayListStuff

        public ICommand AddToPlaylist => new RelayCommand(o =>
        {
            if (o is SoundItemViewModel)
            {
                this.AddSongToPlayList();
            }
            else
                Logger.Instance.ExceptionLogg(nameof(MainWindowViewModel), nameof(AddToPlaylist), new Exception($"Objekt ist kein {nameof(SoundItemViewModel)}"));
        });


        public ICommand CreatePlaylist => new DelegateCommand(() =>
        {
            this.OpenPlayList();
        });

        public ICommand EditPlayList => new RelayCommand(o =>
        {
            if (o != null && o is PlayListListItem editItem)
            {
                this.OpenPlayList(true, editItem);
            }
            else
                Logger.Instance.ExceptionLogg(nameof(MainWindowViewModel), nameof(EditPlayList), new Exception($"Objekt ist kein {nameof(PlayListListItem)}"));
        });

        public ICommand DeletePlayList => new RelayCommand(o =>
        {
            try
            {
                if (o != null && o is PlayListListItem item)
                {
                    ListsRepository.Instance.PlayListItemsSource.Remove(item);
                    this.PlayListItemsSourceFilter.Remove(item);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ExceptionLogg($"{nameof(MainWindowViewModel)}", $"{nameof(this.DeletePlayList)}", ex);
            }
        });

        #endregion

        #region SortColumns Commands

        public ICommand SortDuration => new RelayCommand(o =>
        {
            try
            {
                if (this.currentSort == Sorting.Ascending)
                    this.SortItemsListBy(Sorting.Descending, SortableListViewHeader.Duration);
                else
                    this.SortItemsListBy(Sorting.Ascending, SortableListViewHeader.Duration);
            }
            catch (Exception ex)
            {
                Logger.Instance.ExceptionLogg(nameof(MainWindowViewModel), nameof(SortDuration), ex);
            }
        });

        public ICommand SortTitel => new RelayCommand(o =>
        {
            try
            {
                if (this.currentSort == Sorting.Ascending)
                    this.SortItemsListBy(Sorting.Descending, SortableListViewHeader.Title);
                else
                    this.SortItemsListBy(Sorting.Ascending, SortableListViewHeader.Title);
            }
            catch (Exception ex)
            {
                Logger.Instance.ExceptionLogg(nameof(MainWindowViewModel), nameof(SortTitel), ex);
            }
        });

        #endregion

        private void OnTimerTick(object sender, EventArgs e)
        {
            try
            {
                this.UpdateTimerUnit();
            }
            catch (Exception ex)
            {
                Logger.Instance.ExceptionLogg(nameof(MainWindowViewModel), nameof(OnTimerTick), ex);
            }
        }

        #endregion
    }
}
