using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dapper.Expressions;

namespace Dapper
{
    public partial class DbQuery<T> : IDbQuery<T>
    {
        #region fields

        private readonly Dictionary<string, object> _parameters
            = new Dictionary<string, object>();

        private readonly PageData _page = new PageData();

        private string _lockname = string.Empty;

        private readonly IDbContext _context = null;

        private readonly List<Expression> _whereExpressions = new List<Expression>();

        private readonly List<SetExpression> _setExpressions = new List<SetExpression>();

        private readonly List<OrderExpression> _orderExpressions = new List<OrderExpression>();

        private readonly List<Expression> _groupExpressions = new List<Expression>();

        private readonly List<Expression> _havingExpressions = new List<Expression>();

        private Expression _filterExpression = null;

        private Expression _selectExpression = null;

        private Expression _countExpression = null;

        public DbQuery(IDbContext context)
        {
            _context = context;
        }

        #endregion

        #region resovles
        private void ResovleParameter(T entity)
        {
            var serializer = GlobalSettings.EntityMapperProvider.GetDeserializer(typeof(T));
            var values = serializer(entity);
            foreach (var item in values)
            {
                if (_parameters.ContainsKey(item.Key))
                {
                    _parameters[item.Key] = item.Value;
                }
                else
                {
                    _parameters.Add(item.Key, item.Value);
                }
            }
        }

        private string ResovleCount()
        {
            var table = GetTableMetaInfo().TableName;
            var column = "COUNT(1)";
            var where = ResolveWhere();
            var group = ResolveGroup();
            if (group.Length > 0)
            {
                column = group.Remove(0, 10);
            }
            else if (_countExpression != null)
            {
                column = new SelectExpressionResovle(_countExpression).Resovle();
            }
            var sql = $"SELECT {column} FROM {table}{where}{group}";
            if (group.Length > 0)
            {
                sql = $"SELECT COUNT(1) FROM ({sql}) as t";
                return sql;
            }
            return sql;
        }

        private string ResovleSum()
        {
            var table = GetTableMetaInfo().TableName;
            var column = $"SUM({ResovleColumns()})";
            var where = ResolveWhere();
            var sql = $"SELECT {column} FROM {table}{where}";
            return sql;
        }

        private string ResolveGet()
        {
            var table = GetTableMetaInfo().TableName;
            var columns = GetColumnMetaInfos();
            var column = ResovleColumns();
            var where = $" WHERE {columns.Where(a => a.IsPrimaryKey == true).First().ColumnName}=@id";
            string sql;
            if (_context.DbContextType == DbContextType.SqlServer)
            {
                sql = $"SELECT TOP 1 {column} FROM {table}{where}";
            }
            else
            {
                sql = $"SELECT {column} FROM {table}{where} LIMIT 0,1";
            }
            return sql;
        }

        private string ResolveSelect()
        {
            var table = GetTableMetaInfo().TableName;
            var column = ResovleColumns();
            var where = ResolveWhere();
            var group = ResolveGroup();
            var having = ResolveHaving();
            var order = ResolveOrder();
            string sql;
            if (_context.DbContextType == DbContextType.SqlServer)
            {
                if (_lockname != string.Empty)
                {
                    _lockname = $" WITH({_lockname})";
                }
                if (_page.Index == 0)
                {
                    sql = $"SELECT TOP {_page.Count} {column} FROM {table}{_lockname}{where}{group}{having}{order}";
                }
                else if (_page.Index > 0)
                {
                    if (order == string.Empty)
                    {
                        order = " ORDER BY (SELECT 1)";
                    }
                    var limit = $" OFFSET {_page.Index} ROWS FETCH NEXT {_page.Count} ROWS ONLY";
                    sql = $"SELECT {column} FROM {_lockname}{table}{where}{group}{having}{order}{limit}";
                }
                else
                {
                    sql = $"SELECT {column} FROM {_lockname}{table}{where}{group}{having}{order}";
                }
            }
            else
            {
                var limit = _page.Index > 0 || _page.Count > 0 ? $" LIMIT {_page.Index},{_page.Count}" : string.Empty;
                sql = $"SELECT {column} FROM {table}{where}{group}{having}{order}{limit}{_lockname}";
            }
            return sql;
        }

