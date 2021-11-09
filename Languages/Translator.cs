using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikPlayer.Languages
{
    public class Translator
    {
        private static readonly Lazy<Translator>lazy = new Lazy<Translator>(() => new Translator());

        public static Translator Instance { get { return lazy.Value; } }

        private Translator()
        {
        }

        public const string OPEN_FILE_DIALOG_EXPORT_TITLE = "Bitte wählen Sie den entsprechenden Pfad aus.";
    }
}
