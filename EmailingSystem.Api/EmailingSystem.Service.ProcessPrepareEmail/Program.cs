using System;
using Microsoft.Extensions.DependencyInjection;
using EmailingSystem.Common.Providers.Interface;
using Microsoft.Extensions.Configuration;
using EmailingSystem.Common.Common;
using EmailingSystem.Common;
using EmailingSystem.Common.Providers.Implementation;
using System.Threading.Tasks;
using Confluent.Kafka;
using EmailingSystem.Common.DataModels;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using EmailingSystem.Service.ProcessPrepareEmail.Managers;

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
          EmailingSystemTopic.ProcessPrepareEmail,
          _kafkaSettings.ConsumerGroupId,
          _kafkaSettings.Partition))
            {
                while (true)
                {
                    try
                    {
                        if (consumer.Consume(out Message<Null, string> msg, TimeSpan.FromMilliseconds(100)))
                        {
                            Console.WriteLine("EmailingSystem.Service.ProcessPrepareEmail " + msg.Value);
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
                    var initialSearchFilter2 = Builders<User>.Filter.Lt(x => x.SendEmailDatetime, DateTime.UtcNow.Date) &
                        Builders<User>.Filter.Ne(x => x.Status, Common.Enums.Status.Inactive) &
                    Builders<User>.Filter.Eq(x => x.ProcessingID, Guid.Empty.ToString());

                    var userResult2 = await _dataProvider.FindOne(initialSearchFilter2);


                    var initialSearchFilter = Builders<User>.Filter.And(Builders<User>.Filter.Lt(x => x.SendEmailDatetime, DateTime.UtcNow.Date),
                        Builders<User>.Filter.Ne(x => x.Status, Common.Enums.Status.Inactive),
                    Builders<User>.Filter.Eq(x => x.ProcessingID, Guid.Empty.ToString()),
                    Builders<User>.Filter.Eq<bool>(x => x.IsSentEmailValidation, false));
                        
                    
                    var userResult = await _dataProvider.FindOne(initialSearchFilter);
                    
                    if (userResult == null)
                    {
                        break;
                    }

                    var services = new ServiceCollection();
                    services.AddTransient<IProcessPrepareEmailManager>(s => new ProcessPrepareEmailManager(userResult));
                    var statusManager = services.BuildServiceProvider().GetService<IProcessPrepareEmailManager>();


                    FilterDefinition<User> searchDefinition = Builders<User>.Filter.Eq(x => x.UserId, userResult.UserId);
                    UpdateDefinition<User> updateDefinition;

                    if (await statusManager.Execute())
                    {
                        userResult.SendEmailDatetime = DateTime.UtcNow;
                        await PublishSendEmail(userResult);

                        updateDefinition = Builders<User>.Update.Set(x => x.SendEmailDatetime, userResult.SendEmailDatetime)
                            .Set(x => x.IsSentEmailValidation, true);
                    }
                    else
                    {
                        updateDefinition = Builders<User>.Update.Set(x => x.IsSentEmailValidation, true);
                    }

                    await _dataProvider.FindUpdate(searchDefinition, updateDefinition);

                    i++;
                }
            }
            catch (Exception ex)
            {
                await _logProvider.PublishError("EmailingSystem.Services.ProcessStatusTopic", "Error on Commit ", ex);
            }
        }

        private static async Task<bool> PublishSendEmail(User user) {

            using (var producer = _pubSubProvider.GetPublishProvider(_kafkaSettings.BrokerList,
                                  _kafkaSettings.ProducerGroupId))
            {
                var r = new Random();
                var deliveryReport = producer.ProduceAsync(EmailingSystemTopic.SendEmail, null, Serializer.Serialize(user).ToString(), r.Next(0, _kafkaSettings.Partition + 1));
                await deliveryReport.ContinueWith(task =>
                {
                    Console.WriteLine($"PublishSendEmail  - User: { user.Username }, User Status: { user.Status}, Partition: {task.Result.Partition}, Offset: {task.Result.Offset}");
                });

                producer.Flush(1000);
            }

            return true;
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
