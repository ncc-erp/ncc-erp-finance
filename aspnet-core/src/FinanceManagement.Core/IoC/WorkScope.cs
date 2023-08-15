using Abp;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.IoC
{
    public class WorkScope : AbpServiceBase, IWorkScope
    {
        private readonly IIocManager _iocManager;

        public WorkScope(IIocManager iocManager)
        {
            _iocManager = iocManager;
        }
        IQueryable<TEntity> IWorkScope.GetAll<TEntity, TPrimaryKey>()
        {
            return (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>().GetAll();
        }

        IQueryable<TEntity> IWorkScope.GetAll<TEntity>()
        {
            return (this as IWorkScope).GetRepo<TEntity, long>().GetAll();
        }

        IQueryable<TEntity> IWorkScope.All<TEntity>()
        {
            return (this as IWorkScope).GetRepo<TEntity, long>().GetAll();
        }

        IRepository<TEntity, TPrimaryKey> IWorkScope.GetRepo<TEntity, TPrimaryKey>()
        {
            var repoType = typeof(IRepository<,>);
            Type[] typeArgs = { typeof(TEntity), typeof(TPrimaryKey) };
            var repoGenericType = repoType.MakeGenericType(typeArgs);
            var resolveMethod = _iocManager.GetType()
                .GetMethods()
                .First(s => s.Name == "Resolve" && !s.IsGenericMethod && s.GetParameters().Length == 1 && s.GetParameters()[0].ParameterType == typeof(Type));
            var repo = resolveMethod.Invoke(_iocManager, new object[] { repoGenericType });
            return repo as IRepository<TEntity, TPrimaryKey>;
        }

        IRepository<TEntity, long> IWorkScope.GetRepo<TEntity>()
        {
            return (this as IWorkScope).GetRepo<TEntity, long>();
        }

        IRepository<TEntity, long> IWorkScope.Repository<TEntity>()
        {
            return (this as IWorkScope).GetRepo<TEntity, long>();
        }

        TEntity IWorkScope.Clone<TEntity>(TEntity entity)
        {
            entity.Id = 0;
            return (this as IWorkScope).Insert<TEntity, long>(entity);
        }

        TEntity IWorkScope.Insert<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return repo.Insert(entity);
        }

        private void UpdateTenantId<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            var tenantEntity = entity as IMayHaveTenant;
            if (tenantEntity != null)
                tenantEntity.TenantId = CurrentUnitOfWork.GetTenantId();
        }

        long IWorkScope.CloneAndGetId<TEntity>(TEntity entity)
        {
            entity.Id = 0;
            return (this as IWorkScope).InsertAndGetId<TEntity, long>(entity);
        }

        TPrimaryKey IWorkScope.InsertAndGetId<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return repo.InsertAndGetId(entity);
        }

        IEnumerable<TEntity> IWorkScope.InsertRange<TEntity>(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                yield return (this as IWorkScope).Insert<TEntity, long>(entity);
            }
        }

        async Task<IEnumerable<TEntity>> IWorkScope.InsertRangeAsync<TEntity>(IEnumerable<TEntity> entities)
        {
            var updatedEntities = new List<TEntity>();
            foreach (var entity in entities)
            {
                updatedEntities.Add(await (this as IWorkScope).InsertAsync<TEntity, long>(entity));
            }

            return updatedEntities;
        }

        Task<TEntity> IWorkScope.InsertAsync<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return repo.InsertAsync(entity);
        }

        public TEntity Insert<TEntity>(TEntity entity) where TEntity : class, IEntity<long>
        {
            return (this as IWorkScope).Insert<TEntity, long>(entity);
        }

        public async Task<TEntity> InsertAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>
        {
            return await (this as IWorkScope).InsertAsync<TEntity, long>(entity);
        }

        public long InsertAndGetId<TEntity>(TEntity entity) where TEntity : class, IEntity<long>
        {
            return (this as IWorkScope).InsertAndGetId<TEntity, long>(entity);
        }

        public async Task<long> InsertAndGetIdAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>
        {
            return await (this as IWorkScope).InsertAndGetIdAsync<TEntity, long>(entity);
        }

        Task<TPrimaryKey> IWorkScope.InsertAndGetIdAsync<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return repo.InsertAndGetIdAsync(entity);
        }

        public TEntity Update<TEntity>(TEntity entity) where TEntity : class, IEntity<long>
        {
            return (this as IWorkScope).Update<TEntity, long>(entity);
        }

        TEntity IWorkScope.Update<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return repo.Update(entity);
        }

        public async Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>
        {
            return await (this as IWorkScope).UpdateAsync<TEntity, long>(entity);
        }

        Task<TEntity> IWorkScope.UpdateAsync<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return repo.UpdateAsync(entity);
        }

        public long InsertOrUpdateAndGetId<TEntity>(TEntity entity) where TEntity : class, IEntity<long>
        {
            return (this as IWorkScope).InsertOrUpdateAndGetId<TEntity, long>(entity);
        }

        TPrimaryKey IWorkScope.InsertOrUpdateAndGetId<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return repo.InsertOrUpdateAndGetId(entity);
        }

        public async Task<long> InsertOrUpdateAndGetIdAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>
        {
            return await (this as IWorkScope).InsertOrUpdateAndGetIdAsync<TEntity, long>(entity);
        }

        Task<TPrimaryKey> IWorkScope.InsertOrUpdateAndGetIdAsync<TEntity, TPrimaryKey>(TEntity entity)
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, TPrimaryKey>();
            UpdateTenantId<TEntity, TPrimaryKey>(entity);
            return repo.InsertOrUpdateAndGetIdAsync(entity);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class, IEntity<long>
        {
            (this as IWorkScope).GetRepo<TEntity, long>().Delete(entity);
        }

        public void Delete<TEntity>(long id) where TEntity : class, IEntity<long>
        {
            (this as IWorkScope).GetRepo<TEntity, long>().Delete(id);
        }

        public async Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>
        {
            await (this as IWorkScope).GetRepo<TEntity, long>().DeleteAsync(entity);
        }

        public async Task DeleteAsync<TEntity>(long id) where TEntity : class, IEntity<long>
        {
            await (this as IWorkScope).GetRepo<TEntity, long>().DeleteAsync(id);
        }

        public void SoftDelete<TEntity>(TEntity entity) where TEntity : class, IEntity<long>, ISoftDelete
        {
            entity.IsDeleted = true;
            (this as IWorkScope).GetRepo<TEntity, long>().Update(entity);
        }

        public async Task SoftDeleteAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<long>, ISoftDelete
        {
            entity.IsDeleted = true;
            await (this as IWorkScope).GetRepo<TEntity, long>().UpdateAsync(entity);
        }

        public async Task SoftDeleteRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity<long>, ISoftDelete
        {
            foreach(var entity in entities)
            {
                entity.IsDeleted = true;
            }
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public void SoftDeleteRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity<long>, ISoftDelete
        {
            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
            }
            CurrentUnitOfWork.SaveChanges();
        }

        public TEntity Get<TEntity>(long id) where TEntity : class, IEntity<long>
        {
            return (this as IWorkScope).GetRepo<TEntity, long>().Get(id);
        }

        public async Task<TEntity> GetAsync<TEntity>(long id) where TEntity : class, IEntity<long>
        {
            return await (this as IWorkScope).GetRepo<TEntity, long>().GetAsync(id);
        }

        IEnumerable<TEntity> IWorkScope.UpdateRange<TEntity>(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                yield return (this as IWorkScope).Update<TEntity, long>(entity);
            }
        }

        async Task<IEnumerable<TEntity>> IWorkScope.UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities)
        {
            var updatedEntities = new List<TEntity>();
            foreach (var entity in entities)
            {
                updatedEntities.Add(await (this as IWorkScope).UpdateAsync<TEntity, long>(entity));
            }

            return updatedEntities;
        }

        public async Task<IEnumerable<TEntityDto>> Sync<TEntityDto, TEntity>(IEnumerable<TEntityDto> input)
            where TEntity : class, IEntity<long>
            where TEntityDto : class, IEntityDto<long?>
        {
            return await Sync<TEntityDto, TEntity>(input, x => false, x => x);
        }
        public async Task<IEnumerable<TEntityDto>> Sync<TEntityDto, TEntity>(IEnumerable<TEntityDto> input, Expression<Func<TEntity, bool>> condition)
            where TEntity : class, IEntity<long>
            where TEntityDto : class, IEntityDto<long?>
        {
            return await Sync<TEntityDto, TEntity>(input, condition, x => x);
        }

        public async Task<IEnumerable<TEntityDto>> Sync<TEntityDto, TEntity>(
            IEnumerable<TEntityDto> input,
            Expression<Func<TEntity, bool>> condition,
            Func<TEntityDto, TEntityDto> merge
        )
            where TEntity : class, IEntity<long>
            where TEntityDto : class, IEntityDto<long?>
        {
            var repo = (this as IWorkScope).GetRepo<TEntity, long>();
            if (input == null)
            {
                await repo.DeleteAsync(condition);
                return null;
            }

            var currentItems = await repo.GetAll().Where(condition).ToListAsync();
            var newItems = input.ToList();
            foreach (var item in currentItems)
            {
                var newItem = newItems.FirstOrDefault(t => item.Id == t.Id);
                if (newItem != null)
                {
                    newItem = merge(newItem);
                    // Mapper.Map(newItem, item);
                    ObjectMapper.Map(newItem, item);
                    UpdateTenantId<TEntity, long>(item);
                    repo.Update(item);
                }
                else
                {
                    repo.Delete(item);
                }
            }
            foreach (var item in newItems.Where(s => !s.Id.HasValue))
            {
                var newItem = item;
                newItem = merge(newItem);
                //item.Id = await repo.InsertAndGetIdAsync(Mapper.Map<TEntity>(newItem));
                var entity = ObjectMapper.Map<TEntity>(newItem);
                UpdateTenantId<TEntity, long>(entity);
                item.Id = await repo.InsertAndGetIdAsync(entity);
            }

            return newItems;
        }
    }
}
