using MusikPlayer.Helper;
using MusikPlayer.Logs;
using MusikPlayer.Model;
using MusikPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace MusikPlayer.FileManager
{
    public class JsonDirector : FilePathBase
    {
        private static readonly Lazy<JsonDirector> lazy = new Lazy<JsonDirector>(() => new JsonDirector());

        public static JsonDirector Instance { get { return lazy.Value; } }

        private JsonDirector()
        {

        }

        //
        public void TrySaveConfigToJson(Config data)
        {
            try
            {
                this.SaveConfigToJson(data, CONFIG_FILE, base.ConfigFilePath);
            }
            catch (Exception ex)
            {
                Logger.Instance.ExceptionLogg(nameof(JsonDirector), nameof(TrySaveConfigToJson), ex);
            }
        }

        public void TrySaveSongDataToJson(List<SoundItem> data)
        {
            try
            {
                this.SaveSoundToJson(data, SONG_DATA_FILE, base.SongDataFilePath);
            }
            catch (Exception ex)
            {
                Logger.Instance.ExceptionLogg(nameof(JsonDirector), nameof(TrySaveSongDataToJson), ex);
            }
        }

        public Config TryLoadConfigFromJson()
        {
            try
            {
                if (base.ExistFile(base.ConfigFilePath + CONFIG_FILE))
                {
                    return this.LoadConfigFromJson(base.ConfigFilePath + CONFIG_FILE);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ExceptionLogg(nameof(JsonDirector), nameof(TryLoadConfigFromJson), ex);
            }

            return null;
        }

        public List<SoundItem> TryLoadSongDataFromJson()
        {
            try
            {
                if (base.ExistFile(base.SongDataFilePath + SONG_DATA_FILE))
                {
                    return this.LoadSoundsFromJson(base.SongDataFilePath + SONG_DATA_FILE);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ExceptionLogg(nameof(JsonDirector), nameof(TryLoadSongDataFromJson), ex);
            }

            return null;
        }
        //

        public void SaveConfigToJson(Config data)
        {
            this.SaveConfigToJson(data, CONFIG_FILE, base.ConfigFilePath);
        }

        public void SaveSongDataToJson(List<SoundItem> data)
        {
            this.SaveSoundToJson(data, SONG_DATA_FILE, base.SongDataFilePath);
        }

        public Config LoadConfigFromJson()
        {
            if (base.ExistFile(base.ConfigFilePath + CONFIG_FILE))
            {
                return this.LoadConfigFromJson(base.ConfigFilePath + CONFIG_FILE);
            }

            return null;
        }

        public List<SoundItem> LoadSongDataFromJson()
        {
            if (base.ExistFile(base.SongDataFilePath + SONG_DATA_FILE))
            {
                return this.LoadSoundsFromJson(base.SongDataFilePath + SONG_DATA_FILE);
            }
            return null;
        }

        private void PrepareSaving(string file, string folder)
        {
            string filePath = $"{folder}{file}";

            var folderExist = base.ExistFolder(folder) ? true : false;
            var fileExist = base.ExistFile(filePath) ? true : false;

            if (!folderExist)
                base.CreateFolder(folder);

            if (!fileExist)
                base.CreateFile(filePath);

        }

        private void SaveSoundToJson(List<SoundItem> data, string file, string folder)
        {
            string filePath = $"{folder}{file}";

            this.PrepareSaving(file, folder);

            var json = JsonSerializer.Serialize(data);
            base.SaveText(json, filePath);
        }

        private void SaveConfigToJson(Config data, string file, string folder)
        {
            string filePath = $"{folder}{file}";

            this.PrepareSaving(file, folder);

            var json = JsonSerializer.Serialize(data);
            base.SaveText(json, filePath);
        }

        private List<SoundItem> Convert(List<SoundItem> value)
        {
            if (value == null || value.Count == 0)
                return null;
            List<SoundItem> list = new List<SoundItem>();

            foreach (var item in value)
            {

                SoundItem soundItem = new SoundItem();
                soundItem = item;

                var convertedTimeSpanm = TimeSpanConverter.ConvertTimeSpam(item.DurationString);

                if (convertedTimeSpanm.HasValue)
                {
                    soundItem.Duration = convertedTimeSpanm.Value;
                }

                list.Add(soundItem);
            }

            return list;
        }

        private List<SoundItem> LoadSoundsFromJson(string filePath)//TODO [TS] timespan wird nicht richtig geladen
        {
            if (string.IsNullOrEmpty(filePath))
                Logger.Instance.ExceptionLogg(nameof(JsonDirector), nameof(LoadSoundsFromJson), new Exception($"{nameof(filePath)}ist leer"));

            string datas = base.LoadFile(filePath);

            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
            };

            var result = JsonSerializer.Deserialize<List<SoundItem>>(datas, options);

            return this.Convert(result);
        }

        private Config LoadConfigFromJson(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                Logger.Instance.ExceptionLogg(nameof(JsonDirector), nameof(LoadConfigFromJson), new Exception($"{nameof(filePath)}ist leer"));

            string datas = base.LoadFile(filePath);

            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
            };

            var result = JsonSerializer.Deserialize<Config>(datas, options);

            return result;
        }
    }
}
