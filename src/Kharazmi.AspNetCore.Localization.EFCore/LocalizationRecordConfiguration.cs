
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kharazmi.AspNetCore.Localization.EFCore
{
    public class LocalizationRecordConfiguration : IEntityTypeConfiguration<LocalizationEntity>
    {
        private string _table;
        private string _schema;

        public LocalizationRecordConfiguration(string table, string schema)
        {
            _table = table;
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<LocalizationEntity> builder)
        {
            _table = _table.IsEmpty() ? "AppLocalizations" : _table;
            _schema = _schema.IsEmpty() ? "dbo" : _schema;

            builder.ToTable(_table, _schema);

            builder.HasKey(a => a.Id);
            builder.Property(a => a.Key).HasMaxLength(256).IsRequired();
            builder.Property(a => a.Value).IsRequired();
            builder.Property(a => a.Resource).HasMaxLength(256).IsRequired();
            builder.Property(a => a.CultureName).HasMaxLength(10).IsRequired();
        }
    }
}