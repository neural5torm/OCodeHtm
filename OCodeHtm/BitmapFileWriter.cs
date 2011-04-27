using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CnrsUniProv.OCodeHtm.Interfaces;
using System.IO;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Drawing.Imaging;

namespace CnrsUniProv.OCodeHtm
{
    public abstract class BitmapFileWriter<TOutput> : IOutputWriter<TOutput>
    {
        private int SubFolderCounter { get; set; }
        private int FileCounter { get; set; }

        public DirectoryInfo OutputFolder { get; private set; }


        public BitmapFileWriter(string folder)
        {
            FileCounter = 1;

            SetFolder(folder);
        }



        public void SetFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder))
                folder = Directory.GetCurrentDirectory();
            var dir = Directory.Exists(folder) ? new DirectoryInfo(folder) : Directory.CreateDirectory(folder);

            int maxSubFolderNumber = 1;
            int readNumber;

            foreach (var subdir in dir.EnumerateDirectories())
            { 
                if (int.TryParse(subdir.Name, out readNumber))
                    maxSubFolderNumber = Math.Max(maxSubFolderNumber, readNumber + 1);
            }

            SubFolderCounter = Math.Abs(maxSubFolderNumber);

            OutputFolder = dir.CreateSubdirectory(SubFolderCounter.ToString("D4")); 
        }



        public void OutputWriterHandler(object sender, OutputEventArgs<TOutput> e)
        {
            //TODO async (see http://stackoverflow.com/questions/803242/understanding-events-and-event-handlers-in-c/803274#803274)
            GetBitmapFrom(e.Output).Save(
                Path.Combine(OutputFolder.FullName, string.Format("{0}{1:D7}_{2}_{3}.bmp", sender.GetType().Name, FileCounter++, e.Category, this.GetType().Name)), 
                ImageFormat.Bmp);
        }


        protected abstract Bitmap GetBitmapFrom(TOutput output);
    }


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


    public class MatrixToBitmapFileWriter : BitmapFileWriter<Matrix>
    {
       public MatrixToBitmapFileWriter(string folder)
            : base(folder)
        { }

        protected override Bitmap GetBitmapFrom(Matrix output)
        {
            var normalized = output.Normalize(byte.MaxValue, 0, 0.5);

            int width = normalized.ColumnCount;
            int height = normalized.RowCount;
            byte[] pixelValues = new byte[width * height * 3]; // 24bpp

            foreach (var item in normalized.IndexedEnumerator())
            {
                var color = Convert.ToByte(item.Item3);
                pixelValues[(item.Item1 + item.Item2 * height) * 3/*24bpp*/] = color;
                pixelValues[(item.Item1 + item.Item2 * height) * 3/*24bpp*/ + 1] = color;
                pixelValues[(item.Item1 + item.Item2 * height) * 3/*24bpp*/ + 2] = color;
            }

            var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            //Get a reference to the images pixel data
            Rectangle dimension = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData data = bitmap.LockBits(dimension, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr pixelStartAddress = data.Scan0;

            //Copy the pixel data into the bitmap structure
            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, pixelStartAddress, pixelValues.Length);

            bitmap.UnlockBits(data);

            return bitmap;
        }
    }
}
