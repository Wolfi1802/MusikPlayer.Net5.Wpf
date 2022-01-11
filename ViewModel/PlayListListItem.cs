using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikPlayer.ViewModel
{
    public class PlayListListItem : ViewModelBase
    {
        public List<PlayListItemViewModel> ListOfSounds { set; get; }

        public string Name
        {
            get => base.GetProperty<string>(nameof(this.Name));
            set => base.SetProperty(nameof(this.Name), value);

        }

        public TimeSpan DurationOfPlayList
        {
            get => base.GetProperty<TimeSpan>(nameof(this.DurationOfPlayList));
            set
            {
                base.SetProperty(nameof(this.DurationOfPlayList), value);
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
