namespace Kharazmi.AspNetCore.Web.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class ViewResultExceptionOptions
    {
        public ViewResultExceptionOptions(string errorViewName)
        {
            ErrorViewName = errorViewName;
        }

        public string ErrorViewName { get; }
    }
}