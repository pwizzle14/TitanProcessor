using GoldenvaleDAL.ClassObjects;
using GoldenvaleDAL.DataLayerWorker;

namespace TitanProcessor
{
    public class TitanUtility
    {
        private IDataLayerWorker _dataworker;
        public TitanUtility(IDataLayerWorker datalayeworker) 
        {
             _dataworker = datalayeworker;
        }

        public async Task ModifyQueueItemForProcessing(int titanProcesserQueueId, int serviceStatusId)
        {
            var rec = _dataworker.SelectById(titanProcesserQueueId, new TitanProcesserQueue());

            if (rec.Result == null)
            {
                return;
            }

            var data = rec.Result;
            data.ServiceStatusId = serviceStatusId;
            data.ProcessingGUID = Guid.NewGuid().ToString();

            if (serviceStatusId == 2)
            {
                data.StartTimeUTC = DateTime.UtcNow;
                data.EndTimeUTC = null;
            }
            else
            {
                data.EndTimeUTC = DateTime.UtcNow;
            }

            var res = _dataworker.Update(data);

            if(res.IsFaulted) 
            {
                throw new Exception($"Error modifying Item for processing QueueId {titanProcesserQueueId}, serviceStatusId:{serviceStatusId}");
            }
        }
    }
}
