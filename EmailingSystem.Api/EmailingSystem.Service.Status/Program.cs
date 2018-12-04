using System;
using Microsoft.Extensions.DependencyInjection;
using EmailingSystem.Common.Providers.Interface;
using Microsoft.Extensions.Configuration;
using EmailingSystem.Common.Common;
using EmailingSystem.Common.Providers.Implementation;
using System.Threading.Tasks;
using Confluent.Kafka;
using EmailingSystem.Common.DataModels;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using EmailingSystem.Service.Status.Managers;

namespace EmailingSystem.Service.Status
{
    class Program
    {
        private static IPubSubProvider _pubSubProvider;
        private static IConfigurationRoot _configuration;
        private static KafkaSettings _kafkaSettings;
        private static ILogProvider _logProvider;
        private static IDataProvider _dataProvider;

        static void Main(string[] args)
        {

            if (!StartUpService()) return;

            using (var consumer = _pubSubProvider
          .GetConsumerProvider(_kafkaSettings.BrokerList,
          EmailingSystemTopic.ProcessStatus,
          _kafkaSettings.ConsumerGroupId,
          _kafkaSettings.Partition))
            {
                while (true)
                {
                    try
                    {
                        if (consumer.Consume(out Message<Null, string> msg, TimeSpan.FromMilliseconds(100)))
                        {
                            Console.WriteLine("EmailingSystem.Services.ProcessStatusTopic " + msg.Value);
                            CallMain(args).Wait();

                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        private static async Task CallMain(string[] args)
        {
            try
            {
                var newID = Guid.NewGuid();
                int i = 0;

                while (true)
                {
                    var initialSearchFilter = Builders<User>.Filter.Lt(x => x.LastModifiedDate, DateTime.UtcNow.Date) &
                   Builders<User>.Filter.Eq(x => x.ProcessingID, Guid.Empty.ToString());

                    var userResult = await _dataProvider.FindUpdate(initialSearchFilter,
                      Builders<User>.Update.Set(x => x.ProcessingID, newID.ToString())
                      .Set(x => x.IsSentEmailValidation, false));


                    if (userResult == null)
                    {
                        break;
                    }

                    var services = new ServiceCollection();
                    services.AddTransient<IStatusManager>(s => new SetUserStatusRulesManager(userResult));
                    var statusManager = services.BuildServiceProvider().GetService<IStatusManager>();


                    FilterDefinition<User> endProcessFilter = Builders<User>.Filter.Eq(x => x.UserId, userResult.UserId);
                    UpdateDefinition<User> endProcessUpdate;

                    if (await statusManager.Execute())
                    {
                        endProcessUpdate = Builders<User>.Update.Set(x => x.ProcessingID, Guid.Empty.ToString())
                            .Set(x => x.LastModifiedDate, DateTime.UtcNow.Date)
                            .Set(x => x.Status, userResult.Status);
                    }
                    else
                    {
                        endProcessUpdate = Builders<User>.Update.Set(x => x.ProcessingID, Guid.Empty.ToString())
                            .Set(x => x.LastModifiedDate, DateTime.UtcNow.Date);
                    }


                    await _dataProvider.FindUpdate(endProcessFilter, endProcessUpdate);

                    i++;

                    Console.WriteLine(i);
                }
            }
            catch (Exception ex)
            {
                await _logProvider.PublishError("EmailingSystem.Services.ProcessStatusTopic", "Error on Commit ", ex);
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

                var mongoDbSettings = new MongoDbSettings();
                _configuration.GetSection("MongoDbSettings").Bind(mongoDbSettings);

                var serviceProvider = new ServiceCollection()
                .AddSingleton<IPubSubProvider, KafkaProvider>()
                .AddSingleton<ILogProvider, LogProvider>()
                .AddSingleton<IDataProvider, MongoDBProvider>(settings =>
                   new MongoDBProvider(mongoDbSettings.Host, mongoDbSettings.Database, mongoDbSettings.UserCollection))
                .Configure<KafkaSettings>(_configuration.GetSection("KafkaSettings"))
                .AddOptions()
                .BuildServiceProvider();

                _pubSubProvider = serviceProvider.GetService<IPubSubProvider>();
                _logProvider = serviceProvider.GetService<ILogProvider>();
                _dataProvider = serviceProvider.GetService<IDataProvider>();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Action<object> Convert<T>(Action<T> myActionT)
        {
            if (myActionT == null) return null;
            else return new Action<object>(o => myActionT((T)o));
        }
    }
}
