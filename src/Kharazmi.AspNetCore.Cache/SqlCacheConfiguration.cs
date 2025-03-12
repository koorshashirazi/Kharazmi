using System;
using Kharazmi.AspNetCore.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kharazmi.AspNetCore.Cache
{
    public static class ModelBuilderExtensions
    {
        public static void ApplySqlCacheConfiguration(this ModelBuilder builder, TableOption tableOption = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.ApplyConfiguration(new CacheConfiguration(tableOption));
        }
    }

    public class CacheConfiguration : IEntityTypeConfiguration<Core.Caching.Cache>
    {
        private readonly TableOption _tableOption;

        public CacheConfiguration() : this(new TableOption("Cache", "dbo"))
        {
        }

        public CacheConfiguration(TableOption tableOption)
        {
            _tableOption = tableOption ?? new TableOption("Cache", "dbo");
        }

        public void Configure(EntityTypeBuilder<Core.Caching.Cache> builder)
        {
            builder.Property(e => e.Id).HasMaxLength(449);
            builder.Property(e => e.Value).IsRequired();

            builder.HasIndex(e => e.ExpiresAtTime).HasName("IX_Cache_ExpiresAtTime");

            builder.ToTable(name: _tableOption.Name ?? "Cache", schema: _tableOption.Schema ?? "dbo");
        }
    }
}