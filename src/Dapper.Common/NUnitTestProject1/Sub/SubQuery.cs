using Dapper.Common;
using Dapper.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NUnitTestProject1
{
    public class SubQuery<T> : ISubQuery where T : class
    {
        private Expression _where { get; set; }
        private Expression _column { get; set; }
        private string _method { get; set; }
        private bool _useSignTable = true;
        public string Build(Dictionary<string, object> values, string prefix)
        {
            var table = EntityUtil.GetTable<T>();
            var column = ExpressionUtil.BuildColumn(_column, values, prefix).SingleOrDefault().Value;
            var where = ExpressionUtil.BuildExpression(_where, values, prefix, _useSignTable);
            if (_method == nameof(this.Select))
            {
                return string.Format("(select {0} from {1} where {2})", column, table.TableName, where);
            }
            if (_method == nameof(this.Count))
            {
                return string.Format("(select count({0}) from {1} where {2})", column, table.TableName, where);
            }
            throw new NotImplementedException();
        }
        public SubQuery<T> Where(Expression<Func<T, bool>> expression)
        {
            _where = expression;
            return this;
        }
        public SubQuery<T> Where<T1, T2>(Expression<Func<T1, T2, bool>> expression)
        {
            _useSignTable = false;
            _where = expression;
            return this;
        }
        public SubQuery<T> Select<TResut>(Expression<Func<T, TResut>> expression)
        {
            _method = nameof(this.Select);
            _column = expression;
            return this;
        }
        public SubQuery<T> Count<TResut>(Expression<Func<T, TResut>> expression)
        {
            _method = nameof(this.Count);
            _column = expression;
            return this;
        }

        public override bool Equals(object obj)
        {
            return obj is SubQuery<T> query &&
                   EqualityComparer<Expression>.Default.Equals(_where, query._where) &&
                   EqualityComparer<Expression>.Default.Equals(_column, query._column) &&
                   _method == query._method;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_where, _column, _method);
        }

        public static bool operator <(object t1, SubQuery<T> t2)
        {
            return false;
        }
        public static bool operator ==(object t1, SubQuery<T> t2)
        {
            return false;
        }
        public static bool operator !=(object t1, SubQuery<T> t2)
        {
            return false;
        }
        public static bool operator <=(object t1, SubQuery<T> t2)
        {
            return false;
        }
        public static bool operator >=(object t1, SubQuery<T> t2)
        {
            return false;
        }
        public static bool operator >(object t1, SubQuery<T> t2)
        {
            return false;
        }

        public static explicit operator string(SubQuery<T> v)=> string.Empty;
        
    }
}
