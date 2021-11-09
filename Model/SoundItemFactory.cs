using MusikPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikPlayer.Model
{
    public class SoundItemFactory
    {
        private const string FILE_DATA_NAME = ".mp3";

        public SoundItem CreateSoundItem(string filePath)
        {
            var fileppathSplited = filePath.Split('\\');
            var fileName = fileppathSplited[fileppathSplited.Length - 1];
            var nameToShow = fileName.Replace(FILE_DATA_NAME, string.Empty);

            SoundItem soundItem = new SoundItem();
            soundItem.FilePath = filePath;
            soundItem.Name = nameToShow;
            soundItem.Uri = new Uri(filePath);
            soundItem.IsFavorite = false;

            return soundItem;
        }

        public SoundItemViewModel CreateListItem(string filePath)
        {
            return new SoundItemViewModel(this.CreateSoundItem(filePath));
        }
    }
}
