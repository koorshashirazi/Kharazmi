using System;
using System.Runtime.Serialization;

namespace Kharazmi.AspNetCore.Core.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class FrameworkException : Exception, IDisposable
    {
        private bool isDisposed;

        /// <summary> </summary>
        public string CreateAt { get; }

        /// <summary> </summary>
        public string ExceptionId { get; }

        /// <summary> </summary>
        public string Code { get; protected set; }

        /// <summary> </summary>
        public string Description { get; protected set; }
        
      

        /// <summary>
        /// 
        /// </summary>
        public FrameworkException()
            : this("")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public FrameworkException(string message, Exception innerException)
            : this(message, innerException, "", "")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="context"></param>
        protected FrameworkException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="description"></param>
        /// <param name="code"></param>
        /// <param name="errors"></param>
        public FrameworkException(string message, Exception innerException = null, string description = "",
            string code = ""): base(message, innerException)
        {
            Code = code;
            Description = description;;
            CreateAt = DateTime.Now.ToString("g");
            ExceptionId = Guid.NewGuid().ToString("N");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public FrameworkException WithDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public FrameworkException WithCode(string code)
        {
            Code = code;
            return this;
        }

       

        /// <summary>
        /// free managed resources
        /// </summary>
        public virtual void Clear()
        {
        }

        /// <summary>
        /// free native resources if there are any.
        /// </summary>
        public virtual void NativeResourceClear()
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
            if (isDisposed) return;

            if (disposing)
                Clear();

            NativeResourceClear();

            isDisposed = true;
        }


        /// <summary>
        /// NOTE: Leave out the finalizer altogether if this class doesn't
        /// own unmanaged resources, but leave the other methods
        /// exactly as they are.
        /// </summary>
        ~FrameworkException()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }
    }
}