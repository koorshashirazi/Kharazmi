namespace Kharazmi.AspNetCore.Web.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class RedirectResultExceptionOptions
    {
        public RedirectResultExceptionOptions(string controllerName, string actionName, object routeData)
        {
            ControllerName = controllerName;
            ActionName = actionName;
            RouteData = routeData;
        }

        public string ControllerName { get; }
        public string ActionName { get; }
        public object RouteData { get; }
    }
}