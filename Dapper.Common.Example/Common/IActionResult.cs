using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Common.Example
{
    public interface IActionResult
    {
        string ToJson();
        string Message { get; set; }
    }
    public class ActionResult : IActionResult
    {
        public int Total { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }

        public string ToJson()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                Converters =
                {
                    //日期处理
                    new IsoDateTimeConverter()
                     {
                        DateTimeFormat= "yyyy-MM-dd HH:mm:ss"
                     }
                },
                //Camel命名:首字母小写
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            return JsonConvert.SerializeObject(this, serializerSettings);
        }
    }
}
