using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CnrsUniProv.OCodeHtm.Interfaces;

namespace CnrsUniProv.OCodeHtm
{
    public class HtmNetwork<TInput, TOutput> : IClearable
    {
        public string CurrentInputName { get; private set; }
        public string CurrentCategoryName { get; private set; }


        public HtmNetwork()
        {
            CurrentInputName = Default.NoName;
            CurrentCategoryName = Default.NoName;
        }

        public List<HtmLayer<TInput, TOutput>> Layers { get; private set; }

        

        public void ClearCurrentMemory(string newInputName = Default.NoName, string newCategoryName = Default.NoName)
        {
            CurrentInputName = newInputName;
            CurrentCategoryName = newCategoryName;

            //TODO ClearCurrentMemory
            throw new NotImplementedException();
        }
    }
}
