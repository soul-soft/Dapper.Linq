using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Common
{
    /// <summary>
    /// SQL构建对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFrom<T> where T : class, new()
    {
        /// <summary>
        /// Select：返回列表
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        List<T> Select(string columns = "*");
        /// <summary>
        /// Select:返回列表
        /// </summary>
        /// <param name="columns">字段列表</param>
        /// <returns></returns>
        List<TReturn> Select<TReturn>(string columns = "*");
        /// <summary>
        /// SelectAsync:异步返回列表
        /// </summary>
        /// <param name="columns">字段列表</param>
        /// <returns></returns>
        Task<IEnumerable<T>> SelectAsync(string columns = "*");
        /// <summary>
        /// SelectAsync:异步返一个新的类型
        /// 不支持匿名类型
        /// </summary>
        /// <param name="columns">字段列表</param>
        /// <returns></returns>
        Task<IEnumerable<TReturn>> SelectAsync<TReturn>(string columns = "*");      
        /// <summary>
        /// Select：返回一个新的类型
        /// 不支持匿名类型
        /// </summary>
        /// <param name="expression">字段列表</param>
        /// <returns></returns>
        List<TReturn> Select<TReturn>(Expression<Func<T, TReturn>> expression);
        /// <summary>
        /// SelectAsync：返回一个新的类型
        /// 不支持匿名类型
        /// </summary>
        /// <param name="expression">字段列表</param>
        /// <returns></returns>
        Task<IEnumerable<TReturn>> SelectAsync<TReturn>(Expression<Func<T, TReturn>> expression);
        /// <summary>
        /// Single：返回单个实体
        /// </summary>
        /// <param name="columns">字段列表</param>
        /// <returns></returns>
        T Single(string columns = "*");
        /// <summary>
        /// Single
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        TReturn Single<TReturn>(string columns = "*");
        /// <summary>
        /// SingleAsync
        /// </summary>
        /// <param name="columns">字段列表</param>
        /// <returns></returns>
        Task<IEnumerable<T>> SingleAsync(string columns = "*");
        /// <summary>
        /// SingleAsync
        /// </summary>
        /// <param name="columns">字段列表</param>
        /// <returns></returns>
        Task<IEnumerable<TReturn>> SingleAsync<TReturn>(string columns = "*");
        /// <summary>
        /// Single
        /// </summary>
        /// <param name="expression">字段列表</param>
        /// <returns></returns>
        TReturn Single<TReturn>(Expression<Func<T, TReturn>> expression);
        /// <summary>
        /// SingleAsync
        /// </summary>
        /// <param name="expression">字段列表</param>
        /// <returns></returns>
        Task<IEnumerable<TReturn>> SingleAsync<TReturn>(Expression<Func<T, TReturn>> expression);
        /// <summary>
        /// Count：返回满足Where条件的记录数
        /// </summary>
        /// <returns></returns>
        int Count();
        /// <summary>
        /// Exists：判断满足Where条件的记录数是否存在
        /// </summary>
        /// <returns>个数</returns>
        bool Exists();
        /// <summary>
        /// Insert：新增实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>bool</returns>
        int Insert(T entity);
        /// <summary>
        /// 异步：新增实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>影响行数</returns>
        Task<int> InsertAsync(T entity);
        /// <summary>
        /// 新增实体：返回自增列
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回自增列</returns>
        int InsertById(T entity);
        /// <summary>
        /// 异步新增实体：返回自增列
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回自增列</returns>
        Task<int> InsertByIdAsync(T entity);
        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <returns>影响行数</returns>
        int Insert(IEnumerable<T> entitys);
        /// <summary>
        /// 异步批量新增
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <returns>影响行数</returns>
        Task<int> InsertAsync(IEnumerable<T> entitys);
        /// <summary>
        /// 执行更新
        /// </summary>
        /// <returns>影响行数</returns>
        int Update();
        /// <summary>
        /// 异步：执行更新
        /// </summary>
        /// <returns>影响行数</returns>
        Task<int> UpdateAsync();
        /// <summary>
        /// 根据主键更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>影响行数</returns>
        int Update(T entity);
        /// <summary>
        /// 异步：根据主键更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>影响行数</returns>
        Task<int> UpdateAsync(T entity);
        /// <summary>
        /// 批量新增实体
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <returns>影响行数</returns>
        int Update(IEnumerable<T> entitys);
        /// <summary>
        /// 异步：根据主键批量修改实体
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <returns>影响行数</returns>
        Task<int> UpdateAsync(IEnumerable<T> entitys);
        /// <summary>
        /// 执行删除操作
        /// </summary>
        /// <returns>影响行数</returns>
        int Delete();
        /// <summary>
        /// 异步：执行删除操作
        /// </summary>
        /// <returns>影响行数</returns>
        Task<int> DeleteAsync();
        /// <summary>
        /// 去重
        /// </summary>
        /// <returns>影响行数</returns>
        IFrom<T> Distinct();
        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>IFrom</returns>
        IFrom<T> Where(string expression);
        /// <summary>
        ///过滤
        /// </summary>
        /// <param name="whereQuery">动态表达式</param>
        /// <returns>IFrom</returns>
        IFrom<T> Where(WhereQuery<T> whereQuery);
        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="whereQuery">动态表达式</param>
        /// <returns>IFrom</returns>
        IFrom<T> Where(Expression<Func<T, bool>> whereQuery);
        /// <summary>
        /// 条件过滤
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="expression">表达式</param>
        /// <returns>IFrom</returns>
        IFrom<T> Where(bool condition, Expression<Func<T, bool>> expression);
        /// <summary>
        /// 更新列
        /// </summary>
        /// <param name="column">列</param>
        /// <param name="value">值</param>
        /// <returns>IFrom</returns>
        IFrom<T> Set(string column, object value);
        /// <summary>
        /// 条件更新列
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="column">列</param>
        /// <param name="value">值</param>
        /// <returns>IFrom</returns>
        IFrom<T> Set(bool condition, string column, object value);
        /// <summary>
        /// 更新列
        /// </summary>
        /// <param name="column">列</param>
        /// <param name="value">值</param>
        /// <returns>IFrom</returns>
        IFrom<T> Set(Expression<Func<T, object>> column, object value);
        /// <summary>
        /// 条件更新列
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="column">列</param>
        /// <param name="value">值</param>
        /// <returns>IFrom</returns>
        IFrom<T> Set(bool condition, Expression<Func<T, object>> column, object value);
        /// <summary>
        /// 更新列
        /// </summary>
        /// <param name="column">列</param>
        /// <param name="expression"></param>
        /// <returns>IFrom</returns>
        IFrom<T> Set(Expression<Func<T, object>> column, Expression<Func<T, object>> expression);
        /// <summary>
        /// 条件更新列
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="column">列</param>
        /// <param name="expression">表达式</param>
        /// <returns>IFrom</returns>
        IFrom<T> Set(bool condition, Expression<Func<T, object>> column, Expression<Func<T, object>> expression);
        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="groupby">表达式</param>
        /// <returns>IFrom</returns>
        IFrom<T> GroupBy(string groupby);
        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="groupby">表达式</param>
        /// <returns>IFrom</returns>
        IFrom<T> GroupBy(Expression<Func<T, object>> groupby);
        /// <summary>
        /// 分组之后筛选
        /// </summary>
        /// <param name="having">表达式</param>
        /// <returns>IFrom</returns>
        IFrom<T> Having(string having);
        /// <summary>
        /// 分组之后筛选
        /// </summary>
        /// <param name="whereQuery">动态条件</param>
        /// <returns>IFrom</returns>
        IFrom<T> Having(WhereQuery<T> whereQuery);
        /// <summary>
        /// 分组之后筛选
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>IFrom</returns>
        IFrom<T> Having(Expression<Func<T, bool>> expression);
        /// <summary>
        /// 倒叙排序
        /// </summary>
        /// <param name="orderBy">表达式</param>
        /// <returns>IFrom</returns>
        IFrom<T> Desc(Expression<Func<T, object>> orderBy);
        /// <summary>
        /// 条件倒叙排序
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="orderBy">表达式</param>
        /// <returns>IFrom</returns>
        IFrom<T> Desc(bool condition, Expression<Func<T, object>> orderBy);
        /// <summary>
        /// 升序
        /// </summary>
        /// <param name="orderBy">表达式</param>
        /// <returns>IFrom</returns>
        IFrom<T> Asc(Expression<Func<T, object>> orderBy);
        /// <summary>
        /// 条件升序
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="orderBy">表达式</param>
        /// <returns>IFrom</returns>
        IFrom<T> Asc(bool condition, Expression<Func<T, object>> orderBy);
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="orderBy">表达式</param>
        /// <returns>IFrom</returns>
        IFrom<T> OrderBy(string orderBy);
        /// <summary>
        /// 从index开始返回count条记录
        /// </summary>
        /// <param name="index">起始位置</param>
        /// <param name="count">个数</param>
        /// <returns>IFrom</returns>
        IFrom<T> Skip(int index, int count);
        /// <summary>
        /// 从0开始返回count条记录
        /// </summary>
        /// <param name="count">个数</param>
        /// <returns>IFrom</returns>
        IFrom<T> Skip(int count);
        /// <summary>
        /// 从index开始获取count条记录,并返回总个数
        /// </summary>
        /// <param name="index">起始位置</param>
        /// <param name="count">返回个数</param>
        /// <param name="total">总个数</param>
        /// <returns>IFrom</returns>
        IFrom<T> Skip(int index, int count, out int total);
        /// <summary>
        /// 悲观锁
        /// </summary>
        /// <returns>IFrom</returns>
        IFrom<T> XLock();
        /// <summary>
        /// 共享锁
        /// </summary>
        /// <returns>IFrom</returns>
        IFrom<T> SLock();
    }
}
