using EmailingSystem.Common.Enums;
using EmailingSystem.Common.DataModels;
using EmailingSystem.Common.Common;
using System;
using System.Threading.Tasks;

namespace EmailingSystem.Service.ProcessPrepareEmail.Rules.SendEmailRules
{
    public class NotResponsiveEmailRules : ISendEmailRules
    {
        User _currentUser;
        public NotResponsiveEmailRules(User currentUser)
        {
            _currentUser = currentUser;
        }

        public async Task<bool> Validate()
        {
            if (_currentUser.Status == Common.Enums.Status.NotResponsive)
            {
                if (await Util.GetNoDaysFromNow(_currentUser.LastLoginDatetime, DateTime.Now.Date) == 3 ||
                    await Util.GetNoDaysFromNow(_currentUser.LastLoginDatetime, DateTime.Now.Date) == 6){

                    return false;
                }

            }

            return false;

        }
    }
}
