using System.Threading.Tasks;

namespace EmailingSystem.Service.ProcessPrepareEmail.Managers
{
    public interface IProcessPrepareEmailManager
    {
        Task<bool> Execute();
    }
}