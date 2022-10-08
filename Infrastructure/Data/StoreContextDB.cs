using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class StoreContextDB:DbContext
    {
        public StoreContextDB(DbContextOptions<StoreContextDB> options):base(options)
        {
            
        }
        public DbSet<Product> Products {get;set;}
    }
}