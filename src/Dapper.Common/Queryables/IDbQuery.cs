using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dapper
{
    /// <summary>
    /// linq 查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDbQuery<T>
    {
        /// <summary>
        /// 通过主键检索数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Get(object id);
        /// <summary>
        /// 异步通过主键检索数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetAsync(object id);
        /// <summary>
        /// count查询
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        int Count(int? commandTimeout = null);
        /// <summary>
        /// 异步count查询
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        Task<int> CountAsync(int? commandTimeout = null);
        /// <summary>
        /// count查询
        /// </summary>
        /// <typeparam name="TResult">类型推断</typeparam>
        /// <param name="expression">字段列表</param>
        /// <returns></returns>
        int Count<TResult>(Expression<Func<T, TResult>> expression);
        /// <summary>
        /// 异步count查询
        /// </summary>
        /// <typeparam name="TResult">类型推断</typeparam>
        /// <param name="expression">字段列表</param>
        /// <returns></returns>
        Task<int> CountAsync<TResult>(Expression<Func<T, TResult>> expression);
        /// <summary>
        /// delete查询
        /// </summary>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        int Delete(int? commandTimeout = null);
        /// <summary>
        /// 异步delete查询
        /// </summary>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(int? commandTimeout = null);
        /// <summary>
        /// delete查询
        /// </summary>
        /// <param name="expression">查询条件</param>
        /// <returns></returns>
        int Delete(Expression<Func<T, bool>> expression);
        /// <summary>
        /// 异步delete查询
        /// </summary>
        /// <param name="expression">查询条件</param>
        /// <returns></returns>
        Task<int> DeleteAsync(Expression<Func<T, bool>> expression);
        /// <summary>
        /// exists查询
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        bool Exists(int? commandTimeout = null);
        /// <summary>
        /// 异步exists查询
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        Task<bool> ExistsAsync(int? commandTimeout = null);
        /// <summary>
        /// exists查询
        /// </summary>
        /// <param name="expression">查询条件</param>
        /// <returns></returns>
        bool Exists(Expression<Func<T, bool>> expression);
        /// <summary>
        /// 异步exists查询
        /// </summary>
        /// <param name="expression">查询条件</param>
        /// <returns></returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);
        /// <summary>
        /// update查询，如果没有指定where则应用到所有记录
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        int Update(int? commandTimeout = null);
        /// <summary>
        /// 异步update查询，如果没有指定where则应用到所有记录
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        Task<int> UpdateAsync(int? commandTimeout = null);
        /// <summary>
        /// update查询，默认根据Primarkey更新，如果存在where则仅使用指定更新条件，
        /// 无法通过该接口更新主键字段和主键字段
        /// </summary>
        /// <param name="entity">参数</param>
        /// <returns></returns>
        int Update(T entity);
        /// <summary>
        /// 异步update查询，默认根据Primarkey更新，如果存在where则仅使用指定更新条件，
        /// 无法通过该接口更新主键字段和主键字段
        /// </summary>
        /// <param name="entity">参数</param>
        /// <returns></returns>
        Task<int> UpdateAsync(T entity);
        /// <summary>
        /// insert查询，该接口会忽略identity字段
        /// </summary>
        /// <param name="entity">参数</param>
        /// <returns></returns>
        int Insert(T entity);
        /// <summary>
        /// 异步insert查询，该接口会忽略identity字段
        /// </summary>
        /// <param name="entity">参数</param>
        /// <returns></returns>
        Task<int> InsertAsync(T entity);
        /// <summary>
        /// insert查询，并返回id，该接口会忽略identity字段
        /// </summary>
        /// <param name="entity">参数</param>
        /// <returns></returns>
        int InsertReturnId(T entity);
        /// <summary>
        /// 异步insert查询，并返回id，该接口会忽略identity字段
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> InsertReturnIdAsync(T entity);
        /// <summary>
        /// 批量insert查询，该接口会忽略identity字段
        /// </summary>
        /// <param name="entitys">参数集合</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        int Insert(IEnumerable<T> entitys,int? commandTimeout = null);
        /// <summary>
        /// 异步批量insert查询，该接口会忽略identity字段
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        Task<int> InsertAsync(IEnumerable<T> entitys, int? commandTimeout = null);
        /// <summary>
        /// select查询
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        IEnumerable<T> Select(int? commandTimeout = null);
        /// <summary>
        /// 异步select查询
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        Task<IEnumerable<T>> SelectAsync(int? commandTimeout = null);
        /// <summary>
        /// 分页select查询
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>结果集，总记录数</returns>
        (IEnumerable<T>, int) SelectMany(int? commandTimeout = null);
        /// <summary>
        /// 异步分页select查询
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        Task<(IEnumerable<T>, int)> SelectManyAsync(int? commandTimeout = null);
        /// <summary>
        /// select查询
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">字段列表</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression, int? commandTimeout = null);
        /// <summary>
        /// 异步select查询
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">字段列表</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> expression, int? commandTimeout = null);
        /// <summary>
        /// 分页select查询
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">字段列表</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        (IEnumerable<TResult>, int) SelectMany<TResult>(Expression<Func<T, TResult>> expression, int? commandTimeout = null);
        /// <summary>
        /// 异步分页select查询
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">字段列表</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        Task<(IEnumerable<TResult>, int)> SelectManyAsync<TResult>(Expression<Func<T, TResult>> expression, int? commandTimeout = null);
        /// <summary>
        /// select查询
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        T Single(int? commandTimeout = null);
        /// <summary>
        /// 异步select查询
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        Task<T> SingleAsync(int? commandTimeout = null);
        /// <summary>
        /// select查询
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">字段列表</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        TResult Single<TResult>(Expression<Func<T, TResult>> expression, int? commandTimeout = null);
        /// <summary>
        /// 异步select查询
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">字段列表</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        Task<TResult> SingleAsync<TResult>(Expression<Func<T, TResult>> expression, int? commandTimeout = null);
        /// <summary>
        /// 在insert,update,select时过滤字段
        /// </summary>
        /// <typeparam name="TResult">类型推断</typeparam>
        /// <param name="expression">字段列表</param>
        /// <returns></returns>
        IDbQuery<T> Filter<TResult>(Expression<Func<T, TResult>> expression);
        /// <summary>
        /// set查询
        /// </summary>
        /// <typeparam name="TResult">类型推断</typeparam>
        /// <param name="column">字段</param>
        /// <param name="value">参数</param>
        /// <param name="condition">是否有效</param>
        /// <returns></returns>
        IDbQuery<T> Set<TResult>(Expression<Func<T, TResult>> column, TResult value, bool condition = true);
        /// <summary>
        /// set查询
        /// </summary>
        /// <typeparam name="TResult">类型推断</typeparam>
        /// <param name="column">字段</param>
        /// <param name="expression">表达式</param>
        /// <param name="condition">是否有效</param>
        /// <returns></returns>
        IDbQuery<T> Set<TResult>(Expression<Func<T, TResult>> column, Expression<Func<T, TResult>> expression, bool condition = true);
        /// <summary>
        /// take查询，从下标为0的行获取count条记录
        /// </summary>
        /// <param name="count">记录个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        IDbQuery<T> Take(int count, bool condition = true);
        /// <summary>
        /// skip，从下标为index的行获取count条记录
        /// </summary>
        /// <param name="index">起始下标</param>
        /// <param name="count">记录个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        IDbQuery<T> Skip(int index, int count, bool condition = true);
        /// <summary>
        /// page查询，从下标为(index-1)*count的行获取count条记录
        /// </summary>
        /// <param name="index">起始页码</param>
        /// <param name="count">记录个数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        IDbQuery<T> Page(int index, int count, bool condition = true);
        /// <summary>
        /// 指定读锁
        /// </summary>
        /// <param name="lockname"></param>
        /// <returns></returns>
        IDbQuery<T> With(string lockname);
        /// <summary>
        /// where查询，多个where有效使用and连接
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="condition">是否有效</param>
        /// <returns></returns>
        IDbQuery<T> Where(Expression<Func<T, bool>> expression, bool condition = true);
        /// <summary>
        /// having查询，多个having查询有效使用and连接
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="condition">是否有效</param>
        /// <returns></returns>
        IDbQuery<T> Having(Expression<Func<T, bool>> expression, bool condition = true);
        /// <summary>
        /// group查询
        /// </summary>
        /// <typeparam name="TResult">类型推断</typeparam>
        /// <param name="expression">字段列表</param>
        /// <returns></returns>
        IDbQuery<T> GroupBy<TResult>(Expression<Func<T, TResult>> expression);
        /// <summary>
        /// orderby查询，升序
        /// </summary>
        /// <typeparam name="TResult">类型推断</typeparam>
        /// <param name="expression">字段列表</param>
        /// <returns></returns>
        IDbQuery<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression);
        /// <summary>
        /// orderby查询，降序
        /// </summary>
        /// <typeparam name="TResult">类型推断</typeparam>
        /// <param name="expression">字段列表</param>
        /// <returns></returns>
        IDbQuery<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> expression);
        /// <summary>
        /// 求和
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">字段列表</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        TResult Sum<TResult>(Expression<Func<T, TResult>> expression, int? commandTimeout = null);
        /// <summary>
        /// 异步求和
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">字段列表</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression, int? commandTimeout = null);
    }
}
