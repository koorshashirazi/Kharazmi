using Microsoft.AspNetCore.Mvc;

namespace Kharazmi.AspNetCore.Web.Extensions
{
    public static class JsonNotifactionExtensions
    {
        public static IActionResult ErrorNotification(this Controller controller, string message)
        {
            return controller.Json(new {message = message, type = "error", __notification = true});
        }

        public static IActionResult SuccessNotification(this Controller controller, string message)
        {
            return controller.Json(new {message = message, type = "success", __notification = true});
        }

        public static IActionResult WarningNotification(this Controller controller, string message)
        {
            return controller.Json(new {message = message, type = "warning", __notification = true});
        }

        public static IActionResult InformationNotification(this Controller controller, string message)
        {
            return controller.Json(new {message = message, type = "info", __notification = true});
        }

        public static IActionResult ErrorMessage(this Controller controller, string title, string message)
        {
            return controller.Json(new {title = title, message = message, type = "error", __message = true});
        }

        public static IActionResult SuccessMessage(this Controller controller, string title, string message)
        {
            return controller.Json(new {title = title, message = message, type = "success", __message = true});
        }

        public static IActionResult WarningMessage(this Controller controller, string title, string message)
        {
            return controller.Json(new {title = title, message = message, type = "warning", __message = true});
        }

        public static IActionResult InformationMessage(this Controller controller, string title, string message)
        {
            return controller.Json(new {title = title, message = message, type = "info", __message = true});
        }
    }
}