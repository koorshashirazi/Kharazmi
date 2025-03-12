using System;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;
using Kharazmi.EventSourcing.EfCore.Test.Specs.Events;

namespace Kharazmi.EventSourcing.EfCore.Test.Specs
{
    public class PersonAggregateTest : AggregateRoot<PersonAggregateTest, string>
    {
        public string FullName { get; private set; }
        public string Adresse { get; private set; }

        internal PersonAggregateTest(string id):base(id)
        {
            RegisterApplier<PersonCreatedEventTest>(Apply);
        }

        public PersonAggregateTest(string id, string fullName, string address): base(id)
        {
            RegisterApplier<PersonCreatedEventTest>(Apply);

            Emit(new PersonCreatedEventTest(fullName, address));
        }


        public void Apply(PersonCreatedEventTest eventObject)
        {
            ArgumentNullException.ThrowIfNull(eventObject);

            Id = eventObject.EventMetadata.AggregateId;
            FullName = eventObject.FullName;
            Adresse = eventObject.Address;
        }
    }
}