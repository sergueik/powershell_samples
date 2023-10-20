using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Core.Utils;

namespace Core.Repository.MongoDb {

    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private IMongoDatabase _database;
        private IMongoCollection<T> _collection;

        protected IMongoCollection<T> Collection
        {
            get { return _collection; }
        }

        static MongoRepository()
        {
            MongoClassMapHelper.RegisterConventionPacks();
            MongoClassMapHelper.SetupClassMap();
        }

        public MongoRepository(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentException("Missing MongoDB connection string");
            }

            var client = new MongoClient(connectionString);
            MongoUrl mongoUrl = MongoUrl.Create(connectionString);
            this._database = client.GetDatabase(mongoUrl.DatabaseName);
            this._collection = this.SetupCollection();
        }

        protected virtual IMongoCollection<T> SetupCollection()
        {
            try
            {
                var collectionName = this.BuildCollectionName();
                var collection = this._database.GetCollection<T>(collectionName);
                return collection;
            }
            catch (MongoException ex)
            {
                throw new RepositoryException(ex.Message);
            }
        }

        protected virtual string BuildCollectionName()
        {
            var pluralizedName = typeof(T).Name.EndsWith("s") ? typeof(T).Name : typeof(T).Name + "s";
            return pluralizedName;
        }


        public virtual async Task<T> Insert(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.ThrowIfNull(entity, "entity");

            if (entity.Id == null)
            {
                entity.Id = ObjectId.GenerateNewId().ToString();
            }

            try
            {
                entity.CreatedDate = DateTime.UtcNow;
                entity.UpdatedDate = DateTime.UtcNow;
                await this._collection.InsertOneAsync(entity, cancellationToken);
            }
            catch (MongoWriteException ex)
            {
                throw new EntityDuplicateException(entity, "Insert failed because the entity already exists!", ex);
            }

            return entity;
        }


        public virtual async Task<T> Update(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.ThrowIfNull(entity, "entity");

            var previousUpdateDate = entity.UpdatedDate;
            var previuosVersion = entity.Version;
            entity.UpdatedDate = DateTime.UtcNow;
            entity.Version++;

            ReplaceOneResult result;

            var idFilter = Builders<T>.Filter.Eq(e => e.Id, entity.Id); //Find entity with same Id

            //Consistency enforcement
            if (!this.IgnoreVersion())
            {
                var versionLowerThan = Builders<T>.Filter.Lt(e => e.Version, entity.Version);

                result = await this.Collection.ReplaceOneAsync(
                    // Consistency enforcement: Where current._id = entity.Id AND entity.Version > current.Version
                    Builders<T>.Filter.And(idFilter, versionLowerThan),
                    entity,
                    null,
                    cancellationToken);

                if (result != null && ((result.IsAcknowledged && result.MatchedCount == 0) || (result.IsModifiedCountAvailable && !(result.ModifiedCount > 0))))
                    throw new EntityConflictException(entity, "Update failed because entity versions conflict!");
            }
            else
            {
                result = await this.Collection.ReplaceOneAsync(idFilter, entity, null, cancellationToken);

                if (result != null && ((result.IsAcknowledged && result.MatchedCount == 0) || (result.IsModifiedCountAvailable && !(result.ModifiedCount > 0))))
                    throw new EntityException(entity, "Entity does not exist.");


            }

            return entity;
        }


        public virtual async Task<T> Get(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this._collection.Find(e => e.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<T> Delete(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this._collection.FindOneAndDeleteAsync(e => e.Id == id, null, cancellationToken);
        }


        public virtual async Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this._collection.Find(e => true).ToListAsync(cancellationToken);
        }


        public virtual async Task<IEnumerable<T>> Pagination(int top, int skip, Func<T, object> orderBy, bool ascending = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = this._collection.Find(e => true).Skip(skip).Limit(top);

            if (ascending)
                return await query.SortBy(e => e.Id).ToListAsync();
            else
                return await query.SortByDescending(e => e.Id).ToListAsync(cancellationToken);
        }

        public virtual bool IgnoreVersion()
        {
            return false;
        }
    }
}
