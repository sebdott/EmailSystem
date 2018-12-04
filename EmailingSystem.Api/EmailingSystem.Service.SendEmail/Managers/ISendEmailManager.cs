using EmailingSystem.Common.DataModels;
using System.Threading.Tasks;

namespace EmailingSystem.Service.SendEmail.Managers
{
    public interface ISendEmailManager
    {
        Task<bool> SendEmail(User currentUser);
    }
}