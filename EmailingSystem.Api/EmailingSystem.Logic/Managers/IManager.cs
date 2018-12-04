using System.Threading.Tasks;

namespace EmailingSystem.Logic.Managers
{
    public interface IManager
    {
        Task<bool> Execute();
    }
}