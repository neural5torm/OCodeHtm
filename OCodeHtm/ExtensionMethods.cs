using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm
{
    public static class ExtensionMethods
    {
        public static bool NextBool(this Random random)
        {
            return random.Next(2) > 0 ? true : false;
        }

        public static int NextSign(this Random random)
        {
            return random.NextBool() ? -1 : 1;
        }
    }
}