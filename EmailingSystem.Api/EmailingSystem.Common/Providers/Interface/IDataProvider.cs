using EmailingSystem.Common.DataModels;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailingSystem.Common.Providers.Interface
{
    public interface IDataProvider
    {
        Task<bool> Save(User user);
        Task<User> Get(string ID);
        Task<User> Update(User user);
        Task<User> FindUpdate(FilterDefinition<User> filterDefinition, UpdateDefinition<User> updateDefinition);
        Task<List<T>> GetAll<T>(int currentPage, int pageSize, string collectionName);
        Task<User> FindOne(FilterDefinition<User> filterDefinition);
        Task<bool> DropCollection(string collectionName);
        Task<long> Count<T>(string collectionName);
        Task<IMongoCollection<T>> GetCollection<T>(string collectionName);
    }
}
