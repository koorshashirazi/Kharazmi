using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Kharazmi.AspNetCore.Web.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class ControllerHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="area"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string CreateActionPath(string area, string controller, string action)
        {
            return area.IsEmpty() ? $"~/{controller}/{action}" : $"~/{area}/{controller}/{action}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static (string areaName, string controllerName) GetMvcRouteData<T>() where T : Controller
        {
            return (typeof(T).GetAreaName(), typeof(T).ControllerName());
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string AreaName<T>() where T : Controller
        {
            var controller = typeof(T);
            return controller.GetAreaName();
        }
    }
}