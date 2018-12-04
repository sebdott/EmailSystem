using EmailingSystem.Common.Enums;
using EmailingSystem.Common.DataModels;
using System;
using EmailingSystem.Common.Common;
using System.Threading.Tasks;

namespace EmailingSystem.Service.Rules.SetUserStatusRules
{
    public class NotResponsiveStatusRules : ISetUserStatusRules
    {
        User _currentUser;
        public NotResponsiveStatusRules(ref User currentUser) {
            _currentUser = currentUser;
        }
        
        public async Task<bool> SetStatus() {

            if (_currentUser.Status == Common.Enums.Status.Active) { 

                if (await Util.GetNoDaysFromNow(_currentUser.LastLoginDatetime, DateTime.UtcNow.Date) >= 4)
                {
                    _currentUser.Status = Common.Enums.Status.NotResponsive;
                    return true;
                }
            }

            return false;

        }
    }

}
