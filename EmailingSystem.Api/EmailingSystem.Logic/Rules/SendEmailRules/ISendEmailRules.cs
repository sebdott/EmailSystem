using System;
using System.Threading.Tasks;

namespace EmailingSystem.Logic.Rules.SendEmailRules
{
    public interface ISendEmailRules
    {
        Task<bool> Validate();
    }
}
