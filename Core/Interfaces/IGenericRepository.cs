using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Core.Entities;
using Core.Specifications;

namespace Core.Interfaces
{

    // Our repository in not responsible for saving changes to the db that is left to unit of work
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<T> GetEntityWithSpec(ISpecifications<T> spec);
        Task<IReadOnlyList<T>> ListAsyncWitSpec(ISpecifications<T> spec);

        Task<int> CountAsync(ISpecifications<T> spec);

        // all of those method is not async because we just need from those method 
        // to make ef core to start track this enity we are not going to save to database in those method 
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

    }
}