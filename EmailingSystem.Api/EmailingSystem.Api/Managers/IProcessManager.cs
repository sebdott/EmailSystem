using EmailingSystem.Common.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailingSystem.Api.Managers
{
    public interface IProcessManager
    {
        Task<Boolean> InitiateSetStatusProcess();
        Task<Boolean> InitiateSendEmailProcess();
        Task<Boolean> ResetTestData(int records);
        Task<DisplayView<User>> GetUserList(int currentPage, int pageSize);
        Task<DisplayView<EmailLog>> GetEmailLog(int currentPage, int pageSize);
    }
}