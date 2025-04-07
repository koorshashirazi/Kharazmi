using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kharazmi.AspNetCore.Core.Application.Models;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Web.Extensions
{
    public static class ModelStateExtensions
    {
        public static void ClearError<T>(this ModelStateDictionary modelState, T model, string nameOfPropertyModel = "")
            where T : class, new()
        {
            var properties = model.GetType().GetProperties();
            foreach (var item in properties)
            {
                var key = string.IsNullOrWhiteSpace(nameOfPropertyModel)
                    ? item.Name
                    : $"{nameOfPropertyModel}.{item.Name}";
                if (!modelState.TryGetValue(key, out var value)) continue;

                if (value.Errors.Any()) modelState.Remove(key);
            }
        }

        public static string GetError(this ModelStateDictionary modelState)
        {
            return string.Join("; ", modelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage));
        }

        public static List<string> GetErrorList(this ModelStateDictionary modelState)
        {
            return modelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
        }

        public static List<ModelStateViewModel> GetModelStateViewModels(this ModelStateDictionary modelState)
        {
            return modelState.GetErrorList()
                .Select(error => new ModelStateViewModel { Type = ModelStateType.Error, Message = error }).ToList();
        }

        /// <summary>
        /// Converts the <paramref name="modelState"/> to a dictionary that can be easily serialized.
        /// </summary>
        public static IDictionary<string, string[]> ToSerializableDictionary(this ModelStateDictionary modelState)
        {
            return modelState.Where(x => x.Value.Errors.Any()).ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );
        }

        /// <summary>
        /// Stores the errors in a ModelValidationResult object to the specified modelstate dictionary.
        /// </summary>
        /// <param name="result">The validation result to store</param>
        /// <param name="modelState">The ModelStateDictionary to store the errors in.</param>
        /// <param name="prefix">An optional prefix. If ommitted, the property names will be the keys. If specified, the prefix will be concatenatd to the property name with a period. Eg "user.Key"</param>
        public static void AddModelError(this ModelStateDictionary modelState, Result result, string? prefix = null)
        {
            if (modelState == null) throw new ArgumentNullException(nameof(modelState));
            if (!result.Failed) return;

            var modelKey = prefix ?? result.FriendlyMessage.MessageType;

            modelState.AddModelError(modelKey, result.FriendlyMessage.Description);


            foreach (var identityError in result.Messages)
                modelState.AddModelError(identityError.MessageType, identityError.Description);

            foreach (var failure in result.ValidationMessages)
            {
                var key = prefix.IsEmpty() || failure.PropertyName.IsEmpty()
                    ? failure.PropertyName
                    : prefix + "." + failure.PropertyName;

                if (!modelState.ContainsKey(key) ||
                    modelState[key].Errors.All(i => i.ErrorMessage != failure.ErrorMessage))
                {
                    modelState.AddModelError(key, failure.ErrorMessage);
                }
            }
        }

        public static string ExportErrors(this ModelStateDictionary modelState, bool useHtmlNewLine = false)
        {
            var builder = new StringBuilder();

            foreach (var error in modelState.Values.SelectMany(a => a.Errors))
            {
                var message = error.ErrorMessage;
                if (string.IsNullOrWhiteSpace(message))
                {
                    continue;
                }

                builder.AppendLine(!useHtmlNewLine ? message : $"{message}<br/>");
            }

            return builder.ToString();
        }


        public static void ExportModelStateToTempData(this ModelStateDictionary modelState, Controller controller,
            string key)
        {
            if (controller != null && modelState != null)
            {
                var modelStateJson = SerializeModelState(modelState);
                controller.TempData[key] = modelStateJson;
            }
        }

        public static string SerializeModelState(this ModelStateDictionary modelState)
        {
            var values = modelState
                .Select(kvp => new ModelStateTransferValue
                {
                    Key = kvp.Key,
                    AttemptedValue = kvp.Value.AttemptedValue,
                    RawValue = kvp.Value.RawValue,
                    ErrorMessages = kvp.Value.Errors.Select(err => err.ErrorMessage).ToList(),
                });

            return JsonConvert.SerializeObject(values);
        }

        public class ModelStateTransferValue
        {
            public string Key { get; set; }
            public string AttemptedValue { get; set; }
            public object RawValue { get; set; }
            public ICollection<string> ErrorMessages { get; set; } = new List<string>();
        }
    }
}