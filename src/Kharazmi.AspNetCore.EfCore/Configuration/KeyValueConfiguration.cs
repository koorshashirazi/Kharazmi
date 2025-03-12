using System;
using Kharazmi.AspNetCore.Core.Configuration;
using Kharazmi.AspNetCore.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kharazmi.AspNetCore.EFCore.Configuration
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyConfigurationValue(this ModelBuilder builder, TableOption tableOption = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.ApplyConfiguration(new KeyValueConfiguration(tableOption));
        }
    }

    public class KeyValueConfiguration : IEntityTypeConfiguration<KeyValue>
    {
        private readonly TableOption _tableOption;

        public KeyValueConfiguration() : this(new TableOption("Values", "dbo"))
        {
        }

        public KeyValueConfiguration(TableOption tableOption)
        {
            _tableOption = tableOption ?? new TableOption("Values", "dbo");
        }

        public void Configure(EntityTypeBuilder<KeyValue> builder)
        {
            builder.ToTable(_tableOption.Name ?? "Values", _tableOption.Schema ?? "dbo");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(v => v.Key).HasMaxLength(450).IsRequired();
            builder.Property(v => v.Value).IsRequired();
            builder.HasIndex(v => v.Key).HasName("UIX_Values_Key").IsUnique();
        }
    }
}