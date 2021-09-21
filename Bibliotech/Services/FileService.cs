using System.IO;

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
            catch
            {
                return false;
            }
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
