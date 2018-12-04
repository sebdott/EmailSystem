using Bogus;
using EmailingSystem.Common;
using CommonStatus = EmailingSystem.Common.Enums.Status;
using System;
using Xunit;
using EmailingSystem.Common.DataModels;
using System.Threading.Tasks;
using EmailingSystem.Service.ProcessPrepareEmail.Rules.SendEmailRules;

namespace EmailingSystem.UnitTest.Service.Email
{
    public class SendEmailRulesTest
    {
        [Fact]
        public async Task UserActive()
        {

            var user = await GetTestUserBasedOnDays(10, CommonStatus.Active);
            ISendEmailRules rules = new ActiveEmailRules(user);

            var isValid = await rules.Validate();

            Assert.True(isValid == (user.Status == CommonStatus.Active));
        }

        [Fact]
        public async Task UserNotResponsive()
        {

            var user = await GetTestUserBasedOnDays(10, CommonStatus.NotResponsive);
            ISendEmailRules rules = new NotResponsiveEmailRules(user);

            var isValid = await rules.Validate();
            
            Assert.True(isValid == (user.Status == CommonStatus.NotResponsive && user.LastLoginDatetime.AddDays(3).Date == DateTime.Now.Date));
            Assert.True(isValid == (user.Status == CommonStatus.NotResponsive && user.LastLoginDatetime.AddDays(6).Date == DateTime.Now.Date));
            Assert.True(isValid == (user.Status == CommonStatus.NotResponsive && user.LastLoginDatetime.AddDays(5).Date == DateTime.Now.Date));
            Assert.True(isValid == (user.Status == CommonStatus.NotResponsive && user.LastLoginDatetime.AddDays(2).Date == DateTime.Now.Date));
            Assert.False(user.Status == CommonStatus.NotResponsive && user.LastLoginDatetime.AddDays(2).Date == DateTime.Now.Date);
        }

        [Fact]
        public async Task UserInactive()
        {
            var user = await GetTestUserBasedOnDays(10, CommonStatus.Inactive);

            ISendEmailRules rules = new InactiveEmailRules(user);

            var isValid = await rules.Validate();

            Assert.False(isValid == (user.Status == CommonStatus.Inactive));
        }

        private async Task<User> GetTestUserBasedOnDays(int noDays, CommonStatus status)
        {
            return await Task.Run(() =>
            {
                var testUser = new Faker<User>()
             .StrictMode(true)
            .RuleFor(o => o.UserId, string.Empty)
            .RuleFor(o => o.Username, f => f.Internet.UserName())
            .RuleFor(o => o.Email, f => f.Internet.Email())
            .RuleFor(u => u.Status, status)
            .RuleFor(o => o.SendEmailDatetime, DateTime.Now)
            .RuleFor(o => o.ProcessingID, Guid.Empty.ToString())
            .RuleFor(o => o.LastModifiedDate, DateTime.Now)
            .RuleFor(o => o.LastLoginDatetime,
                          (f, o) => DateTime.Now.AddDays(noDays))
            .RuleFor(o => o.IsSentEmailValidation, false)
            .RuleFor(o => o.Email, f => f.Internet.Email());

                return testUser.Generate();

            });
        }
    }
}
