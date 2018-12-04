using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Confluent.Kafka;
using Newtonsoft.Json;
using EmailingSystem.Common.Providers.Interface;
using EmailingSystem.Common.DataModels;
using EmailingSystem.Service.Log.Interface;
using EmailingSystem.Common.Common;
using EmailingSystem.Common.Providers.Implementation;
using EmailingSystem.Service.Log.Managers;

namespace EmailingSystem.Service.Log
{
    class Program
    {
        private static IPubSubProvider _pubSubProvider;
        private static IConfigurationRoot _configuration;
        private static KafkaSettings _kafkaSettings;
        private static ILogProvider _logProvider;
        private static ILogServiceManager _logServiceManager;

        static void Main(string[] args)
        {

            if (!StartUpService()) return;

            using (var consumer = _pubSubProvider
                .GetConsumerProvider(_kafkaSettings.BrokerList,
                EmailingSystemTopic.Logging,
                _kafkaSettings.ConsumerGroupId,
                _kafkaSettings.Partition))
            {

                while (true)
                {
                    try
                    {
                        if (consumer.Consume(out Message<Null, string> msg, TimeSpan.FromMilliseconds(100)))
                        {
                            var logValue = Serializer.Deserialize<Common.DataModels.LogItem>(msg.Value);

                            _logServiceManager.PublishLog(logValue);

                            var committedOffsets = consumer.CommitAsync(msg).Result;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }


        }

        private static bool StartUpService()
        {
            try
            {
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                var builder = new ConfigurationBuilder()
                     .AddJsonFile(Constant.AppSettingsFile, optional: true, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                     .AddEnvironmentVariables();

                _configuration = builder.Build();

                _kafkaSettings = new KafkaSettings();
                _configuration.GetSection("KafkaSettings").Bind(_kafkaSettings);

                var graylogSettings = new GraylogSettings();
                _configuration.GetSection("GraylogSettings").Bind(graylogSettings);

                var serviceProvider = new ServiceCollection()
                .AddSingleton<IPubSubProvider, KafkaProvider>()
                .AddSingleton<ILogProvider, LogProvider>()
                .AddSingleton<ILogServiceManager, LogServiceManager>()
                .Configure<KafkaSettings>(_configuration.GetSection("KafkaSettings"))
                .Configure<GraylogSettings>(_configuration.GetSection("GraylogSettings"))
                .AddOptions()
                .BuildServiceProvider();

                _pubSubProvider = serviceProvider.GetService<IPubSubProvider>();
                _logProvider = serviceProvider.GetService<ILogProvider>();
                _logServiceManager = serviceProvider.GetService<ILogServiceManager>();

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