        private string ResovleInsert(bool identity)
        {
            var table = GetTableMetaInfo().TableName;
            var filters = new GroupExpressionResovle(_filterExpression).Resovle().Split(',');
            var columns = GetColumnMetaInfos();
            var intcolumns = columns
                .Where(a => !filters.Contains(a.ColumnName) && !a.IsNotMapped && !a.IsIdentity)
                .Where(a => !a.IsComplexType)
                .Where(a => !a.IsDefault || (_parameters.ContainsKey(a.CsharpName) && _parameters[a.CsharpName] != null));//如果是默认字段
            var columnNames = string.Join(",", intcolumns.Select(s => s.ColumnName));
            var parameterNames = string.Join(",", intcolumns.Select(s => $"@{s.CsharpName}"));
            var sql = $"INSERT INTO {table}({columnNames}) VALUES ({parameterNames})";
            if (identity)
            {
                sql = $"{sql};SELECT @@IDENTITY";
            }
            return sql;
        }

        private DbTableMetaInfo GetTableMetaInfo()
        {
            return GlobalSettings.DbMetaInfoProvider.GetTable(typeof(T));
        }
        private List<DbColumnMetaInfo> GetColumnMetaInfos()
        {
            return GlobalSettings.DbMetaInfoProvider.GetColumns(typeof(T));
        }
        private string ResovleBatchInsert(IEnumerable<T> entitys)
        {
            var table = GetTableMetaInfo().TableName;
            var filters = new GroupExpressionResovle(_filterExpression).Resovle().Split(',');
            var columns = GetColumnMetaInfos()
                .Where(a => !a.IsComplexType).ToList();
            var intcolumns = columns
                .Where(a => !filters.Contains(a.ColumnName) && !a.IsNotMapped && !a.IsIdentity)
                .ToList();
            var columnNames = string.Join(",", intcolumns.Select(s => s.ColumnName));
            if (_context.DbContextType == DbContextType.Mysql)
            {
                var buffer = new StringBuilder();
                buffer.Append($"INSERT INTO {table}({columnNames}) VALUES ");
                var serializer = GlobalSettings.EntityMapperProvider.GetDeserializer(typeof(T));
                var list = entitys.ToList();
                for (var i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var values = serializer(item);
                    buffer.Append("(");
                    for (var j = 0; j < intcolumns.Count; j++)
                    {
                        var column = intcolumns[j];
                        var value = values[column.CsharpName];
                        if (value == null)
                        {
                            buffer.Append(column.IsDefault ? "DEFAULT" : "NULL");
                        }
                        else if (column.CsharpType == typeof(bool) || column.CsharpType == typeof(bool?))
                        {
                            buffer.Append(Convert.ToBoolean(value) == true ? 1 : 0);
                        }
                        else if (column.CsharpType == typeof(DateTime) || column.CsharpType == typeof(DateTime?))
                        {
                            buffer.Append($"'{value}'");
                        }
                        else if (column.CsharpType.IsValueType || (Nullable.GetUnderlyingType(column.CsharpType)?.IsValueType == true))
                        {
                            buffer.Append(value);
                        }
                        else
                        {
                            var str = SqlEncoding(value.ToString());
                            buffer.Append($"'{str}'");
                        }
                        if (j + 1 < intcolumns.Count)
                        {
                            buffer.Append(",");
                        }
                    }
                    buffer.Append(")");
                    if (i + 1 < list.Count)
                    {
                        buffer.Append(",");
                    }
                }
                return buffer.Remove(buffer.Length - 1, 0).ToString();
            }
            throw new NotImplementedException();
        }

        private string ResolveUpdate()
        {
            var table = GetTableMetaInfo().TableName;
            var builder = new StringBuilder();
            if (_setExpressions.Count > 0)
            {
                var where = ResolveWhere();
                foreach (var item in _setExpressions)
                {
                    var column = new BooleanExpressionResovle(item.Column).Resovle();
                    var expression = new BooleanExpressionResovle(item.Expression, _parameters).Resovle();
                    builder.Append($"{column} = {expression},");
                }
                var sql = $"UPDATE {table} SET {builder.ToString().Trim(',')}{where}";
                return sql;
            }
            else
            {
                var filters = new GroupExpressionResovle(_filterExpression).Resovle().Split(',');
                var where = ResolveWhere();
                var columns = GetColumnMetaInfos();
                var updcolumns = columns
                    .Where(a => !filters.Contains(a.ColumnName))
                    .Where(a => !a.IsComplexType)
                    .Where(a => !a.IsIdentity && !a.IsPrimaryKey && !a.IsNotMapped)
                    .Where(a => !a.IsConcurrencyCheck)
                    .Select(s => $"{s.ColumnName} = @{s.CsharpName}");
                if (string.IsNullOrEmpty(where))
                {
                    var primaryKey = columns.Where(a => a.IsPrimaryKey).FirstOrDefault()
                        ?? columns.First();
                    where = $" WHERE {primaryKey.ColumnName} = @{primaryKey.CsharpName}";
                    if (columns.Exists(a => a.IsConcurrencyCheck))
                    {
                        var checkColumn = columns.Where(a => a.IsConcurrencyCheck).FirstOrDefault();
                        where += $" AND {checkColumn.ColumnName} = @{checkColumn.CsharpName}";
                    }
                }
                var sql = $"UPDATE {table} SET {string.Join(",", updcolumns)}";
                if (columns.Exists(a => a.IsConcurrencyCheck))
                {
                    var checkColumn = columns.Where(a => a.IsConcurrencyCheck).FirstOrDefault();
                    sql += $",{checkColumn.ColumnName} = @New{checkColumn.CsharpName}";
                    if (checkColumn.CsharpType.IsValueType)
                    {
                        var version = Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
                        _parameters.Add($"New{checkColumn.CsharpName}", version);
                    }
                    else
                    {
                        var version = Guid.NewGuid().ToString("N");
                        _parameters.Add($"New{checkColumn.CsharpName}", version);
                    }
                }
                sql += where;
                return sql;
            }
        }

