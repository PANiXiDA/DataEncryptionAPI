﻿using Common.SearchParams;
using Microsoft.EntityFrameworkCore;

namespace Dal.Interfaces
{
    public interface IBaseDal<TDbContext, TDbObject, TEntity, TObjectId, TSearchParams, TConvertParams>
        where TDbContext : DbContext, new()
        where TDbObject : class, new()
        where TEntity : class
        where TSearchParams : BaseSearchParams
        where TConvertParams : class
    {
        Task<TObjectId> AddOrUpdateAsync(TEntity entity);

        Task<bool> ExistsAsync(TObjectId id);

        Task<bool> ExistsAsync(TSearchParams searchParams);

        Task<TEntity> GetAsync(TObjectId id, TConvertParams? convertParams = null);

        Task<SearchResult<TEntity>> GetAsync(TSearchParams searchParams, TConvertParams? convertParams = null);

        Task<bool> DeleteAsync(TObjectId id);
    }
}
