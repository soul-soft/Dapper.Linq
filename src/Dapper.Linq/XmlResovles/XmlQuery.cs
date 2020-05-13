using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
    /// <summary>
    /// xml命令映射器
    /// </summary>
    public interface IXmlQuery
    {
        /// <summary>
        ///执行多结果集查询，返回IMultiResult
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        IDbMultipleResult MultipleQuery(int? commandTimeout = null, CommandType? commandType = null);
        /// <summary>
        /// 执行单结果集查询，并返回dynamic类型的结果集
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        IEnumerable<dynamic> Query(int? commandTimeout = null, CommandType? commandType = null);
        /// <summary>
        /// 异步执行单结果集查询，并返回dynamic类型的结果集
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<IEnumerable<dynamic>> QueryAsync(int? commandTimeout = null, CommandType? commandType = null);
        /// <summary>
        /// 执行单结果集查询，并返回T类型的结果集
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        IEnumerable<T> Query<T>(int? commandTimeout = null, CommandType? commandType = null);
        /// <summary>
        /// 异步执行单结果集查询，并返回T类型的结果集
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsync<T>(int? commandTimeout = null, CommandType? commandType = null);
        /// <summary>
        /// 执行无结果集查询，并返回受影响的行数
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        int Execute(int? commandTimeout = null, CommandType? commandType = null);
        /// <summary>
        /// 异步执行无结果集查询，并返回受影响的行数
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<int> ExecuteAsync(int? commandTimeout = null, CommandType? commandType = null);
        /// <summary>
        /// 执行无结果集查询，并返回指定类型的数据
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        T ExecuteScalar<T>(int? commandTimeout = null, CommandType? commandType = null);
        /// <summary>
        /// 异步执行无结果集查询，并返回指定类型的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<T> ExecuteScalarAsync<T>(int? commandTimeout = null, CommandType? commandType = null);
        /// <summary>
        /// 添加数据库参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void AddDbParameter(string name, object value);
    }

    /// <summary>
    /// 实现xml命令映射器
    /// </summary>
    internal class XmlQuery : IXmlQuery
    {
        private readonly string _sql = null;

        private Dictionary<string, object> _parameters = null;

        private readonly IDbContext _mapper = null;

        public XmlQuery(IDbContext mapper, string sql, Dictionary<string,object> parameters=null)
        {
            _mapper = mapper;
            _sql = sql;
            _parameters = parameters;
        }

        public IDbMultipleResult MultipleQuery(int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mapper.QueryMultiple(_sql, _parameters, commandTimeout,commandType);
        }

        public int Execute(int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mapper.Execute(_sql, _parameters, commandTimeout, commandType);
        }

        public Task<int> ExecuteAsync(int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mapper.ExecuteAsync(_sql, _parameters, commandTimeout, commandType);
        }

        public IEnumerable<T> Query<T>(int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mapper.Query<T>(_sql, _parameters, commandTimeout, commandType);
        }

        public IEnumerable<dynamic> Query(int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mapper.Query(_sql, _parameters, commandTimeout, commandType);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mapper.QueryAsync<T>(_sql, _parameters, commandTimeout, commandType);
        }

        public Task<IEnumerable<dynamic>> QueryAsync(int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mapper.QueryAsync(_sql, _parameters, commandTimeout, commandType);
        }

        public T ExecuteScalar<T>(int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mapper.ExecuteScalar<T>(_sql, _parameters, commandTimeout, commandType);
        }

        public Task<T> ExecuteScalarAsync<T>(int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mapper.ExecuteScalarAsync<T>(_sql, _parameters, commandTimeout, commandType);
        }

        public void AddDbParameter(string name,object value)
        {
            if (_parameters == null)
            {
                _parameters = new Dictionary<string, object>();
            }
            if (_parameters.ContainsKey(name))
            {
                _parameters[name] = value;
            }
            else
            {
                _parameters.Add(name,value);
            }
        }
    }
}
