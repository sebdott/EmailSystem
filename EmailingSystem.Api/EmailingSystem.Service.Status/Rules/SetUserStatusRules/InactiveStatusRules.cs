using EmailingSystem.Common.Enums;
using EmailingSystem.Common.DataModels;
using EmailingSystem.Common.Common;
using System;
using System.Threading.Tasks;

namespace EmailingSystem.Service.Rules.SetUserStatusRules
{
    public class InactiveStatusRules : ISetUserStatusRules
    {
        User _currentUser;
        public InactiveStatusRules(ref User currentUser) {
            _currentUser = currentUser;
        }
        
        public async Task<bool> SetStatus() {

            if (_currentUser.Status == Common.Enums.Status.NotResponsive) { 

                if (await Util.GetNoDaysFromNow(_currentUser.LastLoginDatetime, DateTime.UtcNow.Date) >= 2)
                {
                    _currentUser.Status = Common.Enums.Status.Inactive;
                    return true;
                }
            }

            return false;

        }
    }

}
