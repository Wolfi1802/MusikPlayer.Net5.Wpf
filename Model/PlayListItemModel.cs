using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MusikPlayer.Model
{
    public class PlayListItemModel
    {
        [JsonIgnore]
        public readonly string StringFormatDuration = @"hh\:mm\:ss";

        [JsonInclude]
        public TimeSpan Duration
        {
            get { return _duration; }
            set { this._duration = value; this.DurationString = value.ToString(StringFormatDuration); }
        }

        [JsonInclude]
        public string DurationString { set; get; }

        [JsonInclude]
        public bool IsSelected { set; get; }

        [JsonInclude]
        public string Name { set; get; }

        [JsonIgnore]
        private TimeSpan _duration;
    }
}
