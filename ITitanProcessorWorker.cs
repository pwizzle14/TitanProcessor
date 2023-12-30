using GoldenvaleDAL.ClassObjects;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanProcessor.TitanCore;

namespace TitanProcessor
{
    internal interface ITitanProcessorWorker
    {
        Task<int> AddItemToQueueAsync(int serviceId, string serviceData);
        Task ProcessService(ILogger logger, TitanProcesserQueue item);
        Task ModifyQueueItemForProcessing(int titanProcesserQueueId, int serviceStatusId);
        List<TitanProcesserQueue> GetQueueItemsByStatusId(int statusId);


    }
}
