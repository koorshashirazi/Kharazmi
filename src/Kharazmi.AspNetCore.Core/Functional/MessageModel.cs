using System;

namespace Kharazmi.AspNetCore.Core.Functional
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <param name="code"></param>
        /// <param name="messageType"></param>
        protected MessageModel(string description, string code, string messageType)
        {
            Code = code;
            Description = description;
            MessageType = messageType;
            CreateAt = DateTime.Now.ToString("g");
            MessageId = Guid.NewGuid().ToString("N");
        }

        /// <summary> </summary>
        public string CreateAt { get; }

        /// <summary> </summary>
        public string MessageId { get; }

        /// <summary> </summary>
        public string Code { get; }

        /// <summary> </summary>
        public string Description { get; }

        /// <summary> </summary>
        public string MessageType { get; protected set; }

        /// <summary> </summary>
        public static MessageModel Empty => new MessageModel("", "", "None");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <param name="code"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public static MessageModel? For(string description, string code = "", string messageType = "None") =>
            new MessageModel(description, code, messageType);

        public static MessageModel From(Result result) =>
            new MessageModel(result.Description, result.Code, result.ResultType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public MessageModel UpdateMessageType(string type)
        {
            MessageType = type;
            return this;
        }
    }
}