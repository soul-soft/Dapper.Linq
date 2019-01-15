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
    public class MysqlFrom<T> : IFrom<T> where T : class, new()
    {
        #region Construction
        /// <summary>
        /// 创建当前查询
        /// </summary>
        /// <param name="session"></param>
        public MysqlFrom(ISession session, string from = null)
        {
            Session = session;
            FromSql = from == null ? TypeMapper.GetTableName<T>() : from;
        }
        #endregion

        #region Props
        /// <summary>
        /// 会话事物
        /// </summary>
        private ISession Session { get; set; }
        /// <summary>
        /// 查询参数
        /// </summary>
        private DynamicParameters Values = new DynamicParameters();
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
        private StringBuilder TopSql = new StringBuilder();
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
            columns = (_distinct.Length > 0 ? _distinct.ToString() + " " : "") + columns;
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
            if (_lock.Length>0)
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
            var list = Session.Query<T>(sql, Values, CommandType.Text).ToList();
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
            var task = Session.QueryAsync<T>(sql, Values, CommandType.Text);
            return task;
        }
        /// <summary>
        /// 查询数据:指定字段列表
        /// </summary>
        /// <param name="expression">字段列表</param>
        /// <returns></returns>
        public List<T> Select(Expression<Func<T, object>> expression)
        {
            var data = Select(string.Join(",", SqlVisitor.GetColumnNames(expression)));
            return data;
        }
        /// <summary>
        /// 异步查询:指定字段列表
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> SelectAsync(Expression<Func<T, object>> expression)
        {
            var data = SelectAsync(string.Join(",", SqlVisitor.GetColumnNames(expression)));
            return data;
        }
        /// <summary>
        /// 查询单个数据
        /// </summary>
        /// <param name="columns">字段列表</param>
        /// <returns></returns>
        public T Single(string columns = "*")
        {
            Limit(1);
            var sql = SelectBuild(columns);
            var data = Session.Query<T>(sql, Values).SingleOrDefault();
            return data;
        }
        /// <summary>
        /// 异步查询单个数据
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> SingleAsync(string columns = "*")
        {
            Limit(1);
            var sql = SelectBuild(columns);
            var task = Session.QueryAsync<T>(sql, Values);
            return task;
        }
        /// <summary>
        /// 查询单个数据:指定查询列
        /// </summary>
        /// <param name="expression">字段列表</param>
        /// <returns></returns>
        public T Single(Expression<Func<T, object>> expression)
        {
            var data = Single(string.Join(",", SqlVisitor.GetColumnNames<T>(expression)));
            return data;
        }
        /// <summary>
        /// 异步查询单个实体:指定查询列
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> SingleAsync(Expression<Func<T, object>> expression)
        {
            var task = SingleAsync(string.Join(",", SqlVisitor.GetColumnNames<T>(expression)));
            return task;
        }
        #endregion

        #region Exists
        /// <summary>
        /// 判断指定条件的数据格个数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            TopSql.AppendFormat("SELECT COUNT(1) FROM {0}", FromSql);
            if (_where.Length > 0)
            {
                TopSql.AppendFormat(" WHERE {0}", _where);
            }
            if (_groupBy.Length > 0)
            {
                TopSql.AppendFormat(" GROUP BY {0}", _groupBy);
            }
            if (_having.Length > 0)
            {
                TopSql.AppendFormat(" HAVING {0}", _having);
            }
            if (_groupBy.Length > 0)
            {
                TopSql = new StringBuilder(string.Format("SELECT COUNT(1) FROM ({0}) AS T", TopSql.ToString()));
            }
            var count = Session.ExecuteScalar<int>(TopSql.ToString(), Values, CommandType.Text);
            return count;
        }
        /// <summary>
        /// 判断指定条件的数据是否存在
        /// </summary>
        /// <returns></returns>
        public bool Exists()
        {
            TopSql.AppendFormat("SELECT 1 FROM {0}", FromSql);
            if (_where.Length > 0)
            {
                TopSql.AppendFormat(" WHERE {0}", _where);
            }
            if (_groupBy.Length > 0)
            {
                TopSql.AppendFormat(" GROUP BY {0}", _groupBy);
            }
            if (_having.Length > 0)
            {
                TopSql.AppendFormat(" HAVING {0}", _having);
            }
            string existsSql = string.Format("SELECT EXISTS ({0})", TopSql);
            var count = Session.ExecuteScalar<int>(existsSql, Values, CommandType.Text);
            return count >= 1;
        }
        #endregion

        #region Insert
        /// <summary>
        /// 构建插入语句
        /// </summary>
        /// <returns></returns>
        private string InsertBuild()
        {
            var colums = TypeMapper.GetColumnNames<T>();
            var fields = TypeMapper.GetFieldNames<T>();
            InsertSql.AppendFormat("INSERT INTO {0} ({1}) VALUES ({2})", FromSql, string.Join(",", colums), string.Join(",", fields.Select(c => c = '@' + c).ToArray()));
            return InsertSql.ToString();
        }
        /// <summary>
        /// 插入数据
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
            Values.AddDynamicParams(entity);
            var row = Session.Execute(sql, Values, CommandType.Text);
            return row;
        }
        /// <summary>
        /// 异步插入
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
            Values.AddDynamicParams(entity);
            var task = Session.ExecuteAsync(sql, Values, CommandType.Text);
            return task;
        }
        /// <summary>
        /// 插入数据，并更新对象的Idenity属性
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
            Values.AddDynamicParams(entity);
            var identity = Session.ExecuteScalar<int>(string.Format("{0};SELECT @@IDENTITY;", sql), Values, CommandType.Text);
            entity.GetType().GetProperty(TypeMapper.GetIdentityFieldName<T>()).SetValue(entity, identity);
            return identity;
        }
        /// <summary>
        /// 异步插入数据，并更新对象的Idenity属性
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
            Values.AddDynamicParams(entity);
            var task = Session.ExecuteScalarAsync<int>(string.Format("{0};SELECT @@IDENTITY;", sql), Values, CommandType.Text);
            if (task.IsCompleted)
            {
                entity.GetType().GetProperty(TypeMapper.GetIdentityFieldName<T>()).SetValue(entity, task.Result);
            }
            return task;
        }
        /// <summary>
        /// 批量插入
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
            var row = Session.Execute(sql, entitys.Select(s => new DynamicParameters(s)), CommandType.Text);
            return row;
        }
        /// <summary>
        /// 异步插入
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
            Values.AddDynamicParams(entitys);
            var task = Session.ExecuteAsync(sql, entitys.Select(s => new DynamicParameters(s)), CommandType.Text);
            return task;
        }
        #endregion

        #region Update
        /// <summary>
        /// 构建更新语句
        /// </summary>
        /// <returns></returns>
        public string UpdateBuild(bool condition)
        {
            if (_set.Length > 0)
            {
                UpdateSql.AppendFormat("UPDATE {0}", FromSql);
                UpdateSql.AppendFormat(" SET {0}", _set);
            }
            else
            {
                var colums = TypeMapper.GetColumns<T>();
                UpdateSql.AppendFormat("UPDATE {0} SET {1}", FromSql, string.Join(",", colums.Select(s => s.ColumnName + " = @" + s.FieldName)));
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
            var row = Session.Execute(sql, Values, CommandType.Text);
            return row;
        }
        /// <summary>
        /// 异步执行Set更新
        /// </summary>
        /// <returns></returns>
        public Task<int> UpdateAsync()
        {
            var sql = UpdateBuild(false);
            var task = Session.ExecuteAsync(sql, Values, CommandType.Text);
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
            Values.AddDynamicParams(entity);
            var row = Session.Execute(sql, Values, CommandType.Text);
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
            Values.AddDynamicParams(entity);
            var task = Session.ExecuteAsync(sql, Values, CommandType.Text);
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
            var row = Session.Execute(sql, entitys.Select(s => new DynamicParameters(s)), CommandType.Text);
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
            var task = Session.ExecuteAsync(sql, entitys.Select(s => new DynamicParameters(s)), CommandType.Text);
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
                DeleteSql.AppendFormat(" WHERE {0}=@{0}", TypeMapper.GetIdentityFieldName<T>());
            }
            return DeleteSql.ToString();
        }
        /// <summary>
        /// 删除指定条件的数据
        /// </summary>
        /// <returns></returns>
        public int Delete()
        {
            var sql = DeleteBuild(false);
            var row = Session.Execute(sql, Values, CommandType.Text);
            return row;
        }
        /// <summary>
        /// 异步删除
        /// </summary>
        /// <returns></returns>
        public Task<int> DeleteAsync()
        {
            var sql = DeleteBuild(false);
            var task = Session.ExecuteAsync(sql, Values, CommandType.Text);
            return task;
        }
        /// <summary>
        /// 更据对象标识删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Delete(T entity)
        {
            if (entity == null)
            {
                return 0;
            }
            var sql = DeleteBuild(true);
            Values.AddDynamicParams(entity);
            var row = Session.Execute(sql, Values, CommandType.Text);
            return row;
        }
        /// <summary>
        /// 异步删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(T entity)
        {
            if (entity == null)
            {
                return Task.Run(() => 0);
            }
            var sql = DeleteBuild(true);
            Values.AddDynamicParams(entity);
            var task = Session.ExecuteAsync(sql, Values, CommandType.Text);
            return task;
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public int Delete(IEnumerable<T> entitys)
        {
            if (entitys == null || entitys.Count() == 0)
            {
                return 0;
            }
            var sql = DeleteBuild(true);
            var row = Session.Execute(DeleteSql.ToString(), entitys.Select(s => new DynamicParameters(s)), CommandType.Text);
            return row;
        }
        /// <summary>
        /// 异步批量删除
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(IEnumerable<T> entitys)
        {
            if (entitys == null || entitys.Count() == 0)
            {
                return Task.Run(() => 0);
            }
            var sql = DeleteBuild(true);
            var task = Session.ExecuteAsync(DeleteSql.ToString(), entitys.Select(s => new DynamicParameters(s)), CommandType.Text);
            return task;
        }
        #endregion

        #endregion

        #region Distinct
        private StringBuilder _distinct = new StringBuilder();
        public IFrom<T> Distinct()
        {
            _distinct.Append("DISTINCT");
            return this;
        }
        #endregion

        #region Skip
        /// <summary>
        /// Skip
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IFrom<T> Skip(int index, int size, out int total)
        {
            total = Count();
            Limit(size * (index - 1), size);
            return this;
        }
        #endregion

        #region Where
        private StringBuilder _where = new StringBuilder();
        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IFrom<T> Where(string expression)
        {
            _where.Append(expression);

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IFrom<T> Where(bool condition, string expression)
        {
            _where.Append(expression);

            return this;
        }
        /// <summary>
        /// 动态表达式过滤
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IFrom<T> Where(SqlQuery<T> query)
        {
            if (query.Count > 0)
            {
                _where.Append(new SqlVisitor().Build<T>(ref Values, query.Expressions));
            }
            return this;
        }
        /// <summary>
        /// 条件过滤
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IFrom<T> Where(Expression<Func<T, bool>> expression)
        {
            Where(new SqlQuery<T>(expression));
            return this;
        }
        /// <summary>
        /// 条件过滤
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

        #region Add Param
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IFrom<T> Param(object param)
        {
            Values.AddDynamicParams(param);
            return this;
        }
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IFrom<T> Param(bool condition, object param)
        {
            if (condition)
            {
                Param(param);
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
        /// 更新指定字段
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IFrom<T> Set(string column)
        {
            _set.AppendFormat("{0}{1}", _set.Length == 0 ? "" : ",", column);
            return this;
        }
        /// <summary>
        /// 条件更新
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IFrom<T> Set(bool condition, string column)
        {
            if (condition)
            {
                Set(column);
            }
            return this;
        }
        /// <summary>
        /// 更新指定字段
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IFrom<T> Set(Expression<Func<T, object>> expression, object value)
        {
            var column = SqlVisitor.GetColumnName<T>(expression.Body);
            var field = TypeMapper.GetFieldName<T>(column);
            _set.AppendFormat("{0}{1} = @{2}", _set.Length == 0 ? "" : ",", column, TypeMapper.GetFieldName<T>(column));
            Values.Add("@" + field, value);
            return this;
        }
        /// <summary>
        /// 条件更新
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IFrom<T> Set(bool condition, Expression<Func<T, object>> expression, object value)
        {
            if (condition)
            {
                Set(expression, value);
            }
            return this;
        }
        /// <summary>
        /// Lambda Set
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IFrom<T> Set(Expression<Func<T, bool>> expression)
        {
            var setsql = new SqlVisitor().Build<T>(ref Values, new SqlQuery<T>().And(expression).Expressions).Trim(new char[] { '(', ')' }) + ")";
            Set(setsql);
            return this;
        }
        /// <summary>
        /// Condition Lambda Set
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IFrom<T> Set(bool condition, Expression<Func<T, bool>> expression)
        {
            if (condition)
            {
                Set(expression);
            }
            return this;
        }
        #endregion

        #region Group By
        private StringBuilder _groupBy = new StringBuilder();
        public IFrom<T> GroupBy(string groupby)
        {
            _groupBy.AppendFormat(groupby);
            return this;
        }
        /// <summary>
        /// 分组查询
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IFrom<T> GroupBy(Expression<Func<T, object>> expression)
        {
            GroupBy(string.Format("{0}", string.Join(",", SqlVisitor.GetColumnNames(expression))));
            return this;
        }
        private StringBuilder _having = new StringBuilder();
        /// <summary>
        /// 分组筛选
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IFrom<T> Having(string expression)
        {
            _having.Append(expression);
            return this;
        }
        /// <summary>
        /// 分组筛选
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IFrom<T> Having(SqlQuery<T> expression)
        {
            if (expression.Count > 0)
            {
                _having.Append(new SqlVisitor().Build<T>(ref Values, expression.Expressions));
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
            Having(new SqlQuery<T>(expression));
            return this;
        }
        #endregion        

        #region OrderBy
        private StringBuilder _orderBy = new StringBuilder();
        /// <summary>
        /// 倒叙
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public IFrom<T> Desc(Expression<Func<T, object>> orderBy)
        {
            var name = SqlVisitor.GetColumnName<T>(orderBy.Body);
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
        /// 升序
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public IFrom<T> Asc(Expression<Func<T, object>> orderBy)
        {
            var name = SqlVisitor.GetColumnName<T>(orderBy.Body);
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
        /// 自定义
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public IFrom<T> OrderBy(string orderBy)
        {
            _orderBy.AppendFormat("{0}{1} ", _orderBy.Length > 0 ? "," : "", orderBy);
            return this;
        }
        #endregion

        #region Limit
        private StringBuilder _limit = new StringBuilder();
        /// <summary>
        /// Limit查询
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IFrom<T> Limit(int index, int size)
        {
            _limit = _limit.AppendFormat("{0},{1}", index, size);
            return this;
        }
        /// <summary>
        /// Limit查询
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public IFrom<T> Limit(int size)
        {
            Limit(0, size);
            return this;
        }
        #endregion
    }

}
