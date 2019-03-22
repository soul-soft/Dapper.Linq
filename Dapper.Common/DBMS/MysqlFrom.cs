using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Dapper.Common
{
    internal class MysqlFrom<T> : IFrom<T> where T : class, new()
    {
        #region Construction
        /// <summary>
        /// 
        /// </summary>
        public MysqlFrom()
        {
            FromSql = TypeMapper.GetTableName<T>();
        }
        #endregion

        #region Props
        /// <summary>
        /// 会话事物
        /// </summary>
        public ISession Session { get; set; }
        /// <summary>
        /// 查询参数
        /// </summary>
        private Dictionary<string, object> Param = new Dictionary<string, object>();
        /// <summary>
        /// 查询SQL
        /// </summary>
        private StringBuilder QuerySql = new StringBuilder();
        /// <summary>
        /// 插入SQL
        /// </summary>
        private StringBuilder InsertSql = new StringBuilder();
        /// <summary>
        /// 删除SQL
        /// </summary>
        private StringBuilder DeleteSql = new StringBuilder();
        /// <summary>
        /// 更新SQL
        /// </summary>
        private StringBuilder UpdateSql = new StringBuilder();
        /// <summary>
        /// 分页SQL
        /// </summary>
        private StringBuilder SkipSql = new StringBuilder();
        /// <summary>
        /// 表
        /// </summary>
        private string FromSql { get; set; }
        #endregion

        #region ADO.Net

        #region Select
        /// <summary>
        /// 构建Select语句
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private string SelectBuild(string columns = "*")
        {
            columns = columns == "*" ? string.Join(",", TypeMapper.GetDbColumn<T>().Select(s => string.Format("{0} AS {1}", s.ColumnName, s.FieldName))) : columns;
            columns = (_distinct != null ? _distinct + " " : "") + columns;
            QuerySql.AppendFormat("SELECT {0} FROM {1}", columns, FromSql);
            if (_where.Length > 0)
            {
                QuerySql.AppendFormat(" WHERE {0}", _where);
            }
            if (_groupBy.Length > 0)
            {
                QuerySql.AppendFormat(" GROUP BY {0}", _groupBy);
            }
            if (_having.Length > 0)
            {
                QuerySql.AppendFormat(" HAVING {0}", _having);
            }
            if (_orderBy.Length > 0)
            {
                QuerySql.AppendFormat(" ORDER BY {0}", _orderBy);
            }
            if (_limit.Length > 0)
            {
                QuerySql.AppendFormat(" LIMIT {0}", _limit);
            }
            if (_lock.Length > 0)
            {
                QuerySql.AppendFormat(" {0}", _lock);
            }
            return QuerySql.ToString();
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="columns">字段列表</param>
        /// <returns></returns>
        public List<T> Select(string columns = "*")
        {
            var sql = SelectBuild(columns);
            var list = Session.Query<T>(sql, Param, CommandType.Text).ToList();
            return list;
        }
        /// <summary>
        /// 查询数据
        /// 不支持匿名类型
        /// </summary>
        /// <param name="columns">字段列表</param>
        /// <returns></returns>
        public List<TReturn> Select<TReturn>(string columns = "*")
        {
            var sql = SelectBuild(columns);
            var list = Session.Query<TReturn>(sql, Param, CommandType.Text).ToList();
            return list;
        }
        /// <summary>
        /// 异步查询
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> SelectAsync(string columns = "*")
        {
            var sql = SelectBuild(columns);
            var task = Session.QueryAsync<T>(sql, Param, CommandType.Text);
            return task;
        }
        /// <summary>
        /// 异步查询
        /// 不支持匿名类型
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Task<IEnumerable<TReturn>> SelectAsync<TReturn>(string columns = "*")
        {
            var sql = SelectBuild(columns);
            var task = Session.QueryAsync<TReturn>(sql, Param, CommandType.Text);
            return task;
        }
        /// <summary>
        /// 查询数据:指定字段列表
        /// 不支持匿名类型
        /// </summary>
        /// <param name="expression">字段列表</param>
        /// <returns></returns>
        public List<TReturn> Select<TReturn>(Expression<Func<T, TReturn>> expression)
        {
            var data = Select<TReturn>(new FunVisitor<T>().Build(Param, expression, true));
            return data;
        }

        /// <summary>
        /// 异步查询,指定字段列表
        /// 不支持匿名类型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Task<IEnumerable<TReturn>> SelectAsync<TReturn>(Expression<Func<T, TReturn>> expression)
        {
            var data = SelectAsync<TReturn>(new FunVisitor<T>().Build(Param, expression, true));
            return data;
        }
        /// <summary>
        /// 查询单个数据
        /// </summary>
        /// <param name="columns">字段列表</param>
        /// <returns></returns>
        public T Single(string columns = "*")
        {
            Skip(1);
            var sql = SelectBuild(columns);
            var data = Session.Query<T>(sql, Param).SingleOrDefault();
            return data;
        }
        /// <summary>
        /// 查询单个数据
        /// 不支持匿名类型
        /// </summary>
        /// <param name="columns">字段列表</param>
        /// <returns></returns>
        public TReturn Single<TReturn>(string columns = "*")
        {
            Skip(1);
            var sql = SelectBuild(columns);
            var data = Session.Query<TReturn>(sql, Param).SingleOrDefault();
            return data;
        }
        /// <summary>
        /// 异步查询单个数据
        /// 不支持匿名类型
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Task<IEnumerable<TReturn>> SingleAsync<TReturn>(string columns = "*")
        {
            Skip(1);
            var sql = SelectBuild(columns);
            var task = Session.QueryAsync<TReturn>(sql, Param);
            return task;
        }
        /// <summary>
        /// 异步查询单个数据
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> SingleAsync(string columns = "*")
        {
            Skip(1);
            var sql = SelectBuild(columns);
            var task = Session.QueryAsync<T>(sql, Param);
            return task;
        }
        /// <summary>
        /// 查询单个数据:指定查询列
        /// 不支持匿名类型
        /// </summary>
        /// <param name="expression">字段列表</param>
        /// <returns></returns>
        public TReturn Single<TReturn>(Expression<Func<T, TReturn>> expression)
        {
            var data = Single<TReturn>(new FunVisitor<T>().Build(Param, expression, true));
            return data;
        }
        /// <summary>
        /// 异步查询单个实体:指定查询列
        /// 不支持匿名类型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Task<IEnumerable<TReturn>> SingleAsync<TReturn>(Expression<Func<T, TReturn>> expression)
        {
            var task = SingleAsync<TReturn>(new FunVisitor<T>().Build(Param, expression, true));
            return task;
        }
        #endregion

        #region Exists
        /// <summary>
        /// 返回满足Where条件的记录个数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            SkipSql.AppendFormat("SELECT COUNT(1) FROM {0}", FromSql);
            if (_where.Length > 0)
            {
                SkipSql.AppendFormat(" WHERE {0}", _where);
            }
            if (_groupBy.Length > 0)
            {
                SkipSql.AppendFormat(" GROUP BY {0}", _groupBy);
            }
            if (_having.Length > 0)
            {
                SkipSql.AppendFormat(" HAVING {0}", _having);
            }
            if (_groupBy.Length > 0)
            {
                SkipSql = new StringBuilder(string.Format("SELECT COUNT(1) FROM ({0}) AS T", SkipSql.ToString()));
            }
            var count = Session.ExecuteScalar<int>(SkipSql.ToString(), Param, CommandType.Text);
            return count;
        }
        /// <summary>
        /// 返回满足Where的记录是否存在
        /// </summary>
        /// <returns></returns>
        public bool Exists()
        {
            SkipSql.AppendFormat("SELECT 1 FROM {0}", FromSql);
            if (_where.Length > 0)
            {
                SkipSql.AppendFormat(" WHERE {0}", _where);
            }
            if (_groupBy.Length > 0)
            {
                SkipSql.AppendFormat(" GROUP BY {0}", _groupBy);
            }
            if (_having.Length > 0)
            {
                SkipSql.AppendFormat(" HAVING {0}", _having);
            }
            string existsSql = string.Format("SELECT EXISTS ({0})", SkipSql);
            var count = Session.ExecuteScalar<int>(existsSql, Param, CommandType.Text);
            return count >= 1;
        }
        #endregion

        #region Insert
        private string InsertBuild()
        {
            var colums = TypeMapper.GetColumnNames<T>();
            var fields = TypeMapper.GetFieldNames<T>();
            InsertSql.AppendFormat("INSERT INTO {0} ({1}) VALUES ({2})", FromSql, string.Join(",", colums), string.Join(",", fields.Select(c => c = '@' + c).ToArray()));
            return InsertSql.ToString();
        }
        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Insert(T entity)
        {
            if (entity == null)
            {
                return 0;
            }
            var sql = InsertBuild();
            var row = Session.Execute(sql, entity, CommandType.Text);
            return row;
        }
        /// <summary>
        /// 异步插入实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> InsertAsync(T entity)
        {
            if (entity == null)
            {
                return Task.Run(() => 0);
            }
            var sql = InsertBuild();
            var task = Session.ExecuteAsync(sql, entity, CommandType.Text);
            return task;
        }
        /// <summary>
        /// 插入实体,并返回identity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int InsertById(T entity)
        {
            if (entity == null)
            {
                return 0;
            }
            var sql = InsertBuild();
            var identity = Session.ExecuteScalar<int>(string.Format("{0};SELECT @@IDENTITY;", sql), entity, CommandType.Text);
            return identity;
        }
        /// <summary>
        /// 异步插入实体,并返回identity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> InsertByIdAsync(T entity)
        {
            if (entity == null)
            {
                return Task.Run(() => 0);
            }
            var sql = InsertBuild();
            var task = Session.ExecuteScalarAsync<int>(string.Format("{0};SELECT @@IDENTITY;", sql), entity, CommandType.Text);
            return task;
        }
        /// <summary>
        /// 批量插入实体
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public int Insert(IEnumerable<T> entitys)
        {
            if (entitys == null || entitys.Count() == 0)
            {
                return 0;
            }
            var sql = InsertBuild();
            var row = Session.Execute(sql, entitys, CommandType.Text);
            return row;
        }
        /// <summary>
        /// 异步插入实体
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public Task<int> InsertAsync(IEnumerable<T> entitys)
        {
            if (entitys == null || entitys.Count() == 0)
            {
                return Task.Run(() => 0);
            }
            var sql = InsertBuild();
            var task = Session.ExecuteAsync(sql, entitys, CommandType.Text);
            return task;
        }
        #endregion

        #region Update
        private string UpdateBuild(bool condition)
        {
            if (_set.Length > 0)
            {
                UpdateSql.AppendFormat("UPDATE {0}", FromSql);
                UpdateSql.AppendFormat(" SET {0}", _set);
            }
            else
            {
                var colums = TypeMapper.GetDbColumn<T>();
                UpdateSql.AppendFormat("UPDATE {0} SET {1}", FromSql, string.Join(",", colums.FindAll(f => f.PrimaryKey == false).Select(s => s.ColumnName + " = @" + s.FieldName)));
            }
            if (condition)
            {
                UpdateSql.AppendFormat(" WHERE {0} = @{0}", TypeMapper.GetIdentityFieldName<T>());
            }
            else if (_where.Length > 0)
            {
                UpdateSql.AppendFormat(" WHERE {0}", _where);
            }
            return UpdateSql.ToString();
        }
        /// <summary>
        /// 执行SET更新
        /// </summary>
        /// <returns></returns>
        public int Update()
        {
            var sql = UpdateBuild(false);
            var row = Session.Execute(sql, Param, CommandType.Text);
            return row;
        }

        /// <summary>
        /// 异步执行Set更新
        /// </summary>
        /// <returns></returns>
        public Task<int> UpdateAsync()
        {
            var sql = UpdateBuild(false);
            var task = Session.ExecuteAsync(sql, Param, CommandType.Text);
            return task;
        }

        /// <summary>
        /// 更新指定实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(T entity)
        {
            if (entity == null)
            {
                return 0;
            }
            var sql = UpdateBuild(true);
            var row = Session.Execute(sql, entity, CommandType.Text);
            return row;
        }
        /// <summary>
        /// 异步更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(T entity)
        {
            if (entity == null)
            {
                return Task.Run(() => 0);
            }
            var sql = UpdateBuild(true);
            var task = Session.ExecuteAsync(sql, entity, CommandType.Text);
            return task;
        }
        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public int Update(IEnumerable<T> entitys)
        {
            if (entitys == null || entitys.Count() == 0)
            {
                return 0;
            }
            var sql = UpdateBuild(true);
            var row = Session.Execute(sql, entitys, CommandType.Text);
            return row;
        }
        /// <summary>
        /// 异步批量更新
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(IEnumerable<T> entitys)
        {
            if (entitys == null || entitys.Count() == 0)
            {
                return Task.Run(() => 0);
            }
            var sql = UpdateBuild(true);
            var task = Session.ExecuteAsync(sql, entitys, CommandType.Text);
            return task;
        }
        #endregion

        #region Delete
        private string DeleteBuild(bool condition)
        {
            DeleteSql.AppendFormat("DELETE FROM {0}", FromSql);
            if (_where.Length > 0)
            {
                DeleteSql.AppendFormat(" WHERE {0}", _where);
            }
            else if (condition)
            {
                DeleteSql.AppendFormat(" WHERE {0} = @{0}", TypeMapper.GetIdentityFieldName<T>());
            }
            return DeleteSql.ToString();
        }
        /// <summary>
        /// 执行Delete
        /// </summary>
        /// <returns></returns>
        public int Delete()
        {
            var sql = DeleteBuild(false);
            var row = Session.Execute(sql, Param, CommandType.Text);
            return row;
        }
        /// <summary>
        /// 异步执行Delete
        /// </summary>
        /// <returns></returns>
        public Task<int> DeleteAsync()
        {
            var sql = DeleteBuild(false);
            var task = Session.ExecuteAsync(sql, Param, CommandType.Text);
            return task;
        }
        #endregion

        #endregion

        #region Distinct
        private string _distinct = null;
        /// <summary>
        /// Distinct
        /// </summary>
        /// <returns></returns>
        public IFrom<T> Distinct()
        {
            _distinct = "DISTINCT";
            return this;
        }
        #endregion

        #region Skip
        private StringBuilder _limit = new StringBuilder();
        /// <summary>
        /// 从index,开始获取count条记录
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IFrom<T> Skip(int index, int count)
        {
            _limit = _limit.AppendFormat("{0},{1}", index, count);
            return this;
        }
        /// <summary>
        /// 获取前count条记录
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IFrom<T> Skip(int count)
        {
            Skip(0, count);
            return this;
        }
        /// <summary>
        /// 分页查询
        /// 从index开始,获取count条记录,并返回满足当前Where的记录数
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IFrom<T> Skip(int index, int count, out int total)
        {
            total = Count();
            Skip(count * (index - 1), count);
            return this;
        }
        #endregion

        #region Where
        private StringBuilder _where = new StringBuilder();
        /// <summary>
        /// Where
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IFrom<T> Where(string where)
        {
            _where.Append(where);

            return this;
        }
        /// <summary>
        /// 条件Where
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public IFrom<T> Where(bool condition, string where)
        {
            _where.Append(where);

            return this;
        }
        /// <summary>
        /// Where
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IFrom<T> Where(WhereQuery<T> query)
        {
            if (query.Count > 0)
            {
                _where.Append(new WhereVisitor<T>().Build(Param, query.Expressions));
            }
            return this;
        }
        /// <summary>
        /// Where
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IFrom<T> Where(Expression<Func<T, bool>> expression)
        {
            Where(new WhereQuery<T>(expression));
            return this;
        }
        /// <summary>
        /// Where
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IFrom<T> Where(bool condition, Expression<Func<T, bool>> expression)
        {
            if (condition)
            {
                Where(expression);
            }
            return this;
        }
        #endregion

        #region Lock
        private string _lock = "";
        /// <summary>
        /// 悲观锁：读锁
        /// </summary>
        /// <returns></returns>
        public IFrom<T> XLock()
        {
            _lock = "FOR UPDATE";
            return this;
        }
        /// <summary>
        /// 共享锁：写锁
        /// </summary>
        /// <returns></returns>
        public IFrom<T> SLock()
        {
            _lock = "LOCK IN SHARE MODE";
            return this;
        }
        #endregion

        #region Set
        private StringBuilder _set = new StringBuilder();
        /// <summary>
        /// Set
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IFrom<T> Set(string column, object value)
        {
            var field = TypeMapper.GetFieldName<T>(column);
            _set.AppendFormat("{0}{1} = @{2}", _set.Length == 0 ? "" : ",", column, TypeMapper.GetFieldName<T>(column));
            Param.Add("@" + field, value);
            return this;
        }
        /// <summary>
        /// 条件Set
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IFrom<T> Set(bool condition, string column, object value)
        {
            if (condition)
            {
                Set(column, value);
            }
            return this;
        }
        /// <summary>
        /// Set
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IFrom<T> Set(Expression<Func<T, object>> column, object value)
        {
            var name = WhereVisitor<T>.GetColumn(column.Body);
            Set(name, value);
            return this;
        }
        /// <summary>
        /// 条件Set
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IFrom<T> Set(bool condition, Expression<Func<T, object>> column, object value)
        {
            if (condition)
            {
                Set(column, value);
            }
            return this;
        }
        /// <summary>
        /// Set
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">表达式</param>
        /// <returns></returns>
        public IFrom<T> Set(Expression<Func<T, object>> column, Expression<Func<T, object>> value)
        {
            _set.AppendFormat("{0}{1} = {2}", _set.Length == 0 ? "" : ",", WhereVisitor<T>.GetColumn(column.Body), new FunVisitor<T>().Build(Param, value, false));
            return this;
        }
        /// <summary>
        /// 条件Set
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="column">字段</param>
        /// <param name="value">表达式</param>
        /// <returns></returns>
        public IFrom<T> Set(bool condition, Expression<Func<T, object>> column, Expression<Func<T, object>> value)
        {
            if (condition)
            {
                Set(column, value);
            }
            return this;
        }
        #endregion

        #region Group By
        private StringBuilder _groupBy = new StringBuilder();
        /// <summary>
        /// GroupBy
        /// </summary>
        /// <param name="groupby"></param>
        /// <returns></returns>
        public IFrom<T> GroupBy(string groupby)
        {
            _groupBy.AppendFormat(groupby);
            return this;
        }
        /// <summary>
        /// GroupBy
        /// </summary>
        /// <param name="groupby"></param>
        /// <returns></returns>
        public IFrom<T> GroupBy(Expression<Func<T, object>> groupby)
        {
            GroupBy(new FunVisitor<T>().Build(Param, groupby, false));
            return this;
        }
        private StringBuilder _having = new StringBuilder();
        /// <summary>
        /// Having
        /// </summary>
        /// <param name="hiving"></param>
        /// <returns></returns>
        public IFrom<T> Having(string hiving)
        {
            _having.Append(hiving);
            return this;
        }
        /// <summary>
        /// 分组筛选
        /// </summary>
        /// <param name="having"></param>
        /// <returns></returns>
        public IFrom<T> Having(WhereQuery<T> having)
        {
            if (having.Count > 0)
            {
                _having.Append(new WhereVisitor<T>().Build(Param, having.Expressions));
            }
            return this;
        }
        /// <summary>
        /// 分组筛选
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IFrom<T> Having(Expression<Func<T, bool>> expression)
        {
            Having(new WhereQuery<T>(expression));
            return this;
        }
        #endregion        

        #region OrderBy
        private StringBuilder _orderBy = new StringBuilder();
        /// <summary>
        /// 降序排序
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public IFrom<T> Desc(Expression<Func<T, object>> orderBy)
        {
            var name = new FunVisitor<T>().Build(Param, orderBy, false);
            OrderBy(string.Format("{0} DESC", name));
            return this;
        }
        /// <summary>
        /// 条件降序
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public IFrom<T> Desc(bool condition, Expression<Func<T, object>> orderBy)
        {
            if (condition)
            {
                Desc(orderBy);
            }
            return this;
        }
        /// <summary>
        /// 升序排序
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public IFrom<T> Asc(Expression<Func<T, object>> orderBy)
        {
            var name = new FunVisitor<T>().Build(Param, orderBy, false);
            OrderBy(string.Format("{0} ASC", name));
            return this;
        }
        /// <summary>
        /// 条件升序
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public IFrom<T> Asc(bool condition, Expression<Func<T, object>> orderBy)
        {
            if (condition)
            {
                Asc(orderBy);
            }
            return this;
        }
        /// <summary>
        /// OrderBy
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public IFrom<T> OrderBy(string orderBy)
        {
            _orderBy.AppendFormat("{0}{1}", _orderBy.Length > 0 ? "," : "", orderBy);
            return this;
        }
        #endregion
    }

}
