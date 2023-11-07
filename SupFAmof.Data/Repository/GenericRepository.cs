﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SupFAmof.Data.Entity;

namespace SupFAmof.Data.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private static SupFAmOf_Stg_DbContext Context;
        //private static SupFAmOf_Stg_Db_Ver_2Context Context;
        private static DbSet<T> Table { get; set; }

        public GenericRepository(SupFAmOf_Stg_DbContext context)
        {
            Context = context;
            Table = Context.Set<T>();
        }

        private string GenerateRedisKey(Expression<Func<T, bool>> predicate)
        {
            // Generate a unique key for caching based on the predicate
            return $"{typeof(T).FullName}_{predicate.ToString()}";
        }

        public T Find(Func<T, bool> predicate)
        {
            return Table.FirstOrDefault(predicate);
        }
        public async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return
                await Table.SingleOrDefaultAsync(predicate);
        }

        public IQueryable<T> FindAll(Func<T, bool> predicate)
        {
            return Table.Where(predicate).AsQueryable();
        }

        public DbSet<T> GetAll()
        {
            return Table;
        }


        public async Task<T> GetByIdGuid(Guid Id)
        {
            return await Table.FindAsync(Id);
        }

        public async Task<T> GetById(int Id)
        {
            return await Table.FindAsync(Id);
        }

        public async Task HardDeleteGuid(Guid key)
        {
            var rs = await GetByIdGuid(key);
            Table.Remove(rs);
        }

        public async Task HardDelete(int key)
        {
            var rs = await GetById(key);
            Table.Remove(rs);
        }

        public void Insert(T entity)
        {
            Table.Add(entity);
        }

        public async Task UpdateGuid(T entity, Guid Id)
        {
            var existEntity = await GetByIdGuid(Id);
            Context.Entry(existEntity).CurrentValues.SetValues(entity);
            Table.Update(existEntity);
        }

        public async Task Update(T entity, int Id)
        {
            var existEntity = await GetById(Id);
            Context.Entry(existEntity).CurrentValues.SetValues(entity);
            Table.Update(existEntity);
        }


        public void UpdateRange(IQueryable<T> entities)
        {
            Table.UpdateRange(entities);
        }

        public void DeleteRange(IQueryable<T> entities)
        {
            Table.RemoveRange(entities);
        }

        public void InsertRange(IQueryable<T> entities)
        {
            Table.AddRange(entities);
        }

        public EntityEntry<T> Delete(T entity)
        {
            return Table.Remove(entity);
        }

        //async
        public async Task InsertAsync(T entity)
        {
            await Table.AddAsync(entity);
        }

        public async Task InsertRangeAsync(IQueryable<T> entities)
        {
            await Table.AddRangeAsync(entities);
        }

        public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
        {
            return await Table.Where(predicate).ToListAsync();
        }

        public IQueryable<T> GetAllApart()
        {
            return Table.Take(100);
        }
        public async Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken = default)
        {
            return await Context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task UpdateDetached(T entity)
        {
            Table.Update(entity);
        }

        public async Task DetachEntity(T entity)
        {
            Context.Entry(entity).State = EntityState.Detached;
        }

        public IQueryable<T> AsNoTracking()
        {
            return Table.AsNoTracking();
        }
    }
}
