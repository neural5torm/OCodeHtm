﻿using System.Drawing;
using CnrsUniProv.OCodeHtm.Interfaces;

namespace CnrsUniProv.OCodeHtm
{
    public class BitmapFileWriter : BitmapFileWriter<Bitmap>
    {
        public BitmapFileWriter(string folder)
            : base(folder)
        { }

        protected override Bitmap GetBitmapFrom(Bitmap output)
        {
            return output;
        }
    }

}
