using MusikPlayer.FileManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikPlayer.Logs
{
    public class Logger : FilePathBase
    {
        private static readonly Lazy<Logger> lazy = new Lazy<Logger>(() => new Logger());

        public static Logger Instance { get { return lazy.Value; } }
        //TODO [TS] Logger hinzufügen
        private Logger()
        {

        }

        public void Logg(string className, string methodName, string yourMessage)
        {
            this.WriteLog();
        }

        public void RunLogg(string className, string methodName, string action)
        {
            this.WriteLog();
        }

        public void ExceptionLogg(string className, string methodName, string exception, string yourMessage = "NULL")
        {
            this.WriteLog();
        }

        public void TestLogg(string className, string methodName, string result, string exceptedResult)
        {
            this.WriteLog();
        }

        private void WriteLog()
        {
            //TODO[TS]
            //-exist file/direction?
            //-create if someone not exist
            //-write  + try catch
            

        }
    }
}
