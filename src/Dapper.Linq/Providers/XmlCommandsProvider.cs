using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Dapper.XmlResovles;

namespace Dapper
{
    /// <summary>
    /// xml配置提供程序
    /// </summary>
    public interface IXmlCommandsProvider
    {
        /// <summary>
        /// 解析动态sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        string Build<T>(string id, T parameter) where T : class;
        /// <summary>
        /// 解析sql
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string Build(string id);
        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="filename">文件名</param>
        void Load(string filename);
        /// <summary>
        /// 从指定路径加载所有匹配的文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="pattern">文件通配符</param>
        void Load(string path, string pattern);
        /// <summary>
        /// 从指定路径加载所有匹配的文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="pattern">文件通配符</param>
        /// <param name="options">查找选项</param>
        void Load(string path, string pattern, SearchOption options);
        /// <summary>
        /// 从程序集加载配置所有xml
        /// </summary>
        /// <param name="assembly">程序集</param>
        void Load(System.Reflection.Assembly assembly);
        /// <summary>
        /// 从程序集加载配置
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="pattern">正则匹配</param>
        void Load(System.Reflection.Assembly assembly, string pattern);
    }
    /// <summary>
    /// xml配置提供程序
    /// </summary>
    public class XmlCommandsProvider : IXmlCommandsProvider
    {
        private readonly Dictionary<string, CommandNode> _commands
            = new Dictionary<string, CommandNode>();

        private Dictionary<string, string> ResolveVariables(XmlElement element)
        {
            var variables = new Dictionary<string, string>();
            var elements = element.Cast<XmlNode>()
                .Where(a => a.Name == "var");
            foreach (XmlElement item in elements)
            {
                if (item.Name == "var")
                {
                    var id = item.GetAttribute("id");
                    var value = string.IsNullOrEmpty(item.InnerXml)
                         ? item.GetAttribute("value") : item.InnerXml;
                    variables.Add(id, value);
                }
            }
            return variables;
        }

        private string ReplaceVariable(Dictionary<string, string> variables, string text)
        {
            var matches = Regex.Matches(text, @"\${(?<key>.*?)}");
            foreach (Match item in matches)
            {
                var key = item.Groups["key"].Value;
                if (variables.ContainsKey(key))
                {
                    var value = variables[key];
                    text = text.Replace("${" + key + "}", value);
                }
            }
            return Regex.Replace(text, @"\s+", " ").Trim(' ');
        }

