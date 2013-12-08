using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Project.Client.Web.Entities;

namespace Project.Client.Web.Repositories
{
    internal class DataRepository<T> where T : BaseData
    {
        private readonly MongoCollection<T> _collection;

        public DataRepository()
        {
            var mongoCnnStr = ConfigurationManager.AppSettings["MongoCnnStr"] ?? "mongodb://localhost";
            var dbName = ConfigurationManager.AppSettings["MongoDBName"] ?? "GraphChallenge";
            var concern = new WriteConcern { Journal = true, W = 1 };

            var mongoDatabase = new MongoClient(mongoCnnStr).GetServer().GetDatabase(dbName);

            _collection = mongoDatabase.GetCollection<T>(typeof(T).Name, concern);
        }

        public IQueryable<T> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public IQueryable<T> AsOrderedQueryable()
        {
            return AsQueryable().OrderByDescending(x => x.Id);
        }

        public WriteConcernResult Add(T entity)
        {
            return _collection.Insert(entity);
        }

        public void AddBulk(IEnumerable<T> entities)
        {
            _collection.InsertBatch(entities);
        }

        public WriteConcernResult DeleteAll()
        {
            return _collection.RemoveAll();
        }
    }
}