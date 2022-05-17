using MusikPlayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikPlayer.ViewModel
{
    public class PlayListItemViewModel: ViewModelBase
    {
        public PlayListItemViewModel(PlayListItemModel item)
        {
            this.Model = item;
            this.NameToShow = item.Name;
            this.DurationToShow = item.Duration.ToString(item.StringFormatDuration);
            this.IsSelected = item.IsSelected;
        }
        public PlayListItemViewModel()
        {
            this.Model = new PlayListItemModel();
        }

        public PlayListItemModel Model { set; get; }

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

        public bool IsSelected
        {
            get
            {
                return base.GetProperty<bool>(nameof(this.IsSelected));
            }
            set
            {
                base.SetProperty(nameof(this.IsSelected), value);
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

    }
}
