using Bogus;
using CommonStatus = EmailingSystem.Common.Enums.Status;
using EmailingSystem.Common.DataModels;
using System;
using Xunit;
using System.Threading.Tasks;
using EmailingSystem.Service.Rules.SetUserStatusRules;

namespace EmailingSystem.UnitTest.Service.Status
{
    public class SetUserStatusRulesTest
    {
        [Theory]
        [InlineData(-1, CommonStatus.NotResponsive)]
        [InlineData(-2, CommonStatus.NotResponsive)]
        [InlineData(-3, CommonStatus.Active)]
        [InlineData(-5, CommonStatus.Inactive)]
        [InlineData(-20, CommonStatus.Inactive)]
        public async Task SetUserActiveTest(int noOfDays, CommonStatus status)
        {
            var user = await GetTestUserBasedOnDays(noOfDays, status);

            var userStatus = user.Status;

            ISetUserStatusRules rules = new ActiveStatusRules(ref user);

            var isSet = await rules.SetStatus();

            if (isSet)
            {
                if (userStatus == CommonStatus.NotResponsive && user.Status == CommonStatus.Active)
                {
                    Assert.True(true);
                }
                else
                {
                    Assert.True(false);
                }
            }
            else
            {
                if (userStatus == user.Status)
                {
                    Assert.True(true);
                }
                else
                {
                    Assert.True(false);
                }
            }
        }


        [Theory]
        [InlineData(-1, CommonStatus.NotResponsive)]
        [InlineData(-2, CommonStatus.NotResponsive)]
        [InlineData(-3, CommonStatus.Active)]
        [InlineData(-5, CommonStatus.Inactive)]
        [InlineData(-20, CommonStatus.Inactive)]
        public async Task SetUserNotResponsiveTest(int noOfDays, CommonStatus status)
        {
            var user = await GetTestUserBasedOnDays(noOfDays, status);

            var userStatus = user.Status;

            ISetUserStatusRules rules = new NotResponsiveStatusRules(ref user);

            var isSet = await rules.SetStatus();

            if (isSet)
            {
                if (userStatus == CommonStatus.Active && user.Status == CommonStatus.NotResponsive)
                {
                    Assert.True(true);
                }
                else
                {
                    Assert.True(false);
                }
            }
            else
            {
                if (userStatus == user.Status)
                {
                    Assert.True(true);
                }
                else
                {
                    Assert.True(false);
                }
            }
        }

        [Theory]
        [InlineData(-1, CommonStatus.NotResponsive)]
        [InlineData(-2, CommonStatus.NotResponsive)]
        [InlineData(-3, CommonStatus.Active)]
        [InlineData(-5, CommonStatus.Inactive)]
        [InlineData(-20, CommonStatus.Inactive)]
        public async Task SetUserInactiveTest(int noOfDays, CommonStatus status)
        {
            var user = await GetTestUserBasedOnDays(noOfDays, status);

            var userStatus = user.Status;

            ISetUserStatusRules rules = new InactiveStatusRules(ref user);

            var isSet = await rules.SetStatus();

            if (isSet)
            {
                if (userStatus == CommonStatus.NotResponsive && user.Status == CommonStatus.Inactive)
                {
                    Assert.True(true);
                }
                else
                {
                    Assert.True(false);
                }
            }
            else
            {
                if (userStatus == user.Status)
                {
                    Assert.True(true);
                }
                else
                {
                    Assert.True(false);
                }
            }
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
            .RuleFor(u => u.Status, f => f.PickRandom<CommonStatus>())
            .RuleFor(o => o.SendEmailDatetime, DateTime.Now)
            .RuleFor(o => o.ProcessingID, Guid.Empty.ToString())
            .RuleFor(o => o.LastModifiedDate, DateTime.Now)
            .RuleFor(o => o.IsSentEmailValidation, false)
            .RuleFor(o => o.LastLoginDatetime,
                          (f, o) => DateTime.Now.AddDays(noDays))
            .RuleFor(o => o.Email, f => f.Internet.Email());




                return testUser.Generate();

            });
        }

    }
}
