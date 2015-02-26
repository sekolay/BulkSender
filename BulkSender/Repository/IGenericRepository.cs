using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BulkSender.Repository
{
	public interface IGenericRepository<T> where T : class
	{
		IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
		IQueryable<T> All { get; }
		T GetById(object id);
		IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

		void Add(T entity);
		void Delete(T entity);
		void Delete(Expression<Func<T, bool>> match);
		void Edit(T entity);
	}
}