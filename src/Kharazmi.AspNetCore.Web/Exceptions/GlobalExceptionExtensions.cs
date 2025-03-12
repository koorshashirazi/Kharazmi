using Microsoft.AspNetCore.Builder;

 namespace Kharazmi.AspNetCore.Web.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public static class GlobalExceptionExtensions
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder builder)
            => builder.UseMiddleware<GlobalErrorHandlerMiddleware>();
    }
}