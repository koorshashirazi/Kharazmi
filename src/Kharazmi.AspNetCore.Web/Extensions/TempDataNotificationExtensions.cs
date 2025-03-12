using System;
using System.Collections.Generic;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Helpers;
using Kharazmi.AspNetCore.Core.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Web.Extensions
{public static class TempDataNotificationExtensions
    {
        private const string NotificationKey = NotificationConstant.NotificationKey;

        private static readonly JsonSerializerSettings Settings = JsonSerializerHelper.DefaultJsonSettings;

        public static void AddNotification(this ITempDataDictionary tempData, NotificationOptions options)
        {
            tempData.AddAlert(options);
        }

        public static void AddNotification(
            this Controller controller, NotificationOptions notificationOptions)
        {
            notificationOptions ??=  NotificationOptions.Empty;
            controller.TempData.AddNotification(notificationOptions);
        }

        public static Controller AddSuccessMessage(this Controller controller, MessageModel message)
        {
            controller.AddNotification(new NotificationOptions
            {
                NotificationType = NotificationType.Success,
                Message = message.Description,
                Title = message.Code
            });
            return controller;
        }

        public static Controller AddSuccessMessages(this Controller controller, List<MessageModel> messages)
        {
            foreach (var message in messages)
            {
                controller.AddNotification(new NotificationOptions
                {
                    NotificationType = NotificationType.Success,
                    Message = message.Description,
                    Title = message.Code
                });
            }

            return controller;
        }

        public static Controller AddInfoMessage(this Controller controller, MessageModel message)
        {
            controller.AddNotification(new NotificationOptions
            {
                NotificationType = NotificationType.Info,
                Message = message.Description,
                Title = message.Code
            });
            return controller;
        }

        public static Controller AddInfoMessages(this Controller controller, List<MessageModel> messages)
        {
            foreach (var message in messages)
            {
                controller.AddNotification(new NotificationOptions
                {
                    NotificationType = NotificationType.Info,
                    Message = message.Description,
                    Title = message.Code
                });
            }

            return controller;
        }

        public static Controller AddErrorMessage(this Controller controller, MessageModel message)
        {
            controller.AddNotification(new NotificationOptions
            {
                NotificationType = NotificationType.Error,
                Message = message.Description,
                Title = message.Code
            });
            return controller;
        }

        public static Controller AddErrorMessages(this Controller controller, List<MessageModel> messages)
        {
            foreach (var message in messages)
            {
                controller.AddNotification(new NotificationOptions
                {
                    NotificationType = NotificationType.Error,
                    Message = message.Description,
                    Title = message.Code
                });
            }

            return controller;
        }

        public static Controller AddWarningMessage(this Controller controller, MessageModel message)
        {
            controller.AddNotification(new NotificationOptions
            {
                NotificationType = NotificationType.Warning,
                Message = message.Description,
                Title = message.Code
            });
            return controller;
        }

        public static Controller AddWarningMessages(this Controller controller, List<MessageModel> messages)
        {
            foreach (var message in messages)
            {
                controller.AddNotification(new NotificationOptions
                {
                    NotificationType = NotificationType.Warning,
                    Message = message.Description,
                    Title = message.Code
                });
            }

            return controller;
        }

        public static List<NotificationOptions> GetNotifications(this ITempDataDictionary tempData)
        {
            CreateTempData(tempData);
            return DeserializeAlerts(tempData[NotificationKey] as string);
        }

        private static void AddAlert(this ITempDataDictionary tempData, NotificationOptions alert)
        {
            if (alert == null) throw new ArgumentNullException(nameof(alert));

            var deserializeAlertList = tempData.GetNotifications();
            deserializeAlertList.Add(alert);
            tempData[NotificationKey] = SerializeAlerts(deserializeAlertList);
        }

        private static void CreateTempData(this ITempDataDictionary tempData)
        {
            if (!tempData.ContainsKey(NotificationKey))
                tempData[NotificationKey] = string.Empty;
        }

        private static string SerializeAlerts(List<NotificationOptions> tempData)
        {
            return JsonConvert.SerializeObject(tempData, Settings);
        }

        private static List<NotificationOptions> DeserializeAlerts(string tempData)
        {
            return tempData.Length == 0
                ? new List<NotificationOptions>()
                : JsonConvert.DeserializeObject<List<NotificationOptions>>(tempData, Settings);
        }

      
    }
   
}