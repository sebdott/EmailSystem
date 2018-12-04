using EmailingSystem.Common.Enums;
using EmailingSystem.Common.DataModels;
using EmailingSystem.Common.Common;
using EmailingSystem.Common;
using System;
using System.Threading.Tasks;

namespace EmailingSystem.Service.Rules.SetUserStatusRules
{
    public class ActiveStatusRules : ISetUserStatusRules
    {
        User _currentUser;
        public ActiveStatusRules(ref User currentUser) {
            _currentUser = currentUser;
        }

        public async Task<bool> SetStatus()
        {

            if (_currentUser.Status == Common.Enums.Status.NotResponsive) { 

                if (await Util.GetNoDaysFromNow(_currentUser.LastLoginDatetime, DateTime.UtcNow.Date) >= 2)
                {
                    _currentUser.Status = Common.Enums.Status.Active;
                    return true;
                }
            }

            return false;

        }
    }

}
