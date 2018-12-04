using Bogus;
using EmailingSystem.Common.Enums;
using CommonStatus = EmailingSystem.Common.Enums.Status;
using EmailingSystem.Common.DataModels;
using System;
using Xunit;
using System.Threading.Tasks;
using EmailingSystem.Service.ProcessPrepareEmail.Rules.SendEmailRules;
using EmailingSystem.Service.ProcessPrepareEmail.Managers;

namespace EmailingSystem.UnitTest.Service.Email
{
    public class SendEmailRulesManagerTest
    {
        [Theory]
        [InlineData(1, CommonStatus.NotResponsive)]
        [InlineData(2, CommonStatus.NotResponsive)]
        [InlineData(3, CommonStatus.NotResponsive)]
        [InlineData(4, CommonStatus.NotResponsive)]
        [InlineData(5, CommonStatus.NotResponsive)]
        [InlineData(6, CommonStatus.NotResponsive)]
        [InlineData(7, CommonStatus.NotResponsive)]
        [InlineData(3, CommonStatus.Active)]
        [InlineData(4, CommonStatus.Active)]
        [InlineData(5, CommonStatus.Active)]
        [InlineData(6, CommonStatus.Active)]
        [InlineData(1, CommonStatus.Inactive)]
        [InlineData(2, CommonStatus.Inactive)]
        [InlineData(5, CommonStatus.Inactive)]
        [InlineData(20, CommonStatus.Inactive)]
        public async Task ManagerTest(int noOfDays, CommonStatus status)
        {

            var user = await GetTestUserBasedOnDays(noOfDays, status);

            IProcessPrepareEmailManager manager = new ProcessPrepareEmailManager(user);

            var ableToSentEmail = await manager.Execute();

            var isValid = await ValidateSendRules(ableToSentEmail, user.Status, noOfDays);

            Assert.True(isValid);
        }

        private async Task<User> GetTestUserBasedOnDays(int noDays, CommonStatus status)
        {
            return await Task.Run(() =>
            {
                noDays = noDays * -1;

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

        private async Task<bool> ValidateSendRules(bool ableToSentEmail, CommonStatus status, int noOfDays) {

            return await Task.Run(() =>
            {
                if (ableToSentEmail)
                {
                    if (status == CommonStatus.Active)
                    {
                        return true;
                    }
                    else if (status == CommonStatus.Inactive)
                    {
                        return false;
                    }
                    else if (status == CommonStatus.NotResponsive && (noOfDays == 3 || noOfDays == 6))
                    {
                        return false;
                    }
                }
                else
                {
                    if (status == CommonStatus.Active)
                    {
                        return false;
                    }
                    else if (status == CommonStatus.Inactive)
                    {
                        return true;
                    }
                    else if (status == CommonStatus.NotResponsive && (noOfDays != 3 || noOfDays != 6))
                    {
                        return true;
                    }
                }

                return false;

            });
        }
    }
}
