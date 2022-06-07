using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loger_HomeWork
{
     public class Starter
    {
        public void Run()
        {
            var logger = Logger.LoggerInstance;
            var actions = new Actions();
            for (int i = 0; i < 100; i++)
            {
                var random = new Random().Next(minValue: 0, maxValue: 3);
                if (random == 0)
                {
                    var result = actions.FirstMethod();
                    logger.LogInfo(result.Message);
                }
                else if (random == 1)
                {
                    var result = actions.SecondMethod();
                    logger.LogWarn(result.Message);
                }
                else { var result = actions.ThirdMethod();
                    logger.LogError(result.Message);                
                }
            }
           
        }
    }
}
