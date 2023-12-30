using GoldenvaleDAL.ClassObjects;
using GoldenvaleDAL.DataLayerWorker;
using log4net.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanProcessor.Services;
using TitanProcessor.TitanCore;

namespace TitanProcessor
{
    public class TitanProcessorWorker : ITitanProcessorWorker
    {
        private IDataLayerWorker _dataworker;
        private ILogger _logger;
        private TitanUtility _util;
        private System.Timers.Timer _atimer = new System.Timers.Timer();

        public TitanProcessorWorker(IDataLayerWorker dataLayerWorker, ILogger logger, double interval = 5000)
        {
            _dataworker = dataLayerWorker;
            _logger = logger;
            TimerInit(interval);
            _util = new TitanUtility(_dataworker);
        }

        private void TimerInit(double interval)
        {
            _atimer.Elapsed +=  TimerTick;
            _atimer.Interval = interval;
            _atimer.Enabled = false;
        }

        public void StartWatcher()
        {
            ToggleTimer(true);
        }

        public void StopWatcher()
        {
            ToggleTimer(false);
        }

        public void TimerTick(object sender, EventArgs e) 
        {
            if (_dataworker == null)
            {
                Console.WriteLine("Timer tick while data worker is null");
                return;
            }

            var res = GetQueueItemsByStatusId(1); //TODO fix string. 

            if (res.Count > 0)
            {
                Console.WriteLine($"{res.Count} items found. Begining processing {DateTime.Now}");

                ToggleTimer(false);

                foreach (var item in res)
                {
                    ProcessService(_logger, item);
                }

                ToggleTimer(true);
            }
            else
            {
                Console.WriteLine($"{res.Count} items found. Checking again in 5 seconds {DateTime.Now}");
            }
        }

        private void ToggleTimer(bool start = true)
        {
            if (start)
            { 
                _atimer.Start();
                //TimerTick(null, null);
            }
            else
            { 
                _atimer.Stop();
            }

            Console.WriteLine($"Watcher running: {_atimer.Enabled} {DateTime.Now}");
        }

        public async Task<int> AddItemToQueueAsync(int serviceId, string serviceData)
        {

            var data = new TitanProcesserQueue()
            {
                ServiceId = serviceId,
                ServiceData = serviceData,
                ServiceStatusId = 1 //check ServiceStatus table to see values. TODO: change to ENUM
            };

            var results = await _dataworker.Create(data);

            return results.ServiceStatusId;
        }

        public async Task ProcessService(ILogger logger, TitanProcesserQueue item)
        {
            //get service to run 
            var serviceToRun = GetServiceToRun(item.ServiceId);


            //mark as processing 
            await _util.ModifyQueueItemForProcessing(item.TitanProcesserQueueID, 2);

            //run service
            var result = serviceToRun.ProccessService(logger, item);

            ProcessResults(result,item);
        }

        public void ProcessResults(ServiceResults results, TitanProcesserQueue item)
        {
            var serviceFlag = 3;
            //update rec in queue
            if(!results.Success)
            {
                serviceFlag = 4;
            }

            var modResults = _util.ModifyQueueItemForProcessing(item.TitanProcesserQueueID, serviceFlag);

            //log results 
            Console.WriteLine($"Service Item: {item.TitanProcesserQueueID} Success: {results.Success}");
        }

        public async Task ModifyQueueItemForProcessing(int queueId, int serviceStatusId)
        {
            await _util.ModifyQueueItemForProcessing(queueId, 2);
        }

        public List<TitanProcesserQueue> GetQueueItemsByStatusId(int statusId)
        {
            var parms = new Dictionary<string, object>()
            {
                { "ServiceStatusId", statusId }
            };

            var res = _dataworker.ExecuteSproc<TitanProcesserQueue>(TitanProcesserQueue.SprocSelectByStatusId(), parms);

            return res.Result;
        }

        private ITitanService GetServiceToRun(int serviceId)
        {
            switch (serviceId)
            {
                case 1:
                    {
                        return new TitanMonitorService();
                    }
                default:
                    return new NoService();
                    
            }
        }
    }
}
