using EmailingSystem.Common.DataModels;
using EmailingSystem.Service.ProcessPrepareEmail.Managers;
using EmailingSystem.Service.ProcessPrepareEmail.Rules.SendEmailRules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailingSystem.Service.ProcessPrepareEmail.Managers
{
    public class ProcessPrepareEmailManager : IProcessPrepareEmailManager
    {
        List<ISendEmailRules> _rules = new List<ISendEmailRules>();

        public ProcessPrepareEmailManager(User currentUser)
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
