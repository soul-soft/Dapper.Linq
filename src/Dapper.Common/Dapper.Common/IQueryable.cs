using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Collections;
using System.Threading.Tasks;

namespace Dapper.Common
{
    public interface IQueryable<T>
    {
        IQueryable<T> Set(string express, Action<Dictionary<string, object>> param = null, bool condition = true);
        IQueryable<T> Set<TResult>(Expression<Func<T, TResult>> column, TResult value, bool condition = true);
        IQueryable<T> Set<TResult>(Expression<Func<T, TResult>> column, Expression<Func<T, TResult>> expression, bool condition = true);
        IQueryable<T> GroupBy(string expression, bool condition = true);
        IQueryable<T> GroupBy<TResult>(Expression<Func<T, TResult>> expression, bool condition = true);
        IQueryable<T> Where(string expression, Action<Dictionary<string, object>> action = null, bool condition = true);
        IQueryable<T> Where(Expression<Func<T, bool>> expression, bool condition = true);
        IQueryable<T> OrderBy(string orderBy, bool condition = true);
        IQueryable<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression, bool condition = true);
        IQueryable<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> expression, bool condition = true);
        IQueryable<T> Skip(int index, int count, bool condition = true);
        IQueryable<T> Take(int count);
        IQueryable<T> Page(int index, int count, out long total, bool condition = true);
        IQueryable<T> Having(string expression, bool condition = true);
        IQueryable<T> Having(Expression<Func<T, bool>> expression, bool condition = true);
        IQueryable<T> Filter<TResult>(Expression<Func<T, TResult>> columns, bool condition = true);
        IQueryable<T> With(string lockType, bool condition = true);
        IQueryable<T> With(LockType lockType, bool condition = true);
        IQueryable<T> Distinct(bool condition = true);
        T Single(string columns = null, bool buffered = true, int? timeout = null);
        Task<T> SingleAsync(string columns = null, int? timeout = null);
        TResult Single<TResult>(string columns = null, bool buffered = true, int? timeout = null);
        Task<TResult> SingleAsync<TResult>(string columns = null, int? timeout = null);
        TResult Single<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null);
        Task<TResult> SingleAsync<TResult>(Expression<Func<T, TResult>> columns, int? timeout = null);
        IEnumerable<T> Select(string colums = null, bool buffered = true, int? timeout = null);
        Task<IEnumerable<T>> SelectAsync(string colums = null, int? timeout = null);
        IEnumerable<TResult> Select<TResult>(string colums = null, bool buffered = true, int? timeout = null);
        Task<IEnumerable<TResult>> SelectAsync<TResult>(string colums = null, int? timeout = null);
        IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null);
        Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> columns, int? timeout = null);
        int Insert(T entity, bool condition = true, int? timeout = null);
        Task<int> InsertAsync(T entity, bool condition = true, int? timeout = null);
        long InsertReturnId(T entity, bool condition = true, int? timeout = null);
        Task<long> InsertReturnIdAsync(T entity, bool condition = true, int? timeout = null);
        int Insert(IEnumerable<T> entitys, bool condition = true, int? timeout = null);
        Task<int> InsertAsync(IEnumerable<T> entitys, bool condition = true, int? timeout = null);
        int Update(bool condition = true, int? timeout = null);
        Task<int> UpdateAsync(bool condition = true, int? timeout = null);
        int Update(T entity, bool condition = true, int? timeout = null);
        Task<int> UpdateAsync(T entity, bool condition = true, int? timeout = null);
        int Update(IEnumerable<T> entitys, bool condition = true, int? timeout = null);
        Task<int> UpdateAsync(IEnumerable<T> entitys, bool condition = true, int? timeout = null);
        int Delete(bool condition = true, int? timeout = null);
        Task<int> DeleteAsync(bool condition = true, int? timeout = null);
        bool Exists(bool condition = true, int? timeout = null);
        Task<bool> ExistsAsync(bool condition = true, int? timeout = null);
        long Count(string columns = null, bool condition = true, int? timeout = null);
        Task<long> CountAsync(string columns = null, bool condition = true, int? timeout = null);
        long Count<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null);
        Task<long> CountAsync<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null);
        TResult Sum<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null);
        Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null);
    }
    public interface IQueryable<T1, T2>
    {
        IQueryable<T1, T2> Join(string expression);
        IQueryable<T1, T2> Join(Expression<Func<T1, T2, bool>> expression, JoinType join = JoinType.Inner);
        IQueryable<T1, T2> GroupBy(string expression, bool condition = true);
        IQueryable<T1, T2> GroupBy<TResult>(Expression<Func<T1, T2, TResult>> expression, bool condition = true);
        IQueryable<T1, T2> Where(string expression, Action<Dictionary<string, object>> action = null, bool condition = true);
        IQueryable<T1, T2> Where(Expression<Func<T1, T2, bool>> expression, bool condition = true);
        IQueryable<T1, T2> OrderBy(string orderBy, bool condition = true);
        IQueryable<T1, T2> OrderBy<TResult>(Expression<Func<T1, T2, TResult>> expression, bool condition = true);
        IQueryable<T1, T2> OrderByDescending<TResult>(Expression<Func<T1, T2, TResult>> expression, bool condition = true);
        IQueryable<T1, T2> Skip(int index, int count, bool condition = true);
        IQueryable<T1, T2> Take(int count);
        IQueryable<T1, T2> Page(int index, int count, out long total, bool condition = true);
        IQueryable<T1, T2> Having(string expression, bool condition = true);
        IQueryable<T1, T2> Having(Expression<Func<T1, T2, bool>> expression, bool condition = true);
        IQueryable<T1, T2> Distinct(bool condition = true);
        IEnumerable<TResult> Select<TResult>(string colums = null, bool buffered = true, int? timeout = null);
        Task<IEnumerable<TResult>> SelectAsync<TResult>(string colums = null, int? timeout = null);
        IEnumerable<TResult> Select<TResult>(Expression<Func<T1, T2, TResult>> columns, bool buffered = true, int? timeout = null);
        Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T1, T2, TResult>> columns, int? timeout = null);
        long Count(string columns = null, bool condition = true, int? timeout = null);
        Task<long> CountAsync(string columns = null, bool condition = true, int? timeout = null);
    }
    public interface IQueryable<T1, T2, T3>
    {
        IQueryable<T1, T2, T3> Join(string expression);
        IQueryable<T1, T2, T3> Join<E1, E2>(Expression<Func<E1, E2, bool>> expression, JoinType join = JoinType.Inner) where E1 : class where E2 : class;
        IQueryable<T1, T2, T3> GroupBy(string expression, bool condition = true);
        IQueryable<T1, T2, T3> GroupBy<TResult>(Expression<Func<T1, T2, T3, TResult>> expression, bool condition = true);
        IQueryable<T1, T2, T3> Where(string expression, Action<Dictionary<string, object>> action = null, bool condition = true);
        IQueryable<T1, T2, T3> Where(Expression<Func<T1, T2, T3, bool>> expression, bool condition = true);
        IQueryable<T1, T2, T3> OrderBy(string orderBy, bool condition = true);
        IQueryable<T1, T2, T3> OrderBy<TResult>(Expression<Func<T1, T2, T3, TResult>> expression, bool condition = true);
        IQueryable<T1, T2, T3> OrderByDescending<TResult>(Expression<Func<T1, T2, T3, TResult>> expression, bool condition = true);
        IQueryable<T1, T2, T3> Skip(int index, int count, bool condition = true);
        IQueryable<T1, T2, T3> Take(int count);
        IQueryable<T1, T2, T3> Page(int index, int count, out long total, bool condition = true);
        IQueryable<T1, T2, T3> Having(string expression, bool condition = true);
        IQueryable<T1, T2, T3> Having(Expression<Func<T1, T2, T3, bool>> expression, bool condition = true);
        IQueryable<T1, T2, T3> Distinct(bool condition = true);
        IEnumerable<TResult> Select<TResult>(string colums = null, bool buffered = true, int? timeout = null);
        Task<IEnumerable<TResult>> SelectAsync<TResult>(string colums = null, int? timeout = null);
        IEnumerable<TResult> Select<TResult>(Expression<Func<T1, T2, T3, TResult>> columns, bool buffered = true, int? timeout = null);
        Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T1, T2, T3, TResult>> columns, int? timeout = null);
        long Count(string columns = null, bool condition = true, int? timeout = null);
        Task<long> CountAsync(string columns = null, bool condition = true, int? timeout = null);
    }

    public enum LockType
    {
        FOR_UPADTE,
        LOCK_IN_SHARE_MODE,
        UPDLOCK,
        NOLOCK
    }
    public enum JoinType
    {
        Inner,
        Left,
        Right
    }
}
