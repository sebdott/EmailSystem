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
using EmailingSystem.Service.SendEmail.Managers;
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
        private static ISendEmailManager _sendEmailManager;

        static void Main(string[] args)
        {

            if (!StartUpService()) return;

            using (var consumer = _pubSubProvider
          .GetConsumerProvider(_kafkaSettings.BrokerList,
          EmailingSystemTopic.SendEmail,
          _kafkaSettings.ConsumerGroupId,
          _kafkaSettings.Partition))
            {
                while (true)
                {
                    var userValue = new Common.DataModels.User();

                    try
                    {
                        if (consumer.Consume(out Message<Null, string> msg, TimeSpan.FromMilliseconds(100)))
                        {
                            userValue = Serializer.Deserialize<Common.DataModels.User>(msg.Value);
                            CallMain(args, userValue).Wait();
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error : EmailingSystem.Service.SendEmail ");
                    }
                }
            }
        }


        private static async Task CallMain(string[] args, User user)
        {
            try
            {
                var newID = Guid.NewGuid();

                if (user != null)
                {
                    try
                    {
                        await _sendEmailManager.SendEmail(user);

                        Console.WriteLine($"Username:{user.Username} Email:{user.Email} send email success");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Username:{user.Username} Email:{user.Email} send email fail");
                    }
                }
            }
            catch (Exception ex)
            {
                await _logProvider.PublishError("EmailingSystem.Service.SendEmail", "Error on Commit ", ex);
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
                .AddTransient<ISendEmailManager, SendEmailManager>()
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
                _sendEmailManager = serviceProvider.GetService<ISendEmailManager>();
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
