using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper
{
    public interface IMultiResult : IDisposable
    {
        /// <summary>
        /// 返回当前dynamic类型结果集
        /// </summary>
        /// <returns></returns>
        List<dynamic> GetList();
        /// <summary>
        /// 异步返回当前dynamic类型结果集
        /// </summary>
        /// <returns></returns>
        Task<List<dynamic>> GetListAsync();
        /// <summary>
        /// 返回当前T结果集
        /// </summary>
        /// <typeparam name="T">结果集类型</typeparam>
        /// <returns></returns>
        List<T> GetList<T>();
        /// <summary>
        ///  异步返回当前T类型结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<List<T>> GetListAsync<T>();
        /// <summary>
        /// 返回当前dynamic类型结果
        /// </summary>
        /// <returns></returns>
        object Get();
        /// <summary>
        /// 异步返回当前dynamic类型结果
        /// </summary>
        /// <returns></returns>
        Task<object> GetAsync();
        /// <summary>
        /// 返回当前T类型结果
        /// </summary>
        /// <typeparam name="T">结果集类型</typeparam>
        /// <returns></returns>
        T Get<T>();
        /// <summary>
        /// 异步返回当前T类型结果
        /// </summary>
        /// <typeparam name="T">结果集类型</typeparam>
        /// <returns></returns>
        Task<T> GetAsync<T>();
    }

    public class MultiResult : IMultiResult
    {
        private readonly IDataReader _reader = null;
        
        private readonly IDbCommand _command = null;

        internal MultiResult(IDbCommand command)
        {
            _command = command;
            _reader = command.ExecuteReader();
        }

        public void Dispose()
        {
            _reader?.Dispose();
            _command?.Dispose();
        }

        public T Get<T>()
        {
            return GetList<T>().FirstOrDefault();
        }

        public async Task<T> GetAsync<T>()
        {
            return (await GetListAsync<T>()).FirstOrDefault();
        }

        public object Get()
        {
            return GetList<object>().FirstOrDefault();
        }
       
        public async Task<object> GetAsync()
        {
            return (await GetListAsync<object>()).FirstOrDefault();
        }
      
        public async Task<List<dynamic>> GetListAsync()
        {
            var handler = EmitConvert.GetSerializer();
            var list = new List<dynamic>();
            while (await (_reader as DbDataReader).ReadAsync())
            {
                list.Add(handler(_reader));
            }
            NextResult();
            return list;
        }
      
        public List<dynamic> GetList()
        {
            var handler = EmitConvert.GetSerializer();
            var list = new List<dynamic>();
            while (_reader.Read())
            {
                list.Add(handler(_reader));
            }
            NextResult();
            return list;
        }
      
        public List<T> GetList<T>()
        {
            var handler = EmitConvert.GetSerializer<T>(GlobalSettings.EntityMapperProvider, _reader);
            var list = new List<T>();
            while (_reader.Read())
            {
                list.Add(handler(_reader));
            }
            NextResult();
            return list;
        }

        public async Task<List<T>> GetListAsync<T>()
        {
            var handler = EmitConvert.GetSerializer<T>(GlobalSettings.EntityMapperProvider, _reader);
            var list = new List<T>();
            while (await (_reader as DbDataReader).ReadAsync())
            {
                list.Add(handler(_reader));
            }
            NextResult();
            return list;
        }

        public void NextResult()
        {
            if (!_reader.NextResult())
            {
                Dispose();
            }
        }
    }
}
