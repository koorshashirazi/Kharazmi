using System.Collections.Generic;

namespace Kharazmi.AspNetCore.Core.EventSourcing
{
    /// <summary>
    /// Configures events
    /// </summary>
    public class EventSourcingOptions
    {
        public bool EnableStoreEvent { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to raise events.
        /// </summary>
        public IReadOnlyCollection<string> NotAllowedEvents { get; set; } = [];
    }
}