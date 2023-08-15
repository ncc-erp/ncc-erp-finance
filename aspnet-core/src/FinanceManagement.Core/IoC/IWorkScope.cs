using Abp.Application.Services.Dto;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.IoC
{
    public interface IWorkScope : ITransientDependency
    {
        IRepository<TEntity, TPrimaryKey> GetRepo<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>;
        IRepository<TEntity, long> GetRepo<TEntity>() where TEntity : class, IEntity<long>;
        IRepository<TEntity, long> Repository<TEntity>() where TEntity : class, IEntity<long>;
        IQueryable<TEntity> GetAll<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>;
        IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, IEntity<long>;
        IQueryable<TEntity> All<TEntity>() where TEntity : class, IEntity<long>;

        TEntity Clone<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        long CloneAndGetId<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;

        IEnumerable<TEntity> InsertRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity<long>;
        Task<IEnumerable<TEntity>> InsertRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity<long>;
        TEntity Insert<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        Task<TEntity> InsertAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        long InsertAndGetId<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        Task<long> InsertAndGetIdAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        TEntity Update<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        long InsertOrUpdateAndGetId<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        Task<long> InsertOrUpdateAndGetIdAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        TEntity Insert<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        void Delete<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        void Delete<TEntity>(long id) where TEntity : class, IEntity<long>;
        Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>;
        Task DeleteAsync<TEntity>(long id) where TEntity : class, IEntity<long>;
        void SoftDelete<TEntity>(TEntity entity) where TEntity : class, IEntity<long>, ISoftDelete;
        Task SoftDeleteAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>, ISoftDelete;
        void SoftDeleteRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity<long>, ISoftDelete;
        Task SoftDeleteRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity<long>, ISoftDelete;
        TEntity Get<TEntity>(long id) where TEntity : class, IEntity<long>;
        TPrimaryKey InsertAndGetId<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        Task<TEntity> GetAsync<TEntity>(long id) where TEntity : class, IEntity<long>;
        Task<IEnumerable<TEntity>> UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity<long>;
        IEnumerable<TEntity> UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity<long>;

        Task<IEnumerable<TEntityDto>> Sync<TEntityDto, TEntity>(IEnumerable<TEntityDto> input)
            where TEntity : class, IEntity<long>
            where TEntityDto : class, IEntityDto<long?>;

        Task<IEnumerable<TEntityDto>> Sync<TEntityDto, TEntity>(IEnumerable<TEntityDto> input, Expression<Func<TEntity, bool>> condition)
            where TEntity : class, IEntity<long>
            where TEntityDto : class, IEntityDto<long?>;
        Task<TEntity> InsertAsync<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        Task<IEnumerable<TEntityDto>> Sync<TEntityDto, TEntity>(
            IEnumerable<TEntityDto> input,
            Expression<Func<TEntity, bool>> condition,
            Func<TEntityDto, TEntityDto> merge)
            where TEntity : class, IEntity<long>
            where TEntityDto : class, IEntityDto<long?>;
        Task<TPrimaryKey> InsertAndGetIdAsync<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        TEntity Update<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        Task<TEntity> UpdateAsync<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        TPrimaryKey InsertOrUpdateAndGetId<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
        Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
    }
}
