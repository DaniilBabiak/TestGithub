using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loger_HomeWork
{
    public class Actions
    {
        public Result FirstMethod()
        {
            var result = new Result(true, "Start Method: FirstMethod");
            return result;

        }
        public Result SecondMethod()
        {
            var result = new Result(true, "Skipped logic in method: SecondMethod");
            return result;

        }
        public Result ThirdMethod()
        {
            var result = new Result(false, "I broke a logic: ThirdMethod");
            return result;
        }
    }
}
