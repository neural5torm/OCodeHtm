using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm.Interfaces
{
    public interface IClearable
    {
        string CurrentInputName { get; }
        string CurrentCategoryName { get; }

        void ClearCurrentMemory(string newInputName = Default.NoName, string newCategoryName = Default.NoName);
    }
}
