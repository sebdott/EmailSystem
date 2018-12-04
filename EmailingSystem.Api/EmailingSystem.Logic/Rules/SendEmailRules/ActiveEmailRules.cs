using EmailingSystem.Common.Enums;
using EmailingSystem.Common.DataModels;
using System;
using System.Threading.Tasks;

namespace EmailingSystem.Logic.Rules.SendEmailRules
{
    public class ActiveEmailRules : ISendEmailRules
    {
        User _currentUser;
        public ActiveEmailRules(User currentUser)
        {
            _currentUser = currentUser;
        }

        public async Task<bool> Validate()
        {
            return await Task.Run(() =>
            {
                if (_currentUser.Status == Status.Active)
                {
                    return true;
                }

                return false;
            });
        }
    }
}
