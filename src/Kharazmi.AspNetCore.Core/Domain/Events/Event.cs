// using System;
//
// namespace Kharazmi.AspNetCore.Core.Domain.Events
// {
//     /// <summary></summary>
//     public interface IEvent : IMessage
//     {
//         /// <summary></summary>
//         string EventId { get; }
//
//         /// <summary></summary>
//         string Source { get; }
//
//         /// <summary></summary>
//         string Reason { get; }
//
//         /// <summary></summary>
//         bool IsEssential { get; }
//
//         /// <summary></summary>
//         int Version { get; }
//     }
//
//     /// <summary></summary>
//     public abstract class Event : Message, IEvent
//     {
//         /// <summary></summary>
//         protected Event()
//         {
//             EventId = Guid.NewGuid().ToString("N");
//         }
//
//         /// <summary></summary>
//         public string EventId { get; protected set; }
//
//         /// <summary></summary>
//         public string Source { get; protected set; }
//
//         /// <summary></summary>
//         public string Reason { get; protected set; }
//
//         /// <summary></summary>
//         public bool IsEssential { get; protected set; }
//
//         /// <summary></summary>
//         public int Version { get; protected set; }
//     }
// }