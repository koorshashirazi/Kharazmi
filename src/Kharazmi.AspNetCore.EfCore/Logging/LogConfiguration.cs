using System;
using Kharazmi.AspNetCore.Core.Domain.Entities;
using Kharazmi.AspNetCore.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kharazmi.AspNetCore.EFCore.Logging
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyLogConfiguration(this ModelBuilder builder, TableOption tableOption = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.ApplyConfiguration(new LogConfiguration(tableOption));
        }
    }

    public class LogConfiguration : IEntityTypeConfiguration<Log>
    {
        private readonly TableOption _tableOption;

        public LogConfiguration() : this(new TableOption(nameof(Log), "dbo"))
        {
        }

        public LogConfiguration(TableOption tableOption)
        {
            _tableOption = tableOption ?? new TableOption(nameof(Log), "dbo");
        }

        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.ToTable(_tableOption.Name ?? nameof(Log), _tableOption.Schema ?? "dbo");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.HasIndex(e => e.LoggerName).HasName("IX_Log_LoggerName");
            builder.HasIndex(e => e.Level).HasName("IX_Log_Level");

            builder.Property(a => a.Level).HasMaxLength(50).IsRequired();
            builder.Property(a => a.Message).IsRequired();
            builder.Property(a => a.LoggerName).HasMaxLength(256).IsRequired();
            builder.Property(a => a.UserDisplayName).HasMaxLength(50);
            builder.Property(a => a.UserName).HasMaxLength(50);
            builder.Property(a => a.UserBrowserName).HasMaxLength(1024);
            builder.Property(a => a.UserIP).HasMaxLength(256);
            builder.Property(a => a.UserId).HasMaxLength(256);
            builder.Property(a => a.TenantId).HasMaxLength(256);
            builder.Property(a => a.TenantName).HasMaxLength(256);
            builder.Property(a => a.ImpersonatorUserId).HasMaxLength(256);
            builder.Property(a => a.ImpersonatorTenantId).HasMaxLength(256);
        }
    }
}