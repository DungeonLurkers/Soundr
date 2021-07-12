using System;
using System.Linq.Expressions;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Soundr.Commons.Attributes;
using Soundr.YouTube.Services;

namespace Soundr.PlayerApi.Services
{
    [Service(ServiceLifetime.Singleton)]
    public class DbCacheService : ICacheService, IDisposable
    {
        private readonly ILogger<DbCacheService> _logger;
        private Lazy<LiteDatabase> _db;

        public DbCacheService(ILogger<DbCacheService> logger, IHostEnvironment environment)
        {
            _logger = logger;
            var dbPath = environment.ContentRootFileProvider.GetFileInfo("cache.db").PhysicalPath;
            _db = new Lazy<LiteDatabase>(() =>
            {
                _logger.LogInformation($"Initalizing LiteDB at {dbPath}");
                return new LiteDatabase(dbPath);
            });
        }

        public T? TryGetValue<T>(Expression<Func<T, bool>> predicate)
        {
            _logger.LogTrace($"TryGetValue(Expression<Func<{typeof(T).Name}, bool>>)");
            var collection = _db.Value.GetCollection<T>();

            return collection.FindOne(predicate);
        }

        public T? TryGetValue<T>(string predicate)
        {
            _logger.LogTrace("TryGetValue(string)");
            var collection = _db.Value.GetCollection<T>();
            
            return collection.FindOne(predicate);
        }

        public void Insert<T>(T entry)
        {
            _logger.LogTrace($"Insert({typeof(T).Name})");
            var collection = _db.Value.GetCollection<T>();

            collection.Insert(entry);
        }
        
        public void Update<T>(T entry)
        {
            _logger.LogTrace($"Update({typeof(T).Name})");
            var collection = _db.Value.GetCollection<T>();

            collection.Update(entry);
        }

        public void Dispose()
        {
            _logger.LogInformation($"Disposing LiteDB");
            _db.Value.Dispose();
        }
    }
}