using MusikPlayer.Commands;
using MusikPlayer.Enum;
using MusikPlayer.FileManager;
using MusikPlayer.Helper;
using MusikPlayer.Model;
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
        private const string VERSION_INFO = "2.2.0";

        private readonly List<string> mediaExtensions = new List<string> { FILE_DATA_NAME };

        private MediaPlayer mediaPlayer;
        private DispatcherTimer timer;
        private Sorting currentSort;
        private FileDirector fileDirector;
        private SoundItemFactory soundItemFactory;
        private SoundItemViewModel lastPlaySoundItem;
        private SoundItemViewModel currentPlaySoundItem;
        private Config config { get { return Config.Instance(); } }



        public MainWindowViewModel()
        {
            this.mediaPlayer = new MediaPlayer();
            this.fileDirector = FileDirector.GetFileDirector();
            this.soundItemFactory = new SoundItemFactory();

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(0.5);
            this.timer.Tick += OnTimerTick;

            if (SoundItemsSource == null)
                SoundItemsSource = new ObservableCollection<SoundItemViewModel>();

            this.SoundItemsSourceFilter = new ObservableCollection<SoundItemViewModel>();

            this.ProgressBarMax = 100;
            this.ProgressBarValue = 0;

            this.SetConfig();
            this.FillItemsSource();


            this.RunTextAnimationDuration = new TimeSpan(0, 0, 0, 15).ToString();

        }

        #region Propertys

        public static ObservableCollection<SoundItemViewModel> SoundItemsSource { set; get; }

        public ObservableCollection<SoundItemViewModel> SoundItemsSourceFilter { set; get; }

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

        public string SearchWord
        {
            get => base.GetProperty<string>(nameof(this.SearchWord));
            set
            {
                base.SetProperty(nameof(this.SearchWord), value);
                this.StartSearch(value);
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
                    Thread.Sleep(ShowUserMsgMs);
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
            JsonDirector.Instance.SaveConfigToJson(Config.Instance());

            var soundItemList = new List<SoundItem>();

            foreach (SoundItemViewModel soundItemVM in SoundItemsSource)
            {
                soundItemList.Add(soundItemVM.Model);
            }

            JsonDirector.Instance.SaveSongDataToJson(soundItemList);
        }

        private void StartSearch(string searchWord)
        {
            ObservableCollection<SoundItemViewModel> filteredSounds = new ObservableCollection<SoundItemViewModel>();

            if (string.IsNullOrEmpty(searchWord))
            {
                //zeige alle an 
                this.SoundItemsSourceFilter = new ObservableCollection<SoundItemViewModel>(SoundItemsSource);
            }
            else
            {
                foreach (SoundItemViewModel soundItem in SoundItemsSource)
                {
                    if (soundItem.NameToShow.ToLower().Contains(searchWord.ToLower()))
                    {
                        filteredSounds.Add(soundItem);
                    }
                }

                this.SoundItemsSourceFilter = new ObservableCollection<SoundItemViewModel>(filteredSounds);
            }

            this.SortItemsListBy(Sorting.Ascending, SortableListViewHeader.Favorite);
        }

        private void AddToObservableCollections(SoundItemViewModel soundItemViewModel)
        {
            SoundItemsSource.Add(soundItemViewModel);
            this.SoundItemsSourceFilter.Add(soundItemViewModel);
        }

        private void SetConfig()
        {
            var config = JsonDirector.Instance.LoadConfigFromJson();

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
        }

        private void FillItemsSource()
        {
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
                //TODO loggen
                System.Diagnostics.Debug.WriteLine(ex);

                if (SoundItemsSource.Count == 0 && SoundItemsSourceFilter.Count == 0)//wenn beim laden über json was falsch gelaufen ist mach mal normal
                {
                    this.LoadDatasByAlphaStart();
                }
            }



            base.OnPropertyChanged(nameof(this.SoundItemsSourceFilter));
        }

        /// <summary>
        /// Lädt erfolgreich geladene soundItems in die view und sortiert diese.
        /// </summary>
        private void LoadDatasBy(List<SoundItem> loadedDatas)
        {
            foreach (SoundItem item in loadedDatas)
            {
                var soundItemVM = new SoundItemViewModel(item);

                if (this.fileDirector.ExistFile(soundItemVM.Model.FilePath))
                    this.AddToObservableCollections(soundItemVM);
            }

            this.SortItemsListBy(Sorting.Ascending, SortableListViewHeader.Favorite);
        }

        /// <summary>
        /// Sucht aufm pc nach jeder mp3 Datei und fügt diese der View hinzu.(Dauert lange weil duration mit geupdatet werden muss)
        /// </summary>
        private void LoadDatasByAlphaStart()
        {
            this.AddSongsBy(fileDirector.GetAllFilesFromDevice(this.mediaExtensions), true);
            this.UpdateDurationOnViewTask();
        }

        private void ManageStartMusic(SoundItemViewModel soundSelectedItem)
        {
            this.CurrentTrackName = soundSelectedItem.NameToShow;
            this.SetMediaPlayerVolume(this.SoundVolume);
            this.mediaPlayer.MediaFailed += (o, e) =>
            {

                //TODO ERROR LOG
                System.Diagnostics.Debug.WriteLine("FEHLER PASSIERT : " + e.ErrorException);
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
                System.Diagnostics.Debug.WriteLine(ex);
            }
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
                Random random = new Random();
                bool whileActive = true;

                while (whileActive)
                {
                    var randomNumber = random.Next(0, SoundItemsSource.Count);
                    if (SoundItemsSource[randomNumber] != soundSelectedItem)
                    {
                        this.ManageStartMusic(SoundItemsSource[randomNumber]);
                        whileActive = false;
                        return true;
                    }
                }

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
                System.Diagnostics.Debug.WriteLine($"{ex}");
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

            if (!SoundItemsSource.Any(x => x.NameToShow == soundItemVM.NameToShow))
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

        private void SortItemsListBy(Sorting sorting, SortableListViewHeader header)
        {
            this.currentSort = sorting;

            Sorter sorter = new Sorter();

            SoundItemsSource = sorter.GetSortedListBy(SoundItemsSource, sorting, header);
            SoundItemsSourceFilter = sorter.GetSortedListBy(SoundItemsSourceFilter, sorting, header);

            base.OnPropertyChanged(nameof(SoundItemsSource));
            base.OnPropertyChanged(nameof(SoundItemsSourceFilter));
        }

        private void UpdateDurationOnViewTask()
        {
            Thread updateDurationThread = new Thread(this.UpdateDuration);
            updateDurationThread.Name = nameof(updateDurationThread);
            updateDurationThread.Start();
        }

        private void UpdateDuration()
        {
            ObservableCollection<SoundItemViewModel> toUpdate = new ObservableCollection<SoundItemViewModel>(SoundItemsSource);
            int countToUpdate = SoundItemsSource.Count;

            while (toUpdate.Count != 0)
            {
                Thread.Sleep(1);
                toUpdate.Clear();

                foreach (var item in SoundItemsSource)
                {
                    if (!item.UpdateDurationDone)
                    {
                        toUpdate.Add(item);
                    }
                }
            }

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
                System.Diagnostics.Debug.WriteLine($"{ex}");
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
                System.Diagnostics.Debug.WriteLine(ex);
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
                System.Diagnostics.Debug.WriteLine(ex);
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
                System.Diagnostics.Debug.WriteLine(ex);
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
                System.Diagnostics.Debug.WriteLine(ex);
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
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        #endregion
    }
}
