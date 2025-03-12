using System;

namespace Kharazmi.MessageBroker
{
    public class RabbitMqException : ArgumentException
    {
        public RabbitMqException(string message) : base(message)
        {
        }

        public RabbitMqException(string message, Exception e) : base(message, e)
        {
        }
    }
}