using Kharazmi.AspNetCore.Core.EventSourcing;
using Newtonsoft.Json;
using Xunit;
using JsonSerializer = Kharazmi.AspNetCore.Core.EventSourcing.JsonSerializer;

namespace Kharazmi.EventSourcing.EfCore.XUnitTests.Specs
{
    [CollectionDefinition(nameof(JsonSerializerSpecs), DisableParallelization = true)]
    public class JsonProviderSpecsCollection { }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [Collection(nameof(JsonSerializerSpecs))]
    public class JsonSerializerSpecs
    {
        [Fact(DisplayName = "given string DeserializeObject should return object")]
        public void GivenStringDeserializeObjectShouldReturnObject()
        {
            //Arrange

            const string json = """
                                {
                                    "Id":1,
                                    "Name": "Dupont"
                                }
                                """;
            var obj = new ObjectToDeserializeTo(1, "Dupont");

            //Act
            IJsonSerializer sut = new JsonSerializer();
            var result = sut.Deserialize<ObjectToDeserializeTo>(json);
            //Assert

            Assert.Equal(obj.Id, result.Id);
            Assert.Equal(obj.Name, result.Name);
        }

        [Fact(DisplayName = "given event object serializeobject should return a string")]
        public void GivenEventObjectSerializeObjectShouldReturnString()
        {
            //Arrange

            var obj = new ObjectToDeserializeTo(1, "Dupont");
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            //Act
            IJsonSerializer sut = new JsonSerializer();
            var result = sut.Serialize(obj);

            //Assert

            Assert.Equal(json, result);
        }
    }
}