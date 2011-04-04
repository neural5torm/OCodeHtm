using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm.Exceptions
{
    public class HtmException : Exception
    {
        public HtmException(string message, object from)
            : this(message, from, null)
        { }

        public HtmException(string message, object from, Exception innerException)
            : base(from.GetType().Name + ": " + message.TrimEnd('.', ' ') + "." + (innerException != null ? " " + innerException.Message : string.Empty), 
            innerException)
        { }

        
    }   
}
