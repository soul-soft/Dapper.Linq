using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Common
{
    /// <summary>
    /// 事物会话代理类
    /// </summary>
    internal class SessionProxy : ISession
    {
        #region SessionProxy
        /// <summary>
        /// 目标对象
        /// </summary>
        private ISession Target { get; set; }
        /// <summary>
        /// 会话日志
        /// </summary>
        private List<SessionLogger> Loggers = new List<SessionLogger>();
        /// <summary>
        /// 计时器
        /// </summary>
        private Stopwatch Watch= new Stopwatch();
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="target"></param>
        public SessionProxy(ISession target)
        {
            Target = target;
        }
        #endregion

        #region From
        public IFrom<T> From<T>() where T : class, new()
        {
            return new MysqlFrom<T>()
            {
                Session = this,
            };
        }
        #endregion

        #region ADO.NET
        public void Open(bool autocommit)
        {
            Watch.Reset();
            Watch.Start();
            Target.Open(autocommit);
            Watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Time = DateTime.Now,
                Sec = Watch.ElapsedMilliseconds,
                Method = "Open",
                Command = string.Format("Open Session{0}", !autocommit ? ";Start Transaction" : ""),
            });
        }
        public void Commit()
        {
            Watch.Reset();
            Watch.Start();
            Target.Commit();
            Watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Time = DateTime.Now,
                Sec = Watch.ElapsedMilliseconds,
                Method = "Commit",
                Command = "Commit Transaction"
            });
        }
        public void Rollback()
        {
            Watch.Reset();
            Watch.Start();
            Target.Rollback();
            Watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Time = DateTime.Now,
                Sec = Watch.ElapsedMilliseconds,
                Method = "Rollback;",
                Command = "Rollback Transaction",
            });
        }
        public void Close()
        {
            Watch.Reset();
            Watch.Start();
            Target.Close();
            Watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Time = DateTime.Now,
                Sec = Watch.ElapsedMilliseconds,
                Method = "Close",
                Command = "Close Session;"
            });
        }
        public int Timeout
        {
            get
            {
                return Target.Timeout;
            }
            set
            {
                Target.Timeout = value;
            }
        }
        public bool Buffered
        {
            get
            {
                return Target.Buffered;
            }
            set
            {
                Target.Buffered = value;
            }
        }
        #endregion

        #region Execute
        public int Execute(string sql, object param = null, CommandType text = CommandType.Text)
        {
            var row = 0;
            try
            {
                Watch.Reset();
                Watch.Start();
                row = Target.Execute(sql, param, text);
                Watch.Stop();

            }
            finally
            {
                Loggers.Add(new SessionLogger()
                {
                    Command = sql,
                    Param = param,
                    Time = DateTime.Now,
                    Method = "Execute",
                    Change = row,
                    Sec = Watch.ElapsedMilliseconds
                });
            }
            return row;
        }
        public Task<int> ExecuteAsync(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Task<int> task = null;
            try
            {
                Watch.Reset();
                Watch.Start();
                task = Target.ExecuteAsync(sql, param, text);
                Watch.Stop();
            }
            finally
            {
                Loggers.Add(new SessionLogger()
                {
                    Command = sql,
                    Param = param,
                    Time = DateTime.Now,
                    Method = "ExecuteAsync",
                    Change = task.Result,
                    Sec = Watch.ElapsedMilliseconds
                });
            }
            return task;
        }
        public IDataReader ExecuteReader(string sql, object param = null, CommandType text = CommandType.Text)
        {
            IDataReader reader = null;
            try
            {
                Watch.Reset();
                Watch.Start();
                reader = Target.ExecuteReader(sql, param, text);
                Watch.Stop();
            }
            finally
            {
                Loggers.Add(new SessionLogger()
                {
                    Command = sql,
                    Param = param,
                    Time = DateTime.Now,
                    Method = "ExecuteReader",
                    Sec = Watch.ElapsedMilliseconds
                });
            }
            return reader;
        }
        public Task<IDataReader> ExecuteReaderAsync(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Task<IDataReader> task = null;
            try
            {
                Watch.Reset();
                Watch.Start();
                task = Target.ExecuteReaderAsync(sql, param, text);
                Watch.Stop();
            }
            finally
            {
                Loggers.Add(new SessionLogger()
                {
                    Command = sql,
                    Param = param,
                    Time = DateTime.Now,
                    Method = "ExecuteReaderAsync",
                    Sec = Watch.ElapsedMilliseconds
                });
            }
            return task;
        }
        public T ExecuteScalar<T>(string sql, object param = null, CommandType text = CommandType.Text)
        {
            T obj = default(T);
            try
            {
                Watch.Reset();
                Watch.Start();
                obj = Target.ExecuteScalar<T>(sql, param, text);
                Watch.Stop();
            }
            finally
            {
                Loggers.Add(new SessionLogger()
                {
                    Command = sql,
                    Param = param,
                    Time = DateTime.Now,
                    Method = "ExecuteScalar",
                    Sec = Watch.ElapsedMilliseconds
                });
            }
            return obj;
        }
        public object ExecuteScalar(string sql, object param = null, CommandType text = CommandType.Text)
        {
            object obj = null;
            try
            {
                Watch.Reset();
                Watch.Start();
                obj = Target.ExecuteScalar(sql, param, text);
                Watch.Stop();
            }
            finally
            {
                Loggers.Add(new SessionLogger()
                {
                    Command = sql,
                    Param = param,
                    Time = DateTime.Now,
                    Method = "ExecuteScalar",
                    Sec = Watch.ElapsedMilliseconds
                });
            }
            return obj;
        }
        public Task<object> ExecuteScalarAsync(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Task<object> task = null;
            try
            {
                Watch.Reset();
                Watch.Start();
                task = Target.ExecuteScalarAsync(sql, param, text);
                Watch.Stop();
            }
            finally
            {
                Loggers.Add(new SessionLogger()
                {
                    Command = sql,
                    Param = param,
                    Time = DateTime.Now,
                    Method = "ExecuteScalarAsync",
                    Sec = Watch.ElapsedMilliseconds
                });
            }
            return task;
        }
        public Task<T> ExecuteScalarAsync<T>(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Task<T> task = null;
            try
            {
                Watch.Reset();
                Watch.Start();
                task = Target.ExecuteScalarAsync<T>(sql, param, text);
                Watch.Stop();
            }
            finally
            {
                Loggers.Add(new SessionLogger()
                {
                    Command = sql,
                    Param = param,
                    Time = DateTime.Now,
                    Method = "ExecuteScalarAsync",
                    Sec = Watch.ElapsedMilliseconds
                });
            }
            return task;
        }
        #endregion

        #region Query
        public IEnumerable<T> Query<T>(string sql, object param = null, CommandType text = CommandType.Text)
        {
            IEnumerable<T> list = null;
            try
            {
                Watch.Reset();
                Watch.Start();
                list = Target.Query<T>(sql, param, text);
                Watch.Stop();
            }
            finally
            {
                Loggers.Add(new SessionLogger()
                {
                    Command = sql,
                    Param = param,
                    Time = DateTime.Now,
                    Method = "Query",
                    Sec = Watch.ElapsedMilliseconds
                });
            }
            return list;
        }
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Task<IEnumerable<T>> task = null;
            try
            {
                Watch.Reset();
                Watch.Start();
                task = Target.QueryAsync<T>(sql, param, text);
                Watch.Stop();
            }
            finally
            {
                Loggers.Add(new SessionLogger()
                {
                    Command = sql,
                    Param = param,
                    Time = DateTime.Now,
                    Method = "QueryAsync",
                    Sec = Watch.ElapsedMilliseconds
                });
            }
            return task;
        }
        public IEnumerable<dynamic> Query(string sql, object param = null, CommandType text = CommandType.Text)
        {
            IEnumerable<dynamic> list = null;
            try
            {
                Watch.Reset();
                Watch.Start();
                list = Target.Query(sql, param, text);
                Watch.Stop();
            }
            finally
            {
                Loggers.Add(new SessionLogger()
                {
                    Command = sql,
                    Param = param,
                    Time = DateTime.Now,
                    Method = "Query",
                    Sec = Watch.ElapsedMilliseconds
                });
            }
            return list;
        }
        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, CommandType text = CommandType.Text)
        {
            
            Task<IEnumerable<dynamic>> task = null;
            try
            {
                Watch.Reset();
                Watch.Start();
                task = Target.QueryAsync(sql, param, text);
                Watch.Stop();
            }
            finally
            {
                Loggers.Add(new SessionLogger()
                {
                    Command = sql,
                    Param = param,
                    Time = DateTime.Now,
                    Method = "QueryAsync",
                    Sec = Watch.ElapsedMilliseconds
                });
            }
            return task;
        }
        public SqlMapper.GridReader QueryMultiple(string sql, object param = null, CommandType text = CommandType.Text)
        {
            SqlMapper.GridReader gridReader = null;
            try
            {
                Watch.Reset();
                Watch.Start();
                gridReader = Target.QueryMultiple(sql, param, text);
                Watch.Stop();
            }
            finally
            {
                Loggers.Add(new SessionLogger()
                {
                    Command = sql,
                    Param = param,
                    Time = DateTime.Now,
                    Method = "QueryMultiple",
                    Sec = Watch.ElapsedMilliseconds
                });
            }
            return gridReader;
        }
        public Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Task<SqlMapper.GridReader> task = null;
            try
            {
                Watch.Reset();
                Watch.Start();
                task = Target.QueryMultipleAsync(sql, param, text);
                Watch.Stop();

            }
            finally
            {
                Loggers.Add(new SessionLogger()
                {
                    Command = sql,
                    Param = param,
                    Time = DateTime.Now,
                    Method = "QueryMultipleAsync",
                    Sec = Watch.ElapsedMilliseconds
                });
            }
            return task;
        }
        #endregion

        #region Logger
        public string Logger()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("=========================== {0} ===========================\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            foreach (var item in Loggers)
            {
                builder.AppendFormat("\r\nMethod:{0}\r\n", item.Method);
                builder.AppendFormat("Time:{0}\r\n", item.Time.ToString("yyyy-MM-dd HH:mm:ss"));
                builder.AppendFormat("Sec:{0:0.000}s\r\n", item.Sec/1000.0);
                if (item.Change != null)
                {
                    builder.AppendFormat("Changed:{0}\r\n", item.Change);
                }
                if (!string.IsNullOrEmpty(item.Command))
                {
                    builder.AppendFormat("Command:");
                    builder.AppendFormat("{0};", item.Command);
                }
                builder.Append("\r\n");
            }
            builder.AppendFormat("\r\n===========================================================================\r\n");
            return builder.ToString();
        }
        #endregion

    }
}
