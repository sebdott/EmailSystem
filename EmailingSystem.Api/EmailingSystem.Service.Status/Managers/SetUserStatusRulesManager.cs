using EmailingSystem.Common.DataModels;
using EmailingSystem.Service.Rules.SetUserStatusRules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailingSystem.Service.Status.Managers
{
    public class SetUserStatusRulesManager : IStatusManager
    {
        List<ISetUserStatusRules> _rules = new List<ISetUserStatusRules>();

        public SetUserStatusRulesManager(User currentUser)
        {
            _rules.Add(new ActiveStatusRules(ref currentUser));
            _rules.Add(new InactiveStatusRules(ref currentUser));
            _rules.Add(new NotResponsiveStatusRules(ref currentUser));
        }

        public async Task<bool> Execute()
        {
            foreach (var rule in _rules)
            {
                if (await rule.SetStatus())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
