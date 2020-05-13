using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper
{
    /// <summary>
    /// 全局设置
    /// </summary>
    public static class GlobalSettings
    {
        /// <summary>
        /// 数据库元信息提供程序
        /// </summary>
        public static IDbMetaInfoProvider DbMetaInfoProvider { get; set; }
            = new AnnotationDbMetaInfoProvider();
        /// <summary>
        /// 实体映射器提供程序
        /// </summary>
        public static IEntityMapperProvider EntityMapperProvider { get; set; }
            = new EntityMapperProvider();
        /// <summary>
        /// xml命令配置
        /// </summary>
        public static IXmlCommandsProvider XmlCommandsProvider { get; set; }
            = new XmlCommandsProvider();
    }
}
