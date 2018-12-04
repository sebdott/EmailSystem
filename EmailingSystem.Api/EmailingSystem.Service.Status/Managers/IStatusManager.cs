using System.Threading.Tasks;

namespace EmailingSystem.Service.Status.Managers
{
    public interface IStatusManager
    {
        Task<bool> Execute();
    }
}