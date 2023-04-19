using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    //IDisposable when we have finished our transaction is going to dispose of our context  
    public interface IUnitOfWork : IDisposable 
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

        // the idea behand this : it is going to return the number of changes to our database
        Task<int> Complete();
    }
}
