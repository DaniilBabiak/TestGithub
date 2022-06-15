using Loger_HomeWork.FileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loger_HomeWork
{
    public class Logger
    {
        
        private readonly IFileService _fileService;
       public Logger(IFileService fileService)
        {
            _fileService = fileService;
            Logs = new List<string>();
           
        }

        public List <string> Logs { get; private set; }
        public void LogInfo(string message)
        {
            var dt = DateTime.Now;
            var type = "[INFO]";
            var log = dt.ToString() + type + message;
            Console.WriteLine(log);
            Logs.Add(log);
            _fileService.WriteToFile(log);
        }
        public void LogWarn(string message)
        {
            var dt = DateTime.Now;
            var type = "[WARN]";
            var log = dt.ToString() + type + message;
            Console.WriteLine(log);
            Logs.Add(log);
            _fileService.WriteToFile(log);
        }
        public void LogError(string message)
        {
            var dt = DateTime.Now;
            var type = "[ERROR]";
            var log = dt.ToString() + type + message;
            Console.WriteLine(log);
            Logs.Add(log);
            _fileService.WriteToFile(log);
        }
       
           
    }
}
