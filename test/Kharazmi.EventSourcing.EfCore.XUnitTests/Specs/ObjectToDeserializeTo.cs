namespace Kharazmi.EventSourcing.EfCore.XUnitTests.Specs
{
    public class ObjectToDeserializeTo
    {
        public int Id { get; }

        public string Name { get; }

        public ObjectToDeserializeTo(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}