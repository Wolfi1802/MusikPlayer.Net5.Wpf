using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MusikPlayer.ViewModel
{
    public class PlayListListItem : ViewModelBase
    {
        [JsonIgnore]
        public readonly string StringFormatDuration = @"hh\:mm\:ss";

        public List<PlayListItemViewModel> ListOfSounds { set; get; }

        public string Name
        {
            get => base.GetProperty<string>(nameof(this.Name));
            set => base.SetProperty(nameof(this.Name), value);

        }

        [JsonIgnore]
        public TimeSpan DurationOfPlayList
        {
            get => base.GetProperty<TimeSpan>(nameof(this.DurationOfPlayList));
            set
            {
                base.SetProperty(nameof(this.DurationOfPlayList), value);
                this.DurationOfPlayListString = value.ToString(this.StringFormatDuration);
            }
        } 

        public string DurationOfPlayListString
        {
            get => base.GetProperty<string>(nameof(this.DurationOfPlayListString));
            set
            {
                base.SetProperty(nameof(this.DurationOfPlayListString), value);
            }
        }

        public int CountOfSongs
        {
            get => base.GetProperty<int>(nameof(this.CountOfSongs));
            set
            {
                base.SetProperty(nameof(this.CountOfSongs), value);
            }
        }


    }
}
