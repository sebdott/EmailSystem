using EmailingSystem.Common.Enums;
using EmailingSystem.Common.DataModels;
using System;
using System.Threading.Tasks;

namespace EmailingSystem.Logic.Rules.SendEmailRules
{
    public class InactiveEmailRules : ISendEmailRules
    {
        User _currentUser;
        public InactiveEmailRules(User currentUser)
        {
            _currentUser = currentUser;
        }

        public async Task<bool> Validate()
        {
            if (_currentUser.Status == Status.Inactive)
            {
                return false;
            }

            return false;

        }
    }
}
