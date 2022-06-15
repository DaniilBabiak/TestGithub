using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loger_HomeWork.FileService
{
    public interface IFileService
    {
        public void WriteToFile(string message);
    }
}
