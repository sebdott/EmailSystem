
using EmailingSystem.Common.DataModels;

namespace EmailingSystem.Service.Log.Interface
{
    public interface ILogServiceManager
    {
        bool PublishLog(LogItem logItem);
    }
}
