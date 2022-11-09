using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using PassingConsumer.Models;
using System.Text.Json;

namespace PassingConsumer.JobReceivedManager
{
    public class JobSuccessfullyManager
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        private readonly string tableName;


        public JobSuccessfullyManager(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration["AzureTableStorage:ConnectionString"];
            tableName = _configuration["AzureTableStorage:TableName"];

        }

        public async Task StorageSuccessfulJob(string jsonQueue)
        {
            try
            {
                string status = Constants.FAIL;
                Random random = new Random();
                int randomValue = random.Next(0, 15);

                var rnd = new Random();

                var queueMessage = JsonSerializer.Deserialize<QueueMessage>(jsonQueue);

                if(DateTime.Now.AddMinutes(randomValue) <= queueMessage.latestStart)
                {
                    status = Constants.PASS;
                }

                //columnName startedAt

                var jobResultMessage = new JobResultMessage(queueMessage.processName,
                                                            status,
                                                            DateTime.Now.AddMinutes(randomValue),
                                                            queueMessage.processNumber,
                                                            queueMessage.JobId);

                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var tableClient = storageAccount.CreateCloudTableClient();
                var table = tableClient.GetTableReference(tableName);

                await table.ExecuteAsync(TableOperation.Insert(jobResultMessage));
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
    }
}
