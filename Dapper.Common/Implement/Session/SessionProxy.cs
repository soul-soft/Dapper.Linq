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
    public class SessionProxy : ISession
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

        #region SqlFrom
        public IFrom<T> From<T>(ISession session, string from) where T : class, new()
        {
            return Target.From<T>(session, from);
        }
        public IFrom<T> From<T>() where T : class, new()
        {
            return From<T>(this, null);
        }
        public IFrom<dynamic> From(string from)
        {
            return From<dynamic>(this, from);
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
                Watch = watch.ElapsedMilliseconds,
                Text = string.Format("Open(autocommit={0})", autocommit)
            });
        }
        public Task OpenAsync(bool autocommit)
        {
            Task task = null;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            task = Target.OpenAsync(autocommit);
            watch.Stop();
            Loggers.Add(new SessionLogger()
            {
                Time = DateTime.Now,
                Watch = watch.ElapsedMilliseconds,
                Text = string.Format("OpenAsync(autocommit={0})", autocommit)
            });
            return task;
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
                Watch = watch.ElapsedMilliseconds,
                Text = "Commit"
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
                Watch = watch.ElapsedMilliseconds,
                Text = "Rollback"
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
                Watch = watch.ElapsedMilliseconds,
                Text = "Close"
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
                Target.Buffered=value;
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
                Sql = sql,
                Param = param,
                Time = DateTime.Now,
                Text = "Execute",
                Row = row,
                Watch = watch.ElapsedMilliseconds
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
                Sql = sql,
                Param = param,
                Time = DateTime.Now,
                Text = "ExecuteAsync",
                Row = task.Result,
                Watch = watch.ElapsedMilliseconds
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
                Sql = sql,
                Param = param,
                Time = DateTime.Now,
                Text = "ExecuteReader",
                Watch = watch.ElapsedMilliseconds
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
                Sql = sql,
                Param = param,
                Time = DateTime.Now,
                Text = "ExecuteReaderAsync",
                Watch = watch.ElapsedMilliseconds
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
                Sql = sql,
                Param = param,
                Time = DateTime.Now,
                Text = "ExecuteScalar",
                Watch = watch.ElapsedMilliseconds
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
                Sql = sql,
                Param = param,
                Time = DateTime.Now,
                Text = "ExecuteScalar",
                Watch = watch.ElapsedMilliseconds
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
                Sql = sql,
                Param = param,
                Time = DateTime.Now,
                Text = "ExecuteScalarAsync",
                Watch = watch.ElapsedMilliseconds
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
                Sql = sql,
                Param = param,
                Time = DateTime.Now,
                Text = "ExecuteScalarAsync",
                Watch = watch.ElapsedMilliseconds
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
                Sql = sql,
                Param = param,
                Time = DateTime.Now,
                Text = "Query",
                Watch = watch.ElapsedMilliseconds
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
                Sql = sql,
                Param = param,
                Time = DateTime.Now,
                Text = "QueryAsync",
                Watch = watch.ElapsedMilliseconds
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
                Sql = sql,
                Param = param,
                Time = DateTime.Now,
                Text = "Query",
                Watch = watch.ElapsedMilliseconds
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
                Sql = sql,
                Param = param,
                Time = DateTime.Now,
                Text = "QueryAsync",
                Watch = watch.ElapsedMilliseconds
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
                Sql = sql,
                Param = param,
                Time = DateTime.Now,
                Text = "QueryMultiple",
                Watch = watch.ElapsedMilliseconds
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
                Sql = sql,
                Param = param,
                Time = DateTime.Now,
                Text = "QueryMultipleAsync",
                Watch = watch.ElapsedMilliseconds
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
                builder.AppendFormat("\r\nMethod:{0}\r\n", item.Text);
                builder.AppendFormat("Time:{0}\r\n", item.Time.ToString("yyyy-MM-dd HH:mm:ss"));
                builder.AppendFormat("Watch:{0}\r\n", item.Watch);
                if (item.Row!=null)
                {
                    builder.AppendFormat("Change:{0}\r\n", item.Row);
                }
                builder.AppendFormat("Execute:");
                var logger = item.ParamToFormat();
                if (!string.IsNullOrEmpty(logger))
                {
                    builder.AppendFormat("\r\n{0}", logger);
                }
                if (!string.IsNullOrEmpty(item.Sql))
                {
                    builder.AppendFormat("{0};", item.Sql);
                }
                builder.Append("\r\n");
            }
            builder.AppendFormat("\r\n===========================================================================\r\n");
            return builder.ToString();
        }
        #endregion

    }
}
