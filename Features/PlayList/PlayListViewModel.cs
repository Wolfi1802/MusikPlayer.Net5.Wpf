using MusikPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikPlayer.Features.PlayList
{
    public class PlayListViewModel : ViewModelBase
    {
        public List<SoundItemViewModel> ListOfSounds { set; get; }

    }
}
