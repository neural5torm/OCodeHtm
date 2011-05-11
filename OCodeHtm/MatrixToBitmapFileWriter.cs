using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CnrsUniProv.OCodeHtm
{
    public class MatrixToBitmapFileWriter : BitmapFileWriter<Matrix>
    {
        public MatrixToBitmapFileWriter(string folder)
            : base(folder)
        { }

        protected override Bitmap GetBitmapFrom(Matrix output)
        {
            var normalized = output.Normalize(byte.MaxValue);

            var bitmap = new Bitmap(normalized.ColumnCount, normalized.RowCount, PixelFormat.Format24bppRgb);
            foreach (var el in normalized.IndexedEnumerator())
            {
                var lum = (int)el.Item3;
                bitmap.SetPixel(el.Item2, el.Item1, Color.FromArgb(lum, lum, lum));
            }
            return bitmap;
        }
    }
}
