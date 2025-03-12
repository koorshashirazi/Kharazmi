using Kharazmi.AspNetCore.Core.Notifications;
using Kharazmi.AspNetCore.Web.Results;
using Microsoft.AspNetCore.Mvc;

namespace Kharazmi.AspNetCore.Web.Extensions
{
    public static class ActionNotificationExtensions
    {
        public static IActionResult WithInformationNotification(this IActionResult result, string message, string title)
        {
            return new NotificationActionResult
                {InnerResult = result, Type = NotificationType.Info, Message = message, Title = title};
        }

        public static IActionResult WithErrorNotification(this IActionResult result, string message, string title)
        {
            return new NotificationActionResult
                {InnerResult = result, Type = NotificationType.Error, Message = message, Title = title};
        }

        public static IActionResult WithWarningNotification(this IActionResult result, string message, string title)
        {
            return new NotificationActionResult
                {InnerResult = result, Type = NotificationType.Warning, Message = message, Title = title};
        }

        public static IActionResult WithSuccessNotification(this IActionResult result, string message, string title)
        {
            return new NotificationActionResult
                {InnerResult = result, Type = NotificationType.Success, Message = message, Title = title};
        }
    }
}