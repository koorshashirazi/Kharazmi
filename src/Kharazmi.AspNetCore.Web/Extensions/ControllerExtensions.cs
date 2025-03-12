using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kharazmi.AspNetCore.Web.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ControllerExtensions
    {
        public static string ControllerName(this Type controller, string nullString = "")
        {
            return controller.IsSubclassOf(typeof(Controller)) ? controller.Name.Replace("Controller", "") : "";
        }

        public static List<Type> GetControllerTypesWithSpecialAttribute<T>(this Assembly currentAssembly)
            where T : Attribute
        {
            var allControllers = currentAssembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Controller))).ToList();
            return allControllers.Where(t => t.GetCustomAttribute<T>() != null ||
                                             t.GetMethods().Any(m => m.GetCustomAttribute<T>() != null)).ToList();
        }

        public static List<(string Controller, string Action, string RetrunType, string Attribute)> GetControllerTypes(
            this Assembly asm)
        {
            var mvcActionDispatcher = asm.GetTypes()
                .Where(type => typeof(Controller).IsAssignableFrom(type))
                .SelectMany(type =>
                    type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                    true).Any())
                .Select(x => (Controller: x.DeclaringType?.Name, Action: x.Name, ReturnType: x.ReturnType.Name,
                    Attributes: String.Join(",",
                        x.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", "")))))
                .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();

            return mvcActionDispatcher;
        }

        public static bool IsSecure(this Type controllerType)
        {
            if (controllerType == null) throw new ArgumentNullException(nameof(controllerType));
            if (!controllerType.IsSubclassOf(typeof(Controller)))
                throw new TypeAccessException(nameof(controllerType));
            var navigation = controllerType.GetCustomAttribute<AuthorizeAttribute>();
            return navigation != null;
        }

        public static bool HasRole(this Type type, string methodName)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            try
            {
                var action = type.GetMethod(methodName);

                if (methodName.IsNotEmpty() && action == null)
                    throw new ArgumentNullException($"Type: {type.FullName}, Mehtod: {nameof(methodName)}");

                var controllerAuthorizeAttribute = type.GetCustomAttribute<AuthorizeAttribute>();

                if (controllerAuthorizeAttribute != null && controllerAuthorizeAttribute.Roles.IsNotEmpty())
                    return true;

                var actionAuthorizeAttribute = action?.GetCustomAttribute<AuthorizeAttribute>();

                return actionAuthorizeAttribute != null && actionAuthorizeAttribute.Roles.IsNotEmpty();
            }
            catch (Exception)
            {
                throw new AmbiguousMatchException($"Type: {type.FullName}, Method: {methodName}");
            }
        }

        public static bool HasPolicy(this Type type, string methodName)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            try
            {
                var action = type.GetMethod(methodName);

                if (methodName.IsNotEmpty() && action == null)
                    throw new ArgumentNullException($"Type: {type.FullName}, Mehtod: {nameof(methodName)}");

                var controllerAuthorizeAttribute = type.GetCustomAttribute<AuthorizeAttribute>();

                if (controllerAuthorizeAttribute != null && controllerAuthorizeAttribute.Policy.IsNotEmpty())
                    return true;

                var actionAuthorizeAttribute = action?.GetCustomAttribute<AuthorizeAttribute>();

                return actionAuthorizeAttribute != null && actionAuthorizeAttribute.Policy.IsNotEmpty();
            }
            catch (Exception)
            {
                throw new AmbiguousMatchException($"Type: {type.FullName}, Method: {methodName}");
            }
        }

        public static bool IsSecure(this Type type, string methodName)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            try
            {
                var action = type.GetMethod(methodName);
                if (methodName.IsNotEmpty() && action == null)
                    throw new ArgumentNullException($"Type: {type.FullName}, Mehtod: {nameof(methodName)}");

                return IsSecure(type, action);
            }
            catch (Exception)
            {
                throw new AmbiguousMatchException($"Type: {type.FullName}, Method: {methodName}");
            }
        }

        public static bool IsSecure(this Type type, MemberInfo method)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var authorizeAttribute = type.GetCustomAttribute<AuthorizeAttribute>();

            var action = method;

            if (authorizeAttribute != null) return true;

            if (action != null) return action.GetCustomAttribute<AuthorizeAttribute>() != null;

            return true;
        }

        public static string CreateActionPath(this IUrlHelper url, string area, string controller, string action)
        {
            return url.Action(action, controller, !string.IsNullOrWhiteSpace(area) ? new {area} : null);
        }


        public static (string areaName, string controllerName) GetMvcRouteData(this Type controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (!controller.IsSubclassOf(typeof(Controller)))
                throw new TypeAccessException(nameof(controller));

            return (controller.GetAreaName(), controller.ControllerName());
        }

        public static (string areaName, string controllerName, string actionName) GetMvcRouteData(this Type controller,
            MemberInfo method)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (!controller.IsSubclassOf(typeof(Controller)))
                throw new TypeAccessException(nameof(controller));

            return (controller.GetAreaName(), controller.ControllerName(), method.Name);
        }

        public static string AreaName(this Type controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (!controller.IsSubclassOf(typeof(Controller)))
                throw new TypeAccessException(nameof(controller));

            return controller.GetAreaName();
        }

        public static string ActionName(this Type controller, string actionName)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (!controller.IsSubclassOf(typeof(Controller)))
                throw new TypeAccessException(nameof(controller));

            var action = controller.GetMethod(actionName);

            if (actionName.IsNotEmpty() && action == null)
                throw new ArgumentNullException($"Type: {controller.FullName}, Mehtod: {nameof(actionName)}");

            return action?.Name;
        }

        public static string GetAreaName(this Type controller)
        {
            var area = "";
            if (string.IsNullOrWhiteSpace(controller.Namespace)) return area;
            if (!controller.Namespace.Contains("Areas")) return area;
            var parts = controller.Namespace.Split('.').ToList();
            area = parts[parts.FindLastIndex(n => n.Equals("Areas")) + 1];
            return area;
        }

        /// <summary>
        /// Gets the ControllerType's Key
        /// </summary>
        public static string ControllerName(this Type controllerType)
        {
            var baseType = typeof(Controller);
            if (!baseType.GetTypeInfo().IsAssignableFrom(controllerType))
                throw new InvalidOperationException(
                    "This method should be used for `Microsoft.AspNetCore.Mvc.Controller`s.");

            var lastControllerIndex = controllerType.Name.LastIndexOf("Controller", StringComparison.Ordinal);
            if (lastControllerIndex > 0)
                return controllerType.Name.Substring(0, lastControllerIndex);

            throw new InvalidOperationException("This type's name doesn't end with `Controller`.");
        }
    }
}