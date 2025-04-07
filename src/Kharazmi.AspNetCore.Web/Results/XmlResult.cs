using System.Xml.Serialization;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Microsoft.AspNetCore.Mvc;

namespace Kharazmi.AspNetCore.Web.Results
{
    public class XmlResult : ActionResult
    {
        private readonly object _data;

        public XmlResult(object data)
        {
            Ensure.ArgumentIsNotNull(data, nameof(data));

            _data = data;
        }

        public override void ExecuteResult(ActionContext context)
        {
            Ensure.ArgumentIsNotNull(context, nameof(context));

            var response = context.HttpContext.Response;

            var serializer = new XmlSerializer(_data.GetType());

            response.ContentType = "text/xml";
            serializer.Serialize(response.Body, _data);
        }
    }
}