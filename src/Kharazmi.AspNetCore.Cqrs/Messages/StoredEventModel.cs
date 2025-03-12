using System;
using Kharazmi.AspNetCore.Core.EventSourcing;

namespace Kharazmi.AspNetCore.Cqrs.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class StoredEventModel : DomainEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="eventData"></param>
        /// <param name="userInfo"></param>
        /// <param name="requestInfo"></param>
        /// <param name="userClaimInfo"></param>
        public StoredEventModel(
            DomainEvent @event,
            object eventData,
            UserInfo userInfo,
            RequestInfo requestInfo,
            UserClaimInfo userClaimInfo)
        {
            if (@event != null)
            {
                AggregateId = @event.AggregateId;
                EventId = @event.EventId;
                Action = @event.Action;
                CreateAt = @event.CreateAt;
                IsEssential = @event.IsEssential;
            }

            EventData = eventData;
            UserId = userInfo?.UserId;
            UserName = userInfo?.UserName;
            UserInfo = userInfo;
            RequestInfo = requestInfo;
            UserClaimInfo = userClaimInfo;
        }

        /// <summary></summary>
        public string UserId { get; private set; }

        /// <summary></summary>
        public string UserName { get; private set; }

        /// <summary></summary>
        public object EventData { get; private set; }

        /// <summary></summary>
        public UserInfo UserInfo { get; private set; }

        /// <summary></summary>
        public RequestInfo RequestInfo { get; private set; }

        /// <summary></summary>
        public UserClaimInfo UserClaimInfo { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateId(string value)
        {
            EventId = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateEventDataId(object value)
        {
            EventData = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateUserId(string value)
        {
            UserId = value;
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
        public StoredEventModel UpdateCreateAt(DateTime value)
        {
            CreateAt = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateAction(string value)
        {
            Action = value;
            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateReason(string value)
        {
            Reason = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEventModel UpdateEssential(bool value)
        {
            IsEssential = value;
            return this;
        }

    }
}