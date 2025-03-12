using Kharazmi.AspNetCore.Core.Domain.Entities;
using Kharazmi.AspNetCore.Core.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Kharazmi.MongoDb
{
    public abstract class MongoEntity : ValueObject, IEntity<string>
    {
        protected MongoEntity()
        {
            _id = ObjectId.GenerateNewId().ToString();
        }

        [BsonElement("_id"), JsonProperty(PropertyName = "_id")]
        public string Id
        {
            get => _id;
            set => _id = string.IsNullOrEmpty(value) ? ObjectId.GenerateNewId().ToString() : value;
        }

        private string _id;
        public object GetObjectId() => Id;
    }
}