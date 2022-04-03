using MusikPlayer.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikPlayer.Helper
{
    public static class TimeSpanConverter
    {
        public static TimeSpan? ConvertTimeSpam(string value)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                return null;

            if (TimeSpan.TryParse(value, out TimeSpan parsedValue))
            {
                return parsedValue;
            }
            else
            {
                return null;
            }
        }

        public static List<TimeSpan?> ConvertTimeSpam(List<string> values)
        {
            if (values == null || values.Count == 0)
                return null;
            List<TimeSpan?> list = new List<TimeSpan?>();

            foreach (var value in values)
            {
                list.Add(ConvertTimeSpam(value));
            }

            return list;
        }

        public static int GetSecondsFromTimeSpan(TimeSpan time)
        {
            try
            {
                return (int)time.TotalSeconds;//hier muss hart cast
            }
            catch (Exception ex)
            {
                Logger.Instance.ExceptionLogg(nameof(TimeSpanConverter), nameof(GetSecondsFromTimeSpan), ex);
            }
            return default;
        }
    }
}
