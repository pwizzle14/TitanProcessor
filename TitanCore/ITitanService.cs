using GoldenvaleDAL.ClassObjects;
using GoldenvaleDAL.DataLayerWorker;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanProcessor.TitanCore
{
    public interface ITitanService
    {
        string ServiceName { get; }
        ServiceResults ProccessService(ILogger logger, TitanProcesserQueue item);
    }
}