        private CommandNode ResolveCommand(XmlElement element)
        {
            var cmd = new CommandNode();
            foreach (XmlNode item in element.ChildNodes)
            {
                if (item.Name == "var" || item.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                if (item.NodeType == XmlNodeType.Text)
                {
                    cmd.Nodes.Add(new TextNode
                    {
                        Value = item.Value
                    });
                }
                else if (item.NodeType == XmlNodeType.Element && item.Name == "where")
                {
                    var whereNode = new WhereNode();
                    foreach (XmlNode iitem in item.ChildNodes)
                    {
                        if (iitem.NodeType == XmlNodeType.Text)
                        {
                            whereNode.Nodes.Add(new TextNode
                            {
                                Value = iitem.Value
                            });
                        }
                        else if (iitem.NodeType == XmlNodeType.Element && iitem.Name == "if")
                        {
                            var test = iitem.Attributes["test"].Value;
                            var value = string.IsNullOrEmpty(iitem.InnerText) ?
                                (iitem.Attributes["value"]?.Value ?? string.Empty) : iitem.InnerText;
                            whereNode.Nodes.Add(new IfNode
                            {
                                Test = test,
                                Value = value
                            });
                        }
                    }
                    cmd.Nodes.Add(whereNode);
                }
                else if (item.NodeType == XmlNodeType.Element && item.Name == "if")
                {
                    var test = item.Attributes["test"].Value;
                    var value = string.IsNullOrEmpty(item.InnerText) ?
                             (item.Attributes["value"]?.Value ?? string.Empty) : item.InnerText;
                    cmd.Nodes.Add(new IfNode
                    {
                        Test = test,
                        Value = value
                    });
                }
            }
            return cmd;
        }

        public string Build<T>(string id, T parameter) where T : class
        {
            if (!_commands.ContainsKey(id))
            {
                return null;
            }
            var cmd = _commands[id];
            return cmd.Resolve(cmd, parameter);
        }

        public string Build(string id)
        {
            return Build(id, (object)null);
        }
        private void Resolve(XmlDocument document)
        {
            if (document.DocumentElement.Name != "commands")
            {
                return;
            }
            lock (this)
            {
                var @namespace = document.DocumentElement
                    .GetAttribute("namespace") ?? string.Empty;
                //解析全局变量
                var globalVariables = ResolveVariables(document.DocumentElement);
                //获取命令节点
                var elements = document.DocumentElement
                    .Cast<XmlNode>()
                    .Where(a => a.Name != "var" && a is XmlElement);
                foreach (XmlElement item in elements)
                {
                    var id = item.GetAttribute("id");
                    id = string.IsNullOrEmpty(@namespace) ? $"{id}" : $"{@namespace}.{id}";
                    //解析局部变量
                    var localVariables = ResolveVariables(item);
                    //合并局部和全局变量，局部变量可以覆盖全局变量
                    var variables = new Dictionary<string, string>(globalVariables);
                    foreach (var ariable in localVariables)
                    {
                        if (variables.ContainsKey(ariable.Key))
                        {
                            variables[ariable.Key] = ariable.Value;
                        }
                        else
                        {
                            variables.Add(ariable.Key, ariable.Value);
                        }
                    }
                    //替换变量
                    var xml = ReplaceVariable(variables, item.OuterXml);
                    var doc = new XmlDocument();
                    doc.LoadXml(xml);
                    //通过变量解析命令
                    var cmd = ResolveCommand(doc.DocumentElement);
                    if (_commands.ContainsKey(id))
                    {
                        _commands[id] = cmd;
                    }
                    else
                    {
                        _commands.Add(id, cmd);
                    }
                }
            }
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="filename"></param>
        public void Load(string filename)
        {
            var document = new XmlDocument();
            document.Load(filename);
            Resolve(document);
        }

        /// <summary>
        /// 从流中加载xml
        /// </summary>
        /// <param name="stream"></param>
        public void Load(Stream stream)
        {
            XmlDocument document = new XmlDocument();
            document.Load(stream);
            Resolve(document);
        }

        /// <summary>
        /// 从指定路径加载所有匹配的文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="pattern">通配符</param>
        public void Load(string path, string pattern)
        {
            var files = System.IO.Directory.GetFiles(path, pattern);
            foreach (var item in files)
            {
                Load(item);
            }
        }

        /// <summary>
        /// 从指定路径加载所有匹配的文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="pattern">通配符</param>
        /// <param name="options">查找选项</param>
        public void Load(string path, string pattern, SearchOption options)
        {
            var files = System.IO.Directory.GetFiles(path, pattern, options);
            foreach (var item in files)
            {
                Load(item);
            }
        }

        /// <summary>
        /// 从嵌入式资源加载.xml结尾的文件
        /// </summary>
        /// <param name="assembly">程序集</param>
        public void Load(System.Reflection.Assembly assembly)
        {
            var filenames = assembly.GetManifestResourceNames();
            foreach (var item in filenames)
            {
                if (!item.EndsWith(".xml", System.StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                Load(assembly.GetManifestResourceStream(item));
            }

        }

        /// <summary>
        /// 从嵌入式资源加载配置
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="pattern">匹配模式</param>
        public void Load(System.Reflection.Assembly assembly, string pattern)
        {
            var filenames = assembly.GetManifestResourceNames();
            foreach (var item in filenames)
            {
                if (!Regex.IsMatch(item, pattern))
                {
                    continue;
                }
                Load(assembly.GetManifestResourceStream(item));
            }
        }
    }
}
