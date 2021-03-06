﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm.Exceptions
{
    public class HtmRuleException : HtmException
    {
        public HtmRuleException(string message, object from)
            : this(message, from, null)
        { }

        public HtmRuleException(string message, object from, Exception innerException)
            : base(message, from, innerException)
        { }

        
    }
}
