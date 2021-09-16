﻿using System.IO;

namespace Bibliotech.Services
{
    public class FileService
    {
        public bool IsFileOpen(string filePath)
        {
            try
            {
                File.OpenWrite(filePath).Close();

                return false;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}
