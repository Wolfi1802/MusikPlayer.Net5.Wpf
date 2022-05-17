using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikPlayer.ViewModel
{
    /// <summary>
    /// Grundlegend wird die Klasse nur für Json als wrapper verwendet da json sonst net klar kommt.
    /// </summary>
    public class PlayListListItemCollection
    {
        public PlayListListItemCollection()
        {
            this.PlayList = new List<PlayListListItem>();
        }

        public List<PlayListListItem> PlayList { set; get; }
    }
}
