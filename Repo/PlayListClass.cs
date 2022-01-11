using MusikPlayer.Model;
using System;
using System.Collections.Generic;
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
    }
}
