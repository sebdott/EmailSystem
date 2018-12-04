using Bogus;
using EmailingSystem.Common.Common;
using EmailingSystem.Common.DataModels;
using EmailingSystem.Common.Enums;
using EmailingSystem.Common.Providers.Interface;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailingSystem.Api.Managers
{
    public class ProcessManager : IProcessManager
    {
        private IDataProvider _dataProvider;
        private IPubSubProvider _pubSubProvider;
        private IOptions<KafkaSettings> _kafkaSettings;

        public ProcessManager(IDataProvider dataProvider,
            IPubSubProvider pubSubProvider,
            IOptions<KafkaSettings> kafkaSettings)
        {
            _pubSubProvider = pubSubProvider;
            _dataProvider = dataProvider;
            _kafkaSettings = kafkaSettings;
        }

        public async Task<Boolean> InitiateSetStatusProcess()
        {
            using (var producer = _pubSubProvider.GetPublishProvider(_kafkaSettings.Value.BrokerList,
                                      _kafkaSettings.Value.ProducerGroupId))
            {
                var r = new Random();
                var deliveryReport = producer.ProduceAsync(EmailingSystemTopic.ProcessStatus, null, DateTime.Now.ToString(), r.Next(0, _kafkaSettings.Value.Partition + 1));
                await deliveryReport.ContinueWith(task =>
                 {
                     Console.WriteLine($"Partition: {task.Result.Partition}, Offset: {task.Result.Offset}");
                 });

                producer.Flush(1000);
            }

            return true;
        }
        public async Task<Boolean> InitiateSendEmailProcess()
        {

            using (var producer = _pubSubProvider.GetPublishProvider(_kafkaSettings.Value.BrokerList,
                                      _kafkaSettings.Value.ProducerGroupId))
            {
                var r = new Random();
                var deliveryReport = producer.ProduceAsync(EmailingSystemTopic.ProcessPrepareEmail, null, DateTime.Now.ToString(), r.Next(0, _kafkaSettings.Value.Partition + 1));
                await deliveryReport.ContinueWith(task =>
                {
                    Console.WriteLine($"Partition: {task.Result.Partition}, Offset: {task.Result.Offset}");
                });

                producer.Flush(1000);
            }

            return true;
        }

        public async Task<DisplayView<User>> GetUserList(int currentPage, int pageSize) {

            var listofUsers = await _dataProvider.GetAll<User>(currentPage, pageSize, Common.Common.DBCollection.UserCollection);
            var recordTotalCount = await _dataProvider.Count<User>(DBCollection.UserCollection);

            return new DisplayView<User>()
            {
                ListofDisplay = listofUsers,
                RecordTotalCount = recordTotalCount
            };
        }

        public async Task<DisplayView<EmailLog>> GetEmailLog(int currentPage, int pageSize)
        {

            var listOfEmailLog = await _dataProvider.GetAll<EmailLog>(currentPage, pageSize, Common.Common.DBCollection.SendEmailCollection);
            var recordTotalCount = await _dataProvider.Count<EmailLog>(DBCollection.SendEmailCollection);

            return new DisplayView<EmailLog>()
            {
                ListofDisplay = listOfEmailLog,
                RecordTotalCount = recordTotalCount
            };
        }

        public async Task<Boolean> ResetTestData(int records)
        {
            //.RuleFor(u => u.Status, f => f.PickRandom<Status>())
            var _testUser = new Faker<User>()
            .StrictMode(true)
            .RuleFor(o => o.UserId, string.Empty)
            .RuleFor(o => o.Username, f => f.Internet.UserName())
            .RuleFor(o => o.Email, f => f.Internet.Email())
            .RuleFor(u => u.Status, Status.Active)
            .RuleFor(o => o.SendEmailDatetime, DateTime.UtcNow.AddDays(-1).Date)
            .RuleFor(o => o.ProcessingID, Guid.Empty.ToString())
            .RuleFor(o => o.LastLoginDatetime,
            (f, o) => f.Date.Between(DateTime.UtcNow,
                        DateTime.UtcNow.AddDays(-10)))
            .RuleFor(o => o.LastModifiedDate, DateTime.UtcNow.AddDays(-1).Date)
            .RuleFor(o => o.Email, f => f.Internet.Email())
            .RuleFor(o => o.IsSentEmailValidation , false);

            await _dataProvider.DropCollection(Common.Common.DBCollection.UserCollection);
            await _dataProvider.DropCollection(Common.Common.DBCollection.SendEmailCollection);

            for (int i = 0; i < records; i++)
            {
                var user = _testUser.Generate();
                await _dataProvider.Save(user);

                Console.WriteLine(i);
            }

            return true;
        }
    }
}
