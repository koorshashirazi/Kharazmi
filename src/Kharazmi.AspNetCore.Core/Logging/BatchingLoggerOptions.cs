using System;

namespace Kharazmi.AspNetCore.Core.Logging
{
    public class BatchingLoggerOptions
    {
        private int? _batchSize = 32;
        private int? _backgroundQueueSize;
        private TimeSpan _flushPeriod = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Gets or sets the period after which logs will be flushed to the store.
        /// </summary>
        public TimeSpan FlushPeriod
        {
            get => _flushPeriod;
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(FlushPeriod)} must be positive.");
                }

                _flushPeriod = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum size of the background log message queue or null for no limit.
        /// After maximum queue size is reached log event sink would start blocking.
        /// Defaults to <c>null</c>.
        /// </summary>
        public int? BackgroundQueueSize
        {
            get => _backgroundQueueSize;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                        $"{nameof(BackgroundQueueSize)} must be non-negative.");
                }

                _backgroundQueueSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a maximum number of events to include in a single batch or null for no limit.
        /// </summary>
        public int? BatchSize
        {
            get => _batchSize;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(BatchSize)} must be positive.");
                }

                _batchSize = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating if logger accepts and queues writes.
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}