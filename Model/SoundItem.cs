using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusikPlayer.Model
{
    public class SoundItem
    {
        [JsonIgnore]
        public readonly string StringFormatDuration = @"hh\:mm\:ss";

        [JsonIgnore]
        public TimeSpan Duration
        {
            get { return _duration; }
            set { this._duration = value; this.DurationString = value.ToString(StringFormatDuration); }
        }

        [JsonInclude]
        public string DurationString { set; get; }

        [JsonInclude]
        public bool IsFavorite { set; get; }

        [JsonInclude]
        public string Name { set; get; }

        [JsonInclude]
        public string FilePath { set; get; }

        [JsonInclude]
        public Uri Uri { set; get; }


        [JsonIgnore]
        private TimeSpan _duration;
    }
}
