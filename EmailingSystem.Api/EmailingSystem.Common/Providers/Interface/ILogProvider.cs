using System.Threading.Tasks;

namespace EmailingSystem.Common.Providers.Interface
{
    public interface ILogProvider
    {
        Task PublishError(string eventID, string message, System.Exception exception);
        Task PublishInfo(string eventID, string message);

    }
}
