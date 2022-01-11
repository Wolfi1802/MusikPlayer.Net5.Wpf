using MusikPlayer.Model;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusikPlayer.ViewModel
{
    public class SoundItemViewModel : ViewModelBase
    {
        private Mp3FileReader _reader;

        public Action<SoundItemViewModel> OnFavStateChanged;

        public SoundItemViewModel(SoundItem item)
        {
            this.Model = item;
            this.NameToShow = item.Name;
            this.DurationToShow = item.Duration.ToString(item.StringFormatDuration);
            this.IsFavorite = item.IsFavorite;

            Task updateTask = new Task(() =>
            {
                this.UpdateView(item.FilePath);
            });
            updateTask.Start();

        }

        public SoundItem Model { private set; get; }

        public string NameToShow
        {
            get
            {
                return base.GetProperty<string>(nameof(this.NameToShow));
            }
            set
            {
                base.SetProperty(nameof(this.NameToShow), value);
                this.Model.Name = value;
            }
        }

        public bool IsFavorite
        {
            get
            {
                return base.GetProperty<bool>(nameof(this.IsFavorite));
            }
            set
            {
                base.SetProperty(nameof(this.IsFavorite), value);
                this.Model.IsFavorite = value;
                this.OnFavStateChanged?.Invoke(this);
            }
        }

        public string DurationToShow
        {
            get
            {
                return base.GetProperty<string>(nameof(this.DurationToShow));
            }
            set
            {
                base.SetProperty(nameof(this.DurationToShow), value);
                this.Model.Duration = TimeSpan.Parse(value);
            }
        }

        public bool IsBreak
        {
            get => base.GetProperty<bool>(nameof(this.IsBreak));
            set => base.SetProperty(nameof(this.IsBreak), value);
        }

        public bool IsAbleToBreak
        {
            get => base.GetProperty<bool>(nameof(this.IsAbleToBreak));
            set => base.SetProperty(nameof(this.IsAbleToBreak), value);
        }

        public bool UpdateDurationDone
        {
            get => base.GetProperty<bool>(nameof(this.UpdateDurationDone));
            set => base.SetProperty(nameof(this.UpdateDurationDone), value);
        }

        private void UpdateView(string soundPath)
        {
            if (!string.IsNullOrEmpty(soundPath) && this._reader == null)
            {
                this._reader = new Mp3FileReader(soundPath);
                this.DurationToShow = this._reader.TotalTime.ToString(this.Model.StringFormatDuration);
                this.UpdateDurationDone = true;
            }
            else
            {
                //TODO[TS] LOGGER
            }
        }

        private void TryUpdateView(string soundPath)
        {
            if (!string.IsNullOrEmpty(soundPath))
            {
                try
                {
                    this.UpdateView(soundPath);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    Logs.Logger.Instance.ExceptionLogg($"{nameof(SoundItemViewModel)}", $"{nameof(this.TryUpdateView)}", ex.Message, $"hier passiert abgefahrener scheiß");
                }
            }
        }
    }
}
