using EmailingSystem.Common.DataModels;
using EmailingSystem.Common.Providers.Interface;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailingSystem.Common.Providers.Implementation
{
    public class MongoDBProvider : IDataProvider
    {
        private MongoClient _client;
        private IMongoDatabase _db;
        private string _mongoCollection;

        public MongoDBProvider(string hostUri, string dbName, string collection)
        {
            _client = new MongoClient(hostUri);
            _db = _client.GetDatabase(dbName);
            _mongoCollection = collection;
        }

        public async Task<bool> Save(User user)
        {
            try
            {
                var _collection = _db.GetCollection<User>(_mongoCollection);
                await _collection.Indexes.CreateOneAsync(Builders<User>.IndexKeys.Ascending(_ => _.UserId));
                await _collection.InsertOneAsync(user);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<User> Get(string OrderId)
        {
            try
            {
                var _collection = _db.GetCollection<User>(_mongoCollection);

                var filter = Builders<User>.Filter.Eq<string>(s => s.UserId, OrderId);
                //var filter = Builders<Order>.Filter.Gte("OrderId", OrderId);
                var returnValue = await _collection.FindAsync<User>(filter);

                return await returnValue.FirstAsync<User>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<User> Update(User user)
        {
            try
            {
                var collection = _db.GetCollection<User>(_mongoCollection);

                var replaceOneResult = await collection.FindOneAndUpdateAsync(
                      Builders<User>.Filter.Eq("_id", user.UserId),
                      Builders<User>.Update.Set("Username", user.Username)
                      );

                return replaceOneResult;
            }
            catch (Exception)
            {
                return default(User);
            }
        }

        public async Task<User> FindUpdate(FilterDefinition<User> filterDefinition, UpdateDefinition<User> updateDefinition)
        {
            try
            {
                var collection = _db.GetCollection<User>(_mongoCollection);
                var userResult = await collection.FindOneAndUpdateAsync<User>(
                       filterDefinition,
                       updateDefinition
                       );

                return userResult;
            }
            catch (Exception)
            {
                return default(User);
            }
        }

        public async Task<List<T>> GetAll<T>(int currentPage, int pageSize, string collectionName)
        {
            var collection = _db.GetCollection<T>(collectionName);
            double totalDocuments = await collection.CountAsync(FilterDefinition<T>.Empty);
            var totalPages = Math.Ceiling(totalDocuments / pageSize);

            return await collection.Find(FilterDefinition<T>.Empty)
                                  .Skip((currentPage - 1) * pageSize)
                                  .Limit(pageSize).ToListAsync<T>();
        }

        public async Task<User> FindOne(FilterDefinition<User> filter)
        {
            var _collection = _db.GetCollection<User>(_mongoCollection);
            var returnValue = await _collection.FindAsync<User>(filter);

            try
            {
                return await returnValue.FirstAsync<User>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DropCollection(string collectionName)
        {
            await _db.DropCollectionAsync(collectionName);
            return true;
        }

        public async Task<long> Count<T>(string collectionName)
        {

            var _collection = _db.GetCollection<T>(collectionName);
            return await _collection.CountAsync(new BsonDocument());
        }

        public async Task<IMongoCollection<T>> GetCollection<T>(string collectionName)
        {
            return await Task.Run(() => _db.GetCollection<T>(collectionName));
        }

    }
}
