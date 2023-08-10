using System;
using System.Data;

namespace Dapper.Linq
{
    public interface IDbContextTransaction : IDisposable
    {
        IDbTransaction DbTransaction { get; }
        void RollbackTransaction();
        void CommitTransaction();
    }

    internal class DbContextTransaction: IDbContextTransaction
    {
        private Action _callback;
       
        private IDbTransaction _transaction;

        public IDbTransaction DbTransaction => _transaction;

        public DbContextTransaction(IDbTransaction transaction, Action callback)
        {
            _transaction = transaction;
            _callback = callback;

        }

        public void CommitTransaction()
        {
            _transaction?.Commit();
            _transaction?.Dispose();
            _transaction = null;
            _callback?.Invoke();
            _callback = null;
        }

        public void Dispose()
        {
            RollbackTransaction();
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
            _callback?.Invoke();
            _callback = null;
        }
    }
}