        private string ResovleDelete()
        {
            var table = GetTableMetaInfo().TableName;
            var where = ResolveWhere();
            var sql = $"DELETE FROM {table}{where}";
            return sql;
        }

        private string SqlEncoding(string sql)
        {
            var buffer = new StringBuilder();
            for (int i = 0; i < sql.Length; i++)
            {
                var ch = sql[i];
                if (ch == '\'' || ch == '-' || ch == '\\' || ch == '*' || ch == '@')
                {
                    buffer.Append('\\');
                }
                buffer.Append(ch);
            }
            return buffer.ToString();
        }

        private string ResovleExists()
        {
            var table = GetTableMetaInfo().TableName;
            var where = ResolveWhere();
            var group = ResolveGroup();
            var having = ResolveHaving();
            var sql = $"SELECT 1 WHERE EXISTS(SELECT 1 FROM {table}{where}{group}{having})";
            return sql;
        }

        private string ResovleColumns()
        {
            if (_selectExpression == null)
            {
                var filters = new GroupExpressionResovle(_filterExpression)
                    .Resovle().Split(',');
                var columns = GetColumnMetaInfos()
                    .Where(a => !filters.Contains(a.ColumnName) && !a.IsNotMapped)
                    .Select(s => s.ColumnName != s.CsharpName ? $"{s.ColumnName} AS {s.CsharpName}" : s.CsharpName);
                return string.Join(",", columns);
            }
            else
            {
                return new SelectExpressionResovle(_selectExpression).Resovle();
            }
        }

        private string ResolveWhere()
        {
            var builder = new StringBuilder();
            foreach (var expression in _whereExpressions)
            {
                var result = new BooleanExpressionResovle(expression, _parameters).Resovle();
                if (expression == _whereExpressions.First())
                {
                    builder.Append($" WHERE {result}");
                }
                else
                {
                    builder.Append($" AND {result}");
                }
            }
            return builder.ToString();
        }

        private string ResolveGroup()
        {
            var buffer = new StringBuilder();
            foreach (var item in _groupExpressions)
            {
                var result = new GroupExpressionResovle(item).Resovle();
                buffer.Append($"{result},");
            }
            var sql = string.Empty;
            if (buffer.Length > 0)
            {
                buffer.Remove(buffer.Length - 1, 1);
                sql = $" GROUP BY {buffer}";
            }
            return sql;
        }

        private string ResolveHaving()
        {
            var buffer = new StringBuilder();
            foreach (var item in _havingExpressions)
            {
                var result = new BooleanExpressionResovle(item, _parameters).Resovle();
                if (item == _havingExpressions.First())
                {
                    buffer.Append($" HAVING {result}");
                }
                else
                {
                    buffer.Append($" AND {result}");
                }
            }
            return buffer.ToString();
        }

        private string ResolveOrder()
        {
            var buffer = new StringBuilder();
            foreach (var item in _orderExpressions)
            {
                if (item == _orderExpressions.First())
                {
                    buffer.Append($" ORDER BY ");
                }
                var result = new OrderExpressionResovle(item.Expression, item.Asc).Resovle();
                buffer.Append(result);
                buffer.Append(",");
            }
            return buffer.ToString().Trim(',');
        }

        class PageData
        {
            public int Index { get; set; } = -1;
            public int Count { get; set; }
        }

        class OrderExpression
        {
            public bool Asc { get; set; } = true;
            public Expression Expression { get; set; }
        }

        class SetExpression
        {
            public Expression Column { get; set; }
            public Expression Expression { get; set; }
        }
        #endregion
    }
}
