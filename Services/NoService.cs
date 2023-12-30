using GoldenvaleDAL.ClassObjects;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanProcessor.TitanCore;

namespace TitanProcessor.Services
{
    internal class NoService : ITitanService
    {
        public string ServiceName => "No Service";

        public void ProccessService(ILogger logger, TitanProcesserQueue item)
        {
            return;
        }

        ServiceResults ITitanService.ProccessService(ILogger logger, TitanProcesserQueue item)
        {
            throw new NotImplementedException();
        }
    }
}
