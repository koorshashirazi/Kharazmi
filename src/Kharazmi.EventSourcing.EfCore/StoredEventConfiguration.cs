using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kharazmi.EventSourcing.EfCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public static void ApplyStoredEventConfiguration(this ModelBuilder builder)
        {
            builder.CheckArgumentIsNull(nameof(builder));
            builder.ApplyConfiguration(new StoredEventConfiguration());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class StoredEventConfiguration : IEntityTypeConfiguration<EventStoreEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<EventStoreEntity> builder)
        {
            builder.ToTable("EventStores", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(c => c.AggregateId).IsRequired();
            builder.Property(c => c.AggregateVersion).IsRequired();
            builder.Property(c => c.AggregateType).IsRequired();
            builder.Property(c => c.EventType).HasMaxLength(450).IsRequired();
            builder.Property(c => c.PayLoad).IsRequired().HasColumnType("text");

            builder.HasIndex(e => new { e.AggregateType, e.AggregateId });
            builder.HasIndex(x => x.AggregateId);


        }
    }
}