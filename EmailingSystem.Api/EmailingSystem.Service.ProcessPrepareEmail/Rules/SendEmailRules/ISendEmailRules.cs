using System;
using System.Threading.Tasks;

namespace EmailingSystem.Service.ProcessPrepareEmail.Rules.SendEmailRules
{
    public interface ISendEmailRules
    {
        Task<bool> Validate();
    }
}
