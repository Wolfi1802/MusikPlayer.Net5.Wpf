using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusikPlayer.Helper
{
    public static class BoolConverter
    {

        public static string ConvertBoolToString(bool value)
        {
            if (value)
                return "Aktiviert";
            else
                return "Deaktiviert";
        }
    }
}
