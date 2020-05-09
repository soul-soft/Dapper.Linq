using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.XmlResovles
{
    internal class WhereNode : INode
    {
        public List<INode> Nodes { get; set; } = new List<INode>();
    }
}
