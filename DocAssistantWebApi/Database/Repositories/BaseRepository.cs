using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DocAssistant_Common.Models;
using DocAssistantWebApi.Database.Factories;
using Microsoft.EntityFrameworkCore;

namespace DocAssistantWebApi.Database.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : Entity
    {

        protected readonly IDatabaseFactory DatabaseFactory;

        protected BaseRepository(IDatabaseFactory databaseFactory)
        {
            this.DatabaseFactory = databaseFactory;
        }
        
        public virtual async Task<T> Get(T entity)
        {
            T dbEntity = null;
            
            await using var ctx = this.DatabaseFactory.Create();
            
            dbEntity = await ctx.GetSet<T>().FindAsync(entity);
            
            return dbEntity;
        }

        public virtual async Task<bool> UpdateChangedProperties(T entity, Expression<Func<T, bool>> expression = null)
        {
            
            await using var ctx = this.DatabaseFactory.Create();

            var dbEntity = await ctx.GetSet<T>()
                .FirstOrDefaultAsync(expression ?? (e => e.Id == entity.Id));

            if (dbEntity == null) return false;

            var updatedProperties = IRepository<T>.GetUpdatedProperties(dbEntity,entity);
;
            foreach (var property in updatedProperties)
            {
                dbEntity.GetType().GetProperty(property.Item1, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.NonPublic)
                    ?.SetValue(dbEntity,property.Item2);
            }

            return await ctx.GetContext().SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> Update(T entity)
        {

            await using var ctx = this.DatabaseFactory.Create();
        
            ctx.GetContext().Update(entity);

            return await ctx.GetContext().SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> Save(T entity)
        {
            await using var ctx = this.DatabaseFactory.Create();

            await ctx.GetContext().AddAsync(entity);
            return await ctx.GetContext().SaveChangesAsync() > 0;
        }
        public async Task<int> DeleteWhere(Expression<Func<T, bool>> expression)
        {
            await using var ctx = this.DatabaseFactory.Create();

            var entity = await ctx.GetSet<T>().FirstOrDefaultAsync(expression);

            if (entity == default) return 0;
            
            ctx.GetSet<T>().Remove(entity);
            await ctx.GetContext().SaveChangesAsync();

            return 1;
        }

        public virtual async Task<T> Where(Expression<Func<T,bool>> expression)
        {
            await using var ctx = this.DatabaseFactory.Create();

            return await ctx.GetSet<T>().FirstOrDefaultAsync(expression);
        }

        public virtual async Task<IEnumerable<T>> WhereMulti(Expression<Func<T, bool>> expression)
        {
            await using var ctx = this.DatabaseFactory.Create();

            return await ctx.GetSet<T>().Where(expression).ToListAsync();
        }
    }
}