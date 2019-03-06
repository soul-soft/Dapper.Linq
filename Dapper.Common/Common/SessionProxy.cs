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
        private List<SessionLogger> Loggers { get; set; }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="target"></param>
        public SessionProxy(ISession target)
        {
            Target = target;
            Loggers = new List<SessionLogger>();
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
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Target.Open(autocommit);
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Time = DateTime.Now,
                Sec = watch.ElapsedMilliseconds,
                Method = "Open",
                Command= string.Format("Open Session{0}", !autocommit ? ";Start Transaction" : ""),
            });
        }
        public void Commit()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Target.Commit();
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Time = DateTime.Now,
                Sec = watch.ElapsedMilliseconds,
                Method = "Commit",
                Command= "Commit Transaction"
            });
        }
        public void Rollback()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Target.Rollback();
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Time = DateTime.Now,
                Sec = watch.ElapsedMilliseconds,
                Method = "Rollback;",
                Command= "Rollback Transaction",
            });
        }
        public void Close()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Target.Close();
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Time = DateTime.Now,
                Sec = watch.ElapsedMilliseconds,
                Method = "Close",
                Command= "Close Session;"
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
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var row = Target.Execute(sql, param, text);
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Command = sql,
                Param = param,
                Time = DateTime.Now,
                Method = "Execute",
                ChangeRow = row,
                Sec = watch.ElapsedMilliseconds
            });
            return row;
        }
        public Task<int> ExecuteAsync(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var task = Target.ExecuteAsync(sql, param, text);
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Command = sql,
                Param = param,
                Time = DateTime.Now,
                Method = "ExecuteAsync",
                ChangeRow = task.Result,
                Sec = watch.ElapsedMilliseconds
            });
            return task;
        }
        public IDataReader ExecuteReader(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var datareader = Target.ExecuteReader(sql, param, text);
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Command = sql,
                Param = param,
                Time = DateTime.Now,
                Method = "ExecuteReader",
                Sec = watch.ElapsedMilliseconds
            });
            return datareader;
        }
        public Task<IDataReader> ExecuteReaderAsync(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var task = Target.ExecuteReaderAsync(sql, param, text);
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Command = sql,
                Param = param,
                Time = DateTime.Now,
                Method = "ExecuteReaderAsync",
                Sec = watch.ElapsedMilliseconds
            });
            return task;
        }
        public T ExecuteScalar<T>(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var obj = Target.ExecuteScalar<T>(sql, param, text);
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Command = sql,
                Param = param,
                Time = DateTime.Now,
                Method = "ExecuteScalar",
                Sec = watch.ElapsedMilliseconds
            });
            return obj;
        }
        public object ExecuteScalar(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var obj = Target.ExecuteScalar(sql, param, text);
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Command = sql,
                Param = param,
                Time = DateTime.Now,
                Method = "ExecuteScalar",
                Sec = watch.ElapsedMilliseconds
            });
            return obj;
        }
        public Task<object> ExecuteScalarAsync(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var task = Target.ExecuteScalarAsync(sql, param, text);
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Command = sql,
                Param = param,
                Time = DateTime.Now,
                Method = "ExecuteScalarAsync",
                Sec = watch.ElapsedMilliseconds
            });
            return task;
        }
        public Task<T> ExecuteScalarAsync<T>(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var task = Target.ExecuteScalarAsync<T>(sql, param, text);
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Command = sql,
                Param = param,
                Time = DateTime.Now,
                Method = "ExecuteScalarAsync",
                Sec = watch.ElapsedMilliseconds
            });
            return task;
        }
        #endregion

        #region Query
        public IEnumerable<T> Query<T>(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var list = Target.Query<T>(sql, param, text);
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Command = sql,
                Param = param,
                Time = DateTime.Now,
                Method = "Query",
                Sec = watch.ElapsedMilliseconds
            });
            return list;
        }
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var list = Target.QueryAsync<T>(sql, param, text);
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Command = sql,
                Param = param,
                Time = DateTime.Now,
                Method = "QueryAsync",
                Sec = watch.ElapsedMilliseconds
            });
            return list;
        }
        public IEnumerable<dynamic> Query(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var list = Target.Query(sql, param, text);
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Command = sql,
                Param = param,
                Time = DateTime.Now,
                Method = "Query",
                Sec = watch.ElapsedMilliseconds
            });
            return list;
        }
        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var list = Target.QueryAsync(sql, param, text);
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Command = sql,
                Param = param,
                Time = DateTime.Now,
                Method = "QueryAsync",
                Sec = watch.ElapsedMilliseconds
            });
            return list;
        }
        public SqlMapper.GridReader QueryMultiple(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var list = Target.QueryMultiple(sql, param, text);
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Command = sql,
                Param = param,
                Time = DateTime.Now,
                Method = "QueryMultiple",
                Sec = watch.ElapsedMilliseconds
            });
            return list;
        }
        public Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object param = null, CommandType text = CommandType.Text)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var list = Target.QueryMultipleAsync(sql, param, text);
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Command = sql,
                Param = param,
                Time = DateTime.Now,
                Method = "QueryMultipleAsync",
                Sec = watch.ElapsedMilliseconds
            });
            return list;
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
                builder.AppendFormat("Sec:{0}\r\n", item.Sec);
                if (item.ChangeRow != null)
                {
                    builder.AppendFormat("Changed:{0}\r\n", item.ChangeRow);
                }
                builder.AppendFormat("Command:");
                var logger = item.ParamToFormat();
                if (!string.IsNullOrEmpty(logger))
                {
                    builder.AppendFormat("\r\n{0}", logger);
                }
                if (!string.IsNullOrEmpty(item.Command))
                {
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
