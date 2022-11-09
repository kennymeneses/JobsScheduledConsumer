using Microsoft.WindowsAzure.Storage.Table;

namespace PassingConsumer.Models
{
    public class JobResultMessage : TableEntity
    {
        public string processName { get; set; }
        public string result { get; set; }
        public DateTime startedAt { get; set; }
        public int processNumber { get; set; }
        public string JobId { get; set; }

        public JobResultMessage(string _processName, string _result, DateTime _startedAt, int _processNumber, string _jobId)
        {
            PartitionKey = _processNumber.ToString();
            RowKey = _jobId.ToString();
            processName = _processName;
            result = _result;
            startedAt = _startedAt;
            processNumber = _processNumber;
            JobId = _jobId;
        }
    }
}
