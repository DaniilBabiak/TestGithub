using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loger_HomeWork
{
    public class Result

    {
        public bool Status { get; init; }
        public string Message { get; init; } = string.Empty;
        public Result(bool status)
        {
            Status = status;
        }
        public Result(bool status, string message)
        {
            Message = message;
            Status = status;
        }
    }
}
