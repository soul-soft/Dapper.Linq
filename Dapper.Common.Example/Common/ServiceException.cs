using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Common.Example
{
    public class ServiceException:Exception
    {
        public ServiceException(string message):base(message)
        {
        }
    }
    public class ServiceArgumentNullException: ServiceException
    {
        public ServiceArgumentNullException(string name):base(string.Format("参数：{0}不能为空",name))
        {

        }
    }
}
