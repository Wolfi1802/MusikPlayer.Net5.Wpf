using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MusikPlayer.Model
{
    public class Config
    {
        private static Config instance;
        public static Config Instance ()
        {
            if (instance == null)
                instance = new Config();

            return instance;
        }

        [JsonInclude]
        public int? SoundVolume { set; get; }//Volume des SoundPlayers (0%-100%)

        [JsonInclude]
        public int? ShowUserMsgMs { set; get; }//Anzeigedauer für Nachrichten 
    }
}
