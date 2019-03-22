using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Common.Example
{
    public partial class Member
    {
        [Column(remove:true)]
        public List<MemberOrder> Orders { get; set; }
    }
}
