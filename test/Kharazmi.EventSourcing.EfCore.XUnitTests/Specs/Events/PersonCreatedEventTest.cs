using Kharazmi.AspNetCore.Core.Domain;

namespace Kharazmi.EventSourcing.EfCore.XUnitTests.Specs.Events
{
    public sealed class PersonCreatedEventTest : DomainEvent
    {
        public string FullName { get; }
        public string Address { get; }

        [Newtonsoft.Json.JsonConstructor, System.Text.Json.Serialization.JsonConstructor]
        public PersonCreatedEventTest(string fullName, string address)
            : base(DomainEventType.From<PersonCreatedEventTest>())
        {
            FullName = fullName;
            Address = address;
        }
    }
}