using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace Kharazmi.AspNetCore.Localization.EFCore
{
    public static class LocalizationExtentions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static object CheckArgumentIsNull(this object o, string name)
        {
            switch (o)
            {
                case string objString when objString.IsEmpty():
                    throw new ArgumentNullException(name);
                case null:
                    throw new ArgumentNullException(name);
            }

            return o;
        }

        public static void AddDbLocalization<TContext>(this IServiceCollection services,
            Action<LocalizationOptions> options)
            where TContext : DbContext
        {
            services.Configure(options);

            services.AddSingleton<IStringLocalizerFactory, EfStringLocalizerFactory<TContext>>();
            services.AddSingleton<ILocalizerService, EfLocalizationService<TContext>>();
            services.AddSingleton<IEfLocalizerService, EfLocalizationService<TContext>>();
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
        }

        public static void ApplyLocalizationRecordConfiguration(this ModelBuilder builder, string table = "",
            string schema = "")
        {
            Ensure.IsNotNull(builder, nameof(builder));
            builder.ApplyConfiguration(new LocalizationRecordConfiguration(table, schema));
        }
    }
}