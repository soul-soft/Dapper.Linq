using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Collections;

namespace Dapper.Extension
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
        IQueryable<T> With(string locks, bool condition = true);
        IQueryable<T> With(LockType locks, bool condition = true);
        IQueryable<T> Distinct(bool condition = true);
        T Single(string columns = null, bool buffered = true, int? timeout = null);
        TResult Single<TResult>(string columns = null, bool buffered = true, int? timeout = null);
        TResult Single<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null);
        IEnumerable<T> Select(string colums = null, bool buffered = true, int? timeout = null);
        IEnumerable<TResult> Select<TResult>(string colums = null, bool buffered = true, int? timeout = null);
        IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null);
        int Insert(T entity, bool condition = true, int? timeout = null);
        long InsertReturnId(T entity, bool condition = true, int? timeout = null);
        int Insert(IEnumerable<T> entitys, bool condition = true, int? timeout = null);
        int Update(bool condition = true, int? timeout = null);
        int Update(T entity, bool condition = true, int? timeout = null);
        int Update(IEnumerable<T> entitys, bool condition = true, int? timeout = null);
        int Delete(bool condition = true, int? timeout = null);
        bool Exists(bool condition = true, int? timeout = null);
        long Count(string columns = null, bool condition = true, int? timeout = null);
        long Count<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null);
        TResult Sum<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null);
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
        TResult Single<TResult>(string columns = null, bool buffered = true, int? timeout = null);
        TResult Single<TResult>(Expression<Func<T1, T2, TResult>> columns, bool buffered = true, int? timeout = null);
        IEnumerable<TResult> Select<TResult>(string colums = null, bool buffered = true, int? timeout = null);
        IEnumerable<TResult> Select<TResult>(Expression<Func<T1, T2, TResult>> columns, bool buffered = true, int? timeout = null);
        bool Exists(bool condition = true, int? timeout = null);
        long Count(string columns = null, bool condition = true, int? timeout = null);
        long Count<TResult>(Expression<Func<T1, T2, TResult>> expression, bool condition = true, int? timeout = null);
        TResult Sum<TResult>(Expression<Func<T1, T2, TResult>> expression, bool condition = true, int? timeout = null);
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
