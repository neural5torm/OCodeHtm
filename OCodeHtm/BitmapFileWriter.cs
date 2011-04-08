using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CnrsUniProv.OCodeHtm.Interfaces;
using System.IO;

namespace CnrsUniProv.OCodeHtm
{
    public class BitmapFileWriter : IOutputWriter<Bitmap>
    {
        public int SubFolderCounter { get; private set; }
        public int FileCounter { get; private set; }

        public DirectoryInfo OutputFolder { get; private set; }


        public BitmapFileWriter(string folder)
        {
            FileCounter = 1;

            SetFolder(folder);
        }



        public void SetFolder(string folder)
        {
            var dir = Directory.Exists(folder) ? new DirectoryInfo(folder) : Directory.CreateDirectory(folder);

            int maxSubFolderNumber = 1;
            int readNumber;

            foreach (var subdir in dir.EnumerateDirectories())
            { 
                if (int.TryParse(subdir.Name, out readNumber))
                    maxSubFolderNumber = Math.Max(maxSubFolderNumber, readNumber);
            }

            SubFolderCounter = Math.Abs(maxSubFolderNumber);

            OutputFolder = dir.CreateSubdirectory(SubFolderCounter.ToString("D4")); 
        }

        public void OutputWriterHandler(object sender, OutputEventArgs<Bitmap> e)
        {
            //TODO async (see http://stackoverflow.com/questions/803242/understanding-events-and-event-handlers-in-c/803274#803274)
            e.Output.Save(Path.Combine(OutputFolder.FullName, string.Format("{0}{1:D4}_{2}.bmp", 
                sender.GetType().Name, FileCounter++, e.Category)));
        }
    }
}
