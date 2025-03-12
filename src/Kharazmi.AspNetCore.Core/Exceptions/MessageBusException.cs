namespace Kharazmi.AspNetCore.Core.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageBusException : FrameworkException
    {
        private MessageBusException(string message) : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static MessageBusException Create(string message) => new MessageBusException(message);

        /// <summary>/ </summary>
        public static MessageBusException Empty => new MessageBusException("");
    }
}