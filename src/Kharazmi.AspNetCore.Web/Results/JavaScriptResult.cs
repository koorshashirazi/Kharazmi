using Microsoft.AspNetCore.Mvc;

namespace Kharazmi.AspNetCore.Web.Results
{
    public class JavaScriptResult : ContentResult
    {
        public string Script { get => Content; set => Content = value; }
        
        public JavaScriptResult()
        {
            ContentType = "application/x-javascript";
        }
    }
}