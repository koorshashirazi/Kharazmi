using System;

namespace Kharazmi.AspNetCore.Core.Notifications
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NotificationOptions
    {
        public NotificationOptions()
        {
            NotificationId = Guid.NewGuid().ToString("N");
        }

        public static NotificationOptions Empty => new NotificationOptions();

        public static NotificationOptions For(NotificationType type, string message, string Title) =>
            new NotificationOptions
            {
                Title = Title,
                Message = message,
                NotificationType = type
            };

        protected string NotificationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowNewestOnTop { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowCloseButton { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public NotificationType NotificationType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSticky { get; set; }
    }
}