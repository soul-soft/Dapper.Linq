using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Dapper.Expressions;

namespace Dapper.XmlResovles
{
    internal class CommandNode : INode
    {
        public List<INode> Nodes { get; set; } = new List<INode>();

        private string ResolveTextNode(TextNode node)
        {
            return node.Value;
        }

        private string ResolveIfNode<T>(IfNode node, T parameter)
        {
            if (node.Delegate == null)
            {
                lock (this)
                {
                    var context = new ExpressionActivator();
                    var result = context.Create<T>(node.Test);
                    node.Delegate = result.Func;
                }
            }
            var func = node.Delegate as Func<T, bool>;
            if (func(parameter))
            {
                return ResolveTextNode(new TextNode { Value = node.Value });
            }
            return string.Empty;
        }

        private string ResolveWhereNode<T>(WhereNode node, T parameter) where T :class
        {
            var buffer = new StringBuilder();
            foreach (var item in node.Nodes)
            {
                if (parameter!=default && item is IfNode)
                {
                    var text = ResolveIfNode(item as IfNode, parameter);
                    buffer.Append($"{text} ");
                }
                else if (item is TextNode)
                {
                    var text = ResolveTextNode(item as TextNode);
                    buffer.Append($"{text} ");
                }
            }
            var sql = buffer.ToString().Trim(' ');
            if (sql.StartsWith("and", StringComparison.OrdinalIgnoreCase))
            {
                sql = sql.Remove(0, 3);
            }
            else if (sql.StartsWith("or", StringComparison.OrdinalIgnoreCase))
            {
                sql = sql.Remove(0, 2);
            }
            return sql.Length > 0 ? "WHERE " + sql : string.Empty;
        }

        public string Resolve<T>(CommandNode command, T parameter) where T : class
        {
            var buffer = new StringBuilder();
            foreach (var item in command.Nodes)
            {
                if (item is TextNode)
                {
                    var txt = ResolveTextNode(item as TextNode);
                    if (txt.Length > 0)
                    {
                        buffer.AppendFormat($" {txt}");
                    }
                }
                else if (item is WhereNode)
                {
                    var wheresql = ResolveWhereNode(item as WhereNode, parameter);
                    if (wheresql.Length > 0)
                    {
                        buffer.AppendFormat($" {wheresql}");
                    }
                }
                else if (item is IfNode)
                {
                    var txt = ResolveIfNode(item as IfNode, parameter);
                    if (txt.Length > 0)
                    {
                        buffer.AppendFormat($" {txt}");
                    }
                }
            }
            return buffer.ToString().Trim(' ');
        }

    }
}
