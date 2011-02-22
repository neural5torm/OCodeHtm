using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm.Exceptions
{
    public class HtmRuleException : Exception
    {
        public HtmRuleException(string message, object from)
            : this(message, from, null)
        { }

        public HtmRuleException(string message, object from, Exception innerException)
            : base(from.GetType().Name + ": " + message.TrimEnd('.', ' ') + ".", innerException)
        { }

        
    }
}
