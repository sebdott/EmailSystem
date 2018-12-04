using EmailingSystem.Common.DataModels;
using EmailingSystem.Logic.Rules.SendEmailRules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailingSystem.Logic.Managers
{
    public class SendEmailRulesManager : IManager
    {
        List<ISendEmailRules> _rules = new List<ISendEmailRules>();

        public SendEmailRulesManager(User currentUser)
        {
            _rules.Add(new ActiveEmailRules(currentUser));
            _rules.Add(new InactiveEmailRules(currentUser));
            _rules.Add(new NotResponsiveEmailRules(currentUser));
        }

        public async Task<bool> Execute()
        {
            foreach (var rule in _rules)
            {
                if (await rule.Validate())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
