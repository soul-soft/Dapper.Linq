# Dapper.Common QQ群：642555086

        
        
    一、Config
            //开启静态代理
            SessionFactory.StaticProxy = true;
            //配置数据源
            SessionFactory.DataSources.Add(new DataSource()
            {
                Source = () => new MySql.Data.MySqlClient.MySqlConnection(connectionString),
                Name="mysql",
                Type=DataSourceType.MYSQL,
            });
