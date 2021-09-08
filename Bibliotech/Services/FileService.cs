using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibliotech.Services
{
    public class FileService
    {
        public bool IsFileOpen(string filePath)
        {
            bool fileOpened = false;
            try
            {
                System.IO.FileStream fs = System.IO.File.OpenWrite(filePath);
                fs.Close();
            }
            catch (System.IO.IOException ex)
            {
                fileOpened = true;
            }

            return fileOpened;
        }
    }
}
