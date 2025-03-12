using System;

 namespace Kharazmi.MongoDb
{
    public class MongoDbException : Exception, IDisposable
    {
        private bool _isDisposed;
        public MongoDbException()
        {
        }

        public MongoDbException(string message) : base(message)
        {
        }

        public MongoDbException(Exception exception) : base("", exception)
        {
        }

        public MongoDbException(string message, Exception exception) : base(message, exception)
        {
        }

        /// <summary>
        /// Dispose() calls Dispose(true)
        /// </summary>
        public void Dispose()
        {

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  The bulk of the clean-up code is implemented in Dispose(bool)
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
                Clear();

            _isDisposed = true;
        }

        protected virtual void Clear()
        {
        }


        /// <summary>
        /// NOTE: Leave out the finalizer altogether if this class doesn't
        /// own unmanaged resources, but leave the other methods
        /// exactly as they are.
        /// </summary>
        ~MongoDbException()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }
    }
}