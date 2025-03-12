using System;

namespace Kharazmi.AspNetCore.Core.Application.Models
{

    public enum ModelStateType
    {
        Success,
        Error,
        Info,
        Warning
    }

    public class ModelStateViewModel : IEquatable<ModelStateViewModel>
    {
        public ModelStateType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        public bool Equals(ModelStateViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && Title == other.Title && Message == other.Message;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ModelStateViewModel)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Type;
                hashCode = (hashCode * 397) ^ (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}