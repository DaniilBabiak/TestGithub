using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loger_HomeWork.FileService
{
    public class FileService : IFileService
    {
        private string _fileName = string.Empty;
        private string _folderName = string.Empty;

      
        public FileService() 
        {
            var dt = DateTime.Now;
            _fileName = @"..\..\..\..\Logs\" + dt.ToString() + ".txt";
            _fileName = _fileName.Replace(" ", ".");
            _fileName = _fileName.Replace(":", ".");
            _folderName = @"..\..\..\..\Logs\";
            CheckFiles();
        }
       
        public void WriteToFile(string message)
        {
            using (StreamWriter writer = new StreamWriter(_fileName, true))
            {
                writer.WriteLine(message);
            }
        }
      private void CheckFiles()
        {
            int filesAmount = FilesAmount();
            while (filesAmount > 2)
            {
                DeleteLatestFile();
                filesAmount--;
            }
        }
        private int FilesAmount()
        {
            var files = Directory.GetFiles(_folderName);
            return files.Count();
        }
        private void DeleteLatestFile()
        {
            var directory = new DirectoryInfo(_folderName);
            var files = directory.GetFiles();
            var oldestFile = files[0];
            foreach (var file in files) 
            {
               if (oldestFile.CreationTime > file.CreationTime)
                {
                    oldestFile = file;
                }
                
            }
            oldestFile.Delete();
            
        }
    }
}
