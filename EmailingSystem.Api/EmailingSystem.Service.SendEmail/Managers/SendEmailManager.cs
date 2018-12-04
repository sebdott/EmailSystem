using EmailingSystem.Common.DataModels;
using EmailingSystem.Common.Providers.Interface;
using EmailingSystem.Service.SendEmail.Managers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailingSystem.Service.ProcessPrepareEmail.Managers
{
    public class SendEmailManager : ISendEmailManager
    {
        IDataProvider _dataProvider;

        public SendEmailManager(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<bool> SendEmail(User currentUser)
        {
           var sendEmailCollection = await _dataProvider.GetCollection<EmailLog>(Common.Common.DBCollection.SendEmailCollection);

            var emaillog = new EmailLog()
            {
                UserId = currentUser.UserId,
                Username= currentUser.Username,
                Email = currentUser.Email,
                SendEmailDatetime = currentUser.SendEmailDatetime,
            };

            await sendEmailCollection.Indexes.CreateOneAsync(Builders<EmailLog>.IndexKeys.Ascending(_ => _.UserId));
            await sendEmailCollection.InsertOneAsync(emaillog);

            return true;
        }
    }
}
