using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loger_HomeWork
{
    public class Logger
    {
        private static Logger _instance = null;
        public static Logger LoggerInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
                return _instance;
            }
        }
        public List <string> Logs { get; private set; }
        public void LogInfo(string message)
        {
            var dt = DateTime.Now;
            var type = "[INFO]";
            var log = dt.ToString() + type + message;
            Console.WriteLine(log);
            Logs.Add(log);  
        }
        public void LogWarn(string message)
        {
            var dt = DateTime.Now;
            var type = "[WARN]";
            var log = dt.ToString() + type + message;
            Console.WriteLine(log);
            Logs.Add(log);
        }
        public void LogError(string message)
        {
            var dt = DateTime.Now;
            var type = "[ERROR]";
            var log = dt.ToString() + type + message;
            Console.WriteLine(log);
            Logs.Add(log);
        }
        private Logger() { Logs = new List<string>(); }
           
    }
}
