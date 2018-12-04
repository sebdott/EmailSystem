using EmailingSystem.Common.DataModels;
using System;
using System.Threading.Tasks;

namespace EmailingSystem.Service.Rules.SetUserStatusRules
{
    public interface ISetUserStatusRules
    {
        Task<bool> SetStatus();
    }
}
