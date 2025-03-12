﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kharazmi.AspNetCore.Cache.Ef
{
    /// <summary>
    /// Asynchronous version of the IEnumerable interface, allowing elements of the enumerable sequence to be retrieved asynchronously.
    /// </summary>
    public class EFAsyncTaskEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly Task<T> _task;

        /// <summary>
        /// Asynchronous version of the IEnumerable interface.
        /// </summary>
        public EFAsyncTaskEnumerable(Task<T> task)
        {
            _task = task;
        }

        /// <summary>
        /// Gets an asynchronous enumerator over the sequence.
        /// </summary>
        public IAsyncEnumerator<T> GetEnumerator() => new EFAsyncTaskEnumerator<T>(_task);

        /// <summary>
        /// Gets an asynchronous enumerator over the sequence.
        /// </summary>
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            return new EFAsyncTaskEnumerator<T>(_task);
        }
    }
}