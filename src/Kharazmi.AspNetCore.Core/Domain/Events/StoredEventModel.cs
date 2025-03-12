using System;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.Exceptions;

namespace Kharazmi.AspNetCore.Core.Domain.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class StoredEventModel : DomainEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <param name="payload"></param>
        /// <param name="userInfo"></param>
        /// <param name="requestInfo"></param>
        /// <param name="userClaimInfo"></param>
        public StoredEventModel(
            IDomainEvent domainEvent,
            object payload,
            UserInfo userInfo,
            RequestInfo requestInfo,
            UserClaimInfo userClaimInfo) : base(DomainEventType.From<StoredEventModel>())
        {
            if (domainEvent == null)
                throw new EventStoreNullException(nameof(domainEvent));

            var metaData = domainEvent.EventMetadata;

            if (metaData == null)
                throw new ArgumentNullException(nameof(metaData));

            AggregateId = metaData.AggregateId ?? throw new AggregateException("AggregateId is null");
            AggregateVersion = metaData.AggregateVersion;
            Name = $"{metaData.AggregateVersion}@{metaData.AggregateId}";
            EventTypeName = metaData.EventTypeName;
            SourceId = domainEvent.EventMetadata.SourceId.Value;
            Payload = payload;

            UserName = userInfo?.UserName;
            UserInfo = userInfo;
            RequestInfo = requestInfo;
            UserClaimInfo = userClaimInfo;
        }

        /// <summary></summary>
        public string Id { get; private set; }

        /// <summary></summary>
        public string AggregateId { get; private set; }

        /// <summary></summary>
        public ulong AggregateVersion { get; private set; }

        /// <summary></summary>
        public string Name { get; private set; }

        /// <summary></summary>
        public string EventTypeName { get; private set; }

        /// <summary></summary>
        public string SourceId { get; private set; }

        /// <summary></summary>
        public string UserName { get; private set; }

        /// <summary></summary>
        public object Payload { get; private set; }

        /// <summary></summary>
        public UserInfo UserInfo { get; private set; }

        /// <summary></summary>
        public RequestInfo RequestInfo { get; private set; }

        /// <summary></summary>
        public UserClaimInfo UserClaimInfo { get; private set; }

        /// <summary> </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateId(string value)
        {
            Id = value;
            return this;
        }

        /// <summary></summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateName(string value)
        {
            Name = value;
            return this;
        }

        /// <summary></summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateEventTypeName(string value)
        {
            EventTypeName = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateUserName(string value)
        {
            UserName = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateUserInfo(UserInfo value)
        {
            UserInfo = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateUserClaims(UserClaimInfo value)
        {
            UserClaimInfo = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateRequestInfo(RequestInfo value)
        {
            RequestInfo = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateEventDataId(object value)
        {
            Payload = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateAggregateId(string value)
        {
            AggregateId = value;
            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateSource(string value)
        {
            SourceId = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateVersion(ulong value)
        {
            AggregateVersion = value;
            return this;
        }
    }
}