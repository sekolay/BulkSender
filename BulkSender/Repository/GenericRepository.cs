using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Extensions;
using BulkSender.Entity;

namespace BulkSender.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private BulkSenderEntities _entities;

		public GenericRepository(BulkSenderEntities entities)
		{
			_entities = entities;
			_entities.Configuration.ProxyCreationEnabled = false;
		}
		public virtual IQueryable<T> All
		{
			get{
				return _entities.Set<T>();
			}
		}

		public virtual IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
		{
			IQueryable<T> query = _entities.Set<T>();
			return includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
		}

		public T GetById(Object id)
		{
			return _entities.Set<T>().Find(id);
		}

		public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
		{
			return _entities.Set<T>().Where(predicate);
		}

		public virtual void AddGraph(T entity)
		{
			_entities.Set<T>().Add(entity);
		}

		public virtual void Add(T entity)
		{
			DbEntityEntry dbEntityEntry = _entities.Entry<T>(entity);
			if (dbEntityEntry.State != EntityState.Detached)
				dbEntityEntry.State = EntityState.Added;
			else
				_entities.Set<T>().Add(entity);
			_entities.SaveChanges();
		}

		public virtual void AddRange(List<T> entityList)
		{
			_entities.Set<T>().AddRange(entityList);
			_entities.SaveChanges();
		}

		public virtual void Edit(T entity)
		{
			DbEntityEntry dbEntityEntry = _entities.Entry<T>(entity);
			if (dbEntityEntry.State == EntityState.Detached)
			{
				_entities.Set<T>().Attach(entity);
			}
			dbEntityEntry.State = EntityState.Modified;
			_entities.SaveChanges();
		}

		public virtual void Delete(T entity)
		{
			DbEntityEntry dbEntityEntry = _entities.Entry<T>(entity);
			if (dbEntityEntry.State != EntityState.Detached)
			{

				dbEntityEntry.State = EntityState.Deleted;
			}
			else
			{
				DbSet dbSet = _entities.Set<T>();
				dbSet.Attach(entity);
				dbSet.Remove(entity);
			}
			_entities.SaveChanges();
		}

		public virtual void Delete(Expression<Func<T, bool>> match)
		{
			_entities.Set<T>().Where(match).Delete();
		}

	}
}