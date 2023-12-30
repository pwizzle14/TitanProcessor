using GoldenvaleDAL.ClassObjects;
using GoldenvaleDAL.DataLayerWorker;
using log4net.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TitanProcessor.ServiceData;
using TitanProcessor.TitanCore;

namespace TitanProcessor.Services
{
    public class TitanMonitorService : ServiceBase, ITitanService
    {
        public string ServiceName { get => "MonitorService"; }
        private ServiceResults _serviceResults = new ServiceResults();


        public ServiceResults ProccessService(ILogger logger, TitanProcesserQueue item)
        {
            try
            {
               
                //serialize to object
                var titanData = JsonConvert.DeserializeObject<TitanMonitorServiceData>(item.ServiceData);

                if(titanData == null)
                {
                    throw new Exception($"Error passing TitanMonitorServiceData {item.ServiceData}");
                }

                Ping ping = new Ping();

                var pingAmount = 100;

                var failedPings = 0;
                var latencySum = 0;

                for (int i = 1; i < pingAmount; i++)
                {
                    PingReply reply = ping.Send(titanData.URL);

                    if (reply != null)
                    {
                        if (reply.Status != IPStatus.Success)
                            failedPings += 1;
                        else
                            latencySum += (int)reply.RoundtripTime;
                    }
                }

                var averagePing = (latencySum / (pingAmount - failedPings));
                var packetLoss = Convert.ToInt32((Convert.ToDouble(failedPings) / Convert.ToDouble(pingAmount)) * 100);

                string logText = $"Address: {item.ServiceData} - Round Trip Average: {averagePing} ms - PackLoss {packetLoss}";

                //todo return as message.

                _serviceResults.Success = true;

                return _serviceResults;
            }

            catch (SystemException ex)
            {
                string logText = $"Warning: Address: {item.ServiceData} - offline";
                _serviceResults.Success = false;
                _serviceResults.ex = ex;
                _serviceResults.Message = logText;

                return _serviceResults;
            }
        }
    }
}
