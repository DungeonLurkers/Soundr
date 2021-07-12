using System;
using System.Linq.Expressions;

namespace Soundr.YouTube.Services
{
    public interface ICacheService
    {
        T? TryGetValue<T>(Expression<Func<T, bool>> predicate);
        T? TryGetValue<T>(string predicate);
        void Insert<T>(T entry);
        void Update<T>(T entry);
    }
}