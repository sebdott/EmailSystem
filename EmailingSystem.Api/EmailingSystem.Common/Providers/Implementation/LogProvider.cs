using EmailingSystem.Common.Common;
using EmailingSystem.Common.DataModels;
using EmailingSystem.Common.Providers.Interface;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace EmailingSystem.Common.Providers.Implementation
{
    public class LogProvider : ILogProvider
    {
        private IPubSubProvider _pubSubProvider;
        private IOptions<KafkaSettings> _kafkaSettings;

        public LogProvider(IPubSubProvider pubSubProvider, IOptions<KafkaSettings> kafkaSettings)
        {
            _pubSubProvider = pubSubProvider;
            _kafkaSettings = kafkaSettings;
        }

        public async Task PublishError(string identifier, string message, Exception exception)
        {
            string exceptionMessage = string.Empty;
            try {
                var stackTrace = JsonConvert.SerializeObject(exception.StackTrace);
                exceptionMessage = exception.Message + " " + stackTrace;
            }
            catch (Exception) {}

            var _logItem = new LogItem()
            {
                Identifier = identifier,
                Message = message,
                Exception = exceptionMessage,
                Type = LogType.Error
            };

            await PublishLog(_logItem);
        }

        public async Task PublishInfo(string identifier, string message)
        {
            var _logItem = new LogItem()
            {
                Identifier = identifier,
                Message = message,
                Type = LogType.Information
            };

            await PublishLog(_logItem);
        }

        private async Task PublishLog(LogItem logitem)
        {
            using (var producer = _pubSubProvider.GetPublishProvider(_kafkaSettings.Value.BrokerList,
                                     _kafkaSettings.Value.ProducerGroupId))
            {
                var deliveryReport = producer.ProduceAsync(EmailingSystemTopic.Logging, null, Serializer.Serialize(logitem).ToString(), _kafkaSettings.Value.Partition);
                await deliveryReport.ContinueWith(task =>
                {
                    Console.WriteLine($"Partition: {task.Result.Partition}, Offset: {task.Result.Offset}");
                });

                producer.Flush(1000);
            }
        }
    }
}
