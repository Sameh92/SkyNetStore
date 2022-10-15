using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class SpecificationEvaluator<TEnity> where TEnity : BaseEntity
    {
        public static IQueryable<TEnity> GetQuery(IQueryable<TEnity> inputQuery, ISpecifications<TEnity> spec)
        {
            var query = inputQuery;
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }
            query = spec.OurIncludes.Aggregate(query, (currentEntity, includeParameter) => currentEntity.Include(includeParameter));
            return query;
        }
    }
}