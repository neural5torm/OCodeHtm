using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CnrsUniProv.OCodeHtm.Interfaces;

namespace CnrsUniProv.OCodeHtm
{
    public class BitmapFileWriter : IOutputWriter<Bitmap>
    {
        public void OutputWriterHandler(object sender, OutputEventArgs<Bitmap> e)
        {
            //TODOnow! output writer
            throw new NotImplementedException();
        }
    }
}
