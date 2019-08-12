using Dapper.Linq;
using Dapper.Linq.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NUnitTestProject1
{
    public class Case<T> : ISqlBuilder
    {
        private List<Expression> _whens = new List<Expression>();
        private List<string> _thens = new List<string>();
        string _else = null;
        public string Build(Dictionary<string, object> values, string prefix)
        {
            var sb = new StringBuilder();
            foreach (var item in _whens)
            {
                var express = ExpressionUtil.BuildExpression(item, values, prefix);
                sb.AppendFormat(" WHEN {0} THEN '{1}'", express, _thens[_whens.IndexOf(item)]);
            }
            if (_else != null)
            {
                sb.AppendFormat(" ELSE '{0}'", _else);
            }
            return string.Format("(CASE {0} END)", sb);
        }
        public static implicit operator string(Case<T> d) => string.Empty;
        public Case<T> When(Expression<Func<T, bool>> expression)
        {
            new Dictionary<string, object>();
            _whens.Add(expression);
            return this;
        }
        public Case<T> Then(string value)
        {
            _thens.Add(value);
            return this;
        }
        public Case<T> Else(string value)
        {
            _else = value;
            return this;
        }
    }
}
