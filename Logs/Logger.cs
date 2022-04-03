using MusikPlayer.Enum;
using MusikPlayer.FileManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikPlayer.Logs
{
    /// <summary>
    /// Loggt in AppData unter den ProjektName
    /// </summary>
    public class Logger : FilePathBase
    {
        private static readonly Lazy<Logger> lazy = new Lazy<Logger>(() => new Logger());

        public static Logger Instance { get { return lazy.Value; } }
        private Logger()
        {

        }

        /// <summary>
        /// Für Random Stuff den man schnell mal in einer Datei haben will.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="yourMessage"></param>
        public void Logg(string className, string methodName, string yourMessage)
        {
            this.WriteLog($"\nC: {className}\nM: {methodName}\nT:{DateTime.Now}\nMsg: {yourMessage}", LogType.Normal);
        }

        /// <summary>
        /// Jegliche Interaktion vom User wird hier festgehalten
        /// </summary>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="action"></param>
        public void RunLogg(string className, string methodName, string action)
        {
            this.WriteLog($"\nC: {className}\nM: {methodName}\nT:{DateTime.Now}\nMsg: {action}", LogType.Run);
        }

        /// <summary>
        /// Tritt eine unerwartete Exception auf wird sie hiermit geloggt
        /// </summary>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="exception"></param>
        /// <param name="yourMessage"></param>
        public void ExceptionLogg(string className, string methodName, Exception exception, string yourMessage = "NULL")
        {
            this.WriteLog($"\nC: {className}\nM: {methodName}\nT:{DateTime.Now}\nMsg: {yourMessage}\nEx: {exception}", LogType.Exception);
        }

        /// <summary>
        /// UnitTest Logger
        /// </summary>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="result"></param>
        /// <param name="exceptedResult"></param>
        public void TestLogg(string className, string methodName, string result, string exceptedResult)
        {
            this.WriteLog($"\nC: {className}\nM: {methodName}\nT:{DateTime.Now}\nMsg: {result}\nEx: {exceptedResult}", LogType.UnitTest);
        }

        /// <summary>
        /// Schreibt den Inhalt in die Datei
        /// </summary>
        private void WriteLog(string messageToLog, LogType logType)
        {
            try
            {
                string path = base.LogsFilePath;

                switch (logType)
                {
                    case LogType.Normal: base.SaveText(messageToLog, $"{path}\\{nameof(LogType.Normal)}.txt"); break;
                    case LogType.Run: base.SaveText(messageToLog, $"{path}\\{nameof(LogType.Run)}.txt"); break;
                    case LogType.Exception: base.SaveText(messageToLog, $"{path}\\{nameof(LogType.Exception)}.txt"); break;
                    case LogType.UnitTest: base.SaveText(messageToLog, $"{path}\\{nameof(LogType.UnitTest)}.txt"); break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fehler beim Loggen\nMsg:{messageToLog} \nException: {ex}");
            }
        }
    }
}
