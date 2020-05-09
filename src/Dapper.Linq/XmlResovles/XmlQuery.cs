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
        IMultiResult ExecuteMultiQuery(int? commandTimeout = null, CommandType? commandType = null);
        /// <summary>
        /// 执行单结果集查询，并返回dynamic类型的结果集
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        IEnumerable<dynamic> ExecuteQuery(int? commandTimeout = null, CommandType? commandType = null);
        /// <summary>
        /// 异步执行单结果集查询，并返回dynamic类型的结果集
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<IEnumerable<dynamic>> ExecuteQueryAsync(int? commandTimeout = null, CommandType? commandType = null);
        /// <summary>
        /// 执行单结果集查询，并返回T类型的结果集
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        IEnumerable<T> ExecuteQuery<T>(int? commandTimeout = null, CommandType? commandType = null);
        /// <summary>
        /// 异步执行单结果集查询，并返回T类型的结果集
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<IEnumerable<T>> ExecuteQueryAsync<T>(int? commandTimeout = null, CommandType? commandType = null);
        /// <summary>
        /// 执行无结果集查询，并返回受影响的行数
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        int ExecuteNonQuery(int? commandTimeout = null, CommandType? commandType = null);
        /// <summary>
        /// 异步执行无结果集查询，并返回受影响的行数
        /// </summary>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        Task<int> ExecuteNonQueryAsync(int? commandTimeout = null, CommandType? commandType = null);
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

        public IMultiResult ExecuteMultiQuery(int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mapper.ExecuteMultiQuery(_sql, _parameters, commandTimeout,commandType);
        }

        public int ExecuteNonQuery(int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mapper.ExecuteNonQuery(_sql, _parameters, commandTimeout, commandType);
        }

        public Task<int> ExecuteNonQueryAsync(int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mapper.ExecuteNonQueryAsync(_sql, _parameters, commandTimeout, commandType);
        }

        public IEnumerable<T> ExecuteQuery<T>(int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mapper.ExecuteQuery<T>(_sql, _parameters, commandTimeout, commandType);
        }

        public IEnumerable<dynamic> ExecuteQuery(int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mapper.ExecuteQuery(_sql, _parameters, commandTimeout, commandType);
        }

        public Task<IEnumerable<T>> ExecuteQueryAsync<T>(int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mapper.ExecuteQueryAsync<T>(_sql, _parameters, commandTimeout, commandType);
        }

        public Task<IEnumerable<dynamic>> ExecuteQueryAsync(int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mapper.ExecuteQueryAsync(_sql, _parameters, commandTimeout, commandType);
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
