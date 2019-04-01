using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Common.Example
{
    public partial class Member
    {
        [Column(isColumn:true)]
        public List<MemberOrder> Orders { get; set; }
    }
}
