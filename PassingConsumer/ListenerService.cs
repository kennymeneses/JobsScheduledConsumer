using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PassingConsumer.JobReceivedManager;
using System.Text;
using ExceptionReceivedEventArgs = Microsoft.Azure.ServiceBus.ExceptionReceivedEventArgs;
using QueueClient = Microsoft.Azure.ServiceBus.QueueClient;

namespace PassingConsumer
{
    public class ListenerService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private IQueueClient _QueueClient;
        JobSuccessfullyManager jobmanager;
        private int executionCount = 0;
        private Timer _timer;

        public ListenerService(IConfiguration configuration, JobSuccessfullyManager _manager)
        {
            _configuration = configuration;
            jobmanager = _manager;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("Listener Service Started");                
                await base.StartAsync(cancellationToken);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public virtual Task HandleFailureMessage(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            if (exceptionReceivedEventArgs == null)
                throw new ArgumentNullException(nameof(exceptionReceivedEventArgs));
            return Task.CompletedTask;
        }

        public async Task Handle(Message message, CancellationToken cancelToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var body = Encoding.UTF8.GetString(message.Body);
            
            Console.WriteLine($"Job Details are: {body}");
            await _QueueClient.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);

            await jobmanager.StorageSuccessfulJob(body);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string connectionString = _configuration["ServiceBus:ConnectionString"];
            string queueName = _configuration["ServiceBus:QueueName"];

            var messageHandlerOptions = new MessageHandlerOptions(HandleFailureMessage)
            {
                MaxConcurrentCalls = 5,
                AutoComplete = false,
                MaxAutoRenewDuration = TimeSpan.FromMinutes(10)
            };

            _QueueClient = new QueueClient(connectionString, queueName);
            _QueueClient.RegisterMessageHandler(Handle, messageHandlerOptions);

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            await _QueueClient.CloseAsync().ConfigureAwait(false);
        }
    }
}
