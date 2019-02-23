using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Common
{
    public interface IFrom<T> where T : class, new()
    {
        /// <summary>
        /// Select
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        List<T> Select(string columns = "*");
        /// <summary>
        /// Select
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        List<TResult> Select<TResult>(string columns = "*");
        /// <summary>
        /// SelectAsync
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> SelectAsync(string columns = "*");
        /// <summary>
        /// SelectAsync
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        Task<IEnumerable<TResult>> SelectAsync<TResult>(string columns = "*");
        /// <summary>
        /// Select
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        List<T> Select(Expression<Func<T, object>> expression);
        /// <summary>
        /// Select
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        List<TResult> Select<TResult>(Expression<Func<T, TResult>> expression);
        /// <summary>
        /// SelectAsync
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> SelectAsync(Expression<Func<T, object>> expression);
        /// <summary>
        /// SelectAsync
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> expression);
        /// <summary>
        /// Single
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        T Single(string columns = "*");
        /// <summary>
        /// Single
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        TResult Single<TResult>(string columns = "*");
        /// <summary>
        /// SingleAsync
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> SingleAsync(string columns = "*");
        /// <summary>
        /// SingleAsync
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        Task<IEnumerable<TResult>> SingleAsync<TResult>(string columns = "*");
        /// <summary>
        /// Single
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        T Single(Expression<Func<T, object>> expression);
        /// <summary>
        /// Single
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        TResult Single<TResult>(Expression<Func<T, TResult>> expression);
        /// <summary>
        /// SingleAsync
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> SingleAsync(Expression<Func<T, object>> expression);
        /// <summary>
        /// SingleAsync
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<IEnumerable<TResult>> SingleAsync<TResult>(Expression<Func<T, TResult>> expression);
        /// <summary>
        /// Count
        /// </summary>
        /// <returns></returns>
        int Count();
        /// <summary>
        /// Exists
        /// </summary>
        /// <returns></returns>
        bool Exists();
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Insert(T entity);
        /// <summary>
        /// 异步插入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> InsertAsync(T entity);
        /// <summary>
        /// InsertById
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int InsertById(T entity);
        /// <summary>
        /// InsertByIdAsync
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> InsertByIdAsync(T entity);
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        int Insert(IEnumerable<T> entitys);
        /// <summary>
        /// InsertAsync
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        Task<int> InsertAsync(IEnumerable<T> entitys);
        /// <summary>
        /// Update
        /// </summary>
        /// <returns></returns>
        int Update();
        /// <summary>
        /// Condition Update
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int Update(bool condition);
        /// <summary>
        /// UpdateAsync
        /// </summary>
        /// <returns></returns>
        Task<int> UpdateAsync();
        /// <summary>
        /// Condition Update
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(bool condition);
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Update(T entity);
        /// <summary>
        /// UpdateAsync
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(T entity);
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        int Update(IEnumerable<T> entitys);
        /// <summary>
        /// UpdateAsync
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(IEnumerable<T> entitys);
        /// <summary>
        /// Delete
        /// </summary>
        /// <returns></returns>
        int Delete();
        /// <summary>
        /// DeleteAsync
        /// </summary>
        /// <returns></returns>
        Task<int> DeleteAsync();
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Delete(T entity);
        /// <summary>
        /// DeleteAsync
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(T entity);
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        int Delete(IEnumerable<T> entitys);
        /// <summary>
        /// DeleteAsync
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(IEnumerable<T> entitys);
        /// <summary>
        /// Distinct
        /// </summary>
        /// <returns></returns>
        IFrom<T> Distinct();
        /// <summary>
        /// Skip
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        IFrom<T> Skip(int index, int size, out int total);
        /// <summary>
        /// Where
        /// </summary>
        /// <param name="where"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IFrom<T> Where(string where);
        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IFrom<T> Where(WhereQuery<T> where);
        /// <summary>
        /// Where
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IFrom<T> Where(Expression<Func<T, bool>> where);
        /// <summary>
        /// Where
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        IFrom<T> Where(bool condition, Expression<Func<T, bool>> where);
        /// <summary>
        /// Add Param
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IFrom<T> AddParam(object param);
        /// <summary>
        /// Add Param
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IFrom<T> AddParam(bool condition, object param);
        /// <summary>
        /// Set custom sql
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IFrom<T> Set(string column, object value);
        /// <summary>
        /// Condition Set custom sql
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IFrom<T> Set(bool condition, string column, object value);
        /// <summary>
        /// Set columns
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IFrom<T> Set(Expression<Func<T, object>> column, object value);
        /// <summary>
        /// Condition Set custom sql
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IFrom<T> Set(bool condition, Expression<Func<T, object>> column, object value);
        /// <summary>
        /// Lambda Set
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        IFrom<T> Set(Expression<Func<T, object>> column, Expression<Func<T, object>> value);
        /// <summary>
        /// Condition Lambda Set
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IFrom<T> Set(bool condition, Expression<Func<T, object>> column, Expression<Func<T, object>> value);
        /// <summary>
        /// GroupBy
        /// </summary>
        /// <param name="groupby"></param>
        /// <returns></returns>
        IFrom<T> GroupBy(string groupby);
        /// <summary>
        /// GroupBy
        /// </summary>
        /// <param name="groupby"></param>
        /// <returns></returns>
        IFrom<T> GroupBy(Expression<Func<T, object>> groupby);
        /// <summary>
        /// Having
        /// </summary>
        /// <param name="having"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IFrom<T> Having(string having);
        /// <summary>
        /// Having
        /// </summary>
        /// <param name="having"></param>
        /// <returns></returns>
        IFrom<T> Having(WhereQuery<T> having);
        /// <summary>
        /// Having
        /// </summary>
        /// <param name="having"></param>
        /// <returns></returns>
        IFrom<T> Having(Expression<Func<T, bool>> having);
        /// <summary>
        /// Desc
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        IFrom<T> Desc(Expression<Func<T, object>> orderBy);
        /// <summary>
        /// Desc
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        IFrom<T> Desc(bool condition, Expression<Func<T, object>> orderBy);
        /// <summary>
        /// Asc
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        IFrom<T> Asc(Expression<Func<T, object>> orderBy);
        /// <summary>
        /// Asc
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        IFrom<T> Asc(bool condition, Expression<Func<T, object>> orderBy);
        /// <summary>
        /// OrderBy
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        IFrom<T> OrderBy(string orderBy);
        /// <summary>
        /// Limit
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        IFrom<T> Limit(int index, int size);
        /// <summary>
        /// Limit
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        IFrom<T> Limit(int size);
        /// <summary>
        /// XLock
        /// </summary>
        /// <returns></returns>
        IFrom<T> XLock();
        /// <summary>
        /// XLock
        /// </summary>
        /// <returns></returns>
        IFrom<T> SLock();
    }
}
