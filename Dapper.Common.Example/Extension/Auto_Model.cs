using Dapper.Common;
using Standard.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Standard.Model
{
    public partial class Member
    {
        [Column(isColumn:true)]
        public List<MemberOrder> Orders { get; set; }
    }
}
