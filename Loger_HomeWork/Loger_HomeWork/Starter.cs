using Loger_HomeWork.Exceptions;
using Loger_HomeWork.FileService;
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
            IFileService fileService = new FileService.FileService();
            var logger = new Logger(fileService); 
            var actions = new Actions();
            for (int i = 0; i < 100000; i++)
            {
                try
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
                    else
                    {
                        var result = actions.ThirdMethod();
                        logger.LogError(result.Message);
                    }
                }
                catch (BusinessException ex)
                {
                    logger.LogWarn(ex.Message);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }
           
        }
    }
}
