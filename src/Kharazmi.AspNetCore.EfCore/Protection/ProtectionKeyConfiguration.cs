using System;
using Kharazmi.AspNetCore.Core.Cryptography;
using Kharazmi.AspNetCore.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kharazmi.AspNetCore.EFCore.Protection
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyProtectionKeyConfiguration(this ModelBuilder builder, TableOption tableOption = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.ApplyConfiguration(new ProtectionKeyConfiguration(tableOption));
        }
    }

    public class ProtectionKeyConfiguration : IEntityTypeConfiguration<ProtectionKey>
    {
        private readonly TableOption _tableOption;

        public ProtectionKeyConfiguration() : this(new TableOption(name: nameof(ProtectionKey), "dbo"))
        {
        }

        public ProtectionKeyConfiguration(TableOption tableOption)
        {
            _tableOption = tableOption ?? new TableOption(name: nameof(ProtectionKey), "dbo");
        }

        public void Configure(EntityTypeBuilder<ProtectionKey> builder)
        {
            builder.ToTable(_tableOption.Name ?? nameof(ProtectionKey), _tableOption.Schema ?? "dbo");
            builder.HasKey(x => x.Id);
            builder.Property(a => a.FriendlyName).IsRequired();
            builder.HasIndex(a => a.FriendlyName).IsUnique().HasName("IX_ProtectionKey_FriendlyName");
        }
    }
}