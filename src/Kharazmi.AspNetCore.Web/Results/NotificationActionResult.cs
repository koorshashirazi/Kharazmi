using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Notifications;
using Kharazmi.AspNetCore.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Web.Results
{
    public class NotificationActionResult : IActionResult
    {
        public IActionResult InnerResult { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public NotificationType Type { get; set; }

        public Task ExecuteResultAsync(ActionContext context)
        {
            var factory = context.HttpContext?.RequestServices?.GetRequiredService<ITempDataDictionaryFactory>();
            var tempData = factory?.GetTempData(context.HttpContext);

            tempData.AddNotification(NotificationOptions.For(Type, Message, Title));
            return InnerResult.ExecuteResultAsync(context);
        }
    }
}