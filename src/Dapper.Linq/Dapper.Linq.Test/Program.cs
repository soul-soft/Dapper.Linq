// See https://aka.ms/new-console-template for more information
using Dapper.Linq;
using Dapper.Linq.Test;
using MySqlConnector;

Console.WriteLine("Hello, World!");
var connectionStringBuilder = new MySqlConnectionStringBuilder();
connectionStringBuilder.UserID = "root";
connectionStringBuilder.Server = "127.0.0.1";
connectionStringBuilder.Password = "1024";
connectionStringBuilder.Database = "test";
var connection = new MySqlConnection(connectionStringBuilder.ConnectionString);
var context = new DbContext(connection);
var view = @"
SELECT
    st.*,
    (st.id + 1) AS gs
FROM
    student AS st
";
var list = context.From<Student>()
    .ToList();
Console.WriteLine();

