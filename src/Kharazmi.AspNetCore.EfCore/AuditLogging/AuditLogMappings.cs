using System;
using Kharazmi.AspNetCore.Core.AuditLogging;
using Kharazmi.AspNetCore.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kharazmi.AspNetCore.EFCore.AuditLogging
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyAuditLogConfiguration(this ModelBuilder builder, TableOption tableOption = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.ApplyConfiguration(new AuditLogMappings(tableOption));
        }
    }

    internal class AuditLogMappings : IEntityTypeConfiguration<AuditLog>
    {
        private readonly TableOption _tableOption;

        public AuditLogMappings(TableOption tableOption)
        {
            _tableOption = tableOption;
        }

        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable(_tableOption?.Name ?? "AuditLogs", _tableOption?.Schema ?? "dbo");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.SubjectName);
            builder.HasIndex(x => x.SubjectIdentifier);
        }
    }
}