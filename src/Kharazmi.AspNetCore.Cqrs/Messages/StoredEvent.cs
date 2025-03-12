using System;

namespace Kharazmi.AspNetCore.Cqrs.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class StoredEvent : DomainEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="userName"></param>
        /// <param name="userInfo"></param>
        /// <param name="requestInfo"></param>
        /// <param name="userClaimsInfo"></param>
        /// <param name="eventData"></param>
        /// <param name="userId"></param>
        public StoredEvent(
            DomainEvent @event,
            string eventData,
            string userId,
            string userName,
            string userInfo,
            string requestInfo,
            string userClaimsInfo)
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
            UserId = userId;
            UserName = userName;
            UserInfo = userInfo;
            RequestInfo = requestInfo;
            UserClaimsInfo = userClaimsInfo;
        }

        /// <summary>
        ///  EF Constructor
        /// </summary>
        protected StoredEvent()
        {
        }

        /// <summary></summary>
        public string UserId { get; private set; }

        /// <summary></summary>
        public string UserName { get; private set; }

        /// <summary></summary>
        public string EventData { get; private set; }

        /// <summary></summary>
        public string UserInfo { get; private set; }

        /// <summary></summary>
        public string RequestInfo { get; private set; }

        /// <summary></summary>
        public string UserClaimsInfo { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEvent UpdateId(string value)
        {
            EventId = value;
            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEvent UpdateUserId(string value)
        {
            UserId = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEvent UpdateUserName(string value)
        {
            UserName = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEvent UpdateUserInfo(string value)
        {
            UserInfo = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEvent UpdateUserClaims(string value)
        {
            UserClaimsInfo = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEvent UpdateRequestInfo(string value)
        {
            RequestInfo = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEvent UpdateAggregateId(string value)
        {
            AggregateId = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEvent UpdateCreateAt(DateTime value)
        {
            CreateAt = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEvent UpdateAction(string value)
        {
            Action = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEvent UpdateReason(string value)
        {
            Reason = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StoredEvent UpdateEssential(bool value)
        {
            IsEssential = value;
            return this;
        }
    }
}