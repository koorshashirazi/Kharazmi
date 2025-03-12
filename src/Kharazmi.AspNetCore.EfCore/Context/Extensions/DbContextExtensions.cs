using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kharazmi.AspNetCore.EFCore.Context.Extensions
{
    public static class DbContextExtensions
    {
      

        public static string GetValidationErrors(this DbContext context)
        {
            var errors = new StringBuilder();
            var entities = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity);
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(entity, validationContext, validationResults, true))
                    foreach (var validationResult in validationResults)
                    {
                        var names = validationResult.MemberNames.Aggregate((s1, s2) => $"{s1}, {s2}");
                        errors.AppendFormat("{0}: {1}", names, validationResult.ErrorMessage);
                    }
            }

            return errors.ToString();
        }
        
        /// <summary>
        /// Using the ChangeTracker to find names of the changed entities.
        /// </summary>
        public static IEnumerable<string> FindChangedEntityNames(this DbContext dbContext)
        {
            return dbContext.FindChangedEntries().FindEntityNames();
        }

        /// <summary>
        /// Using the ChangeTracker to find names of the changed entities.
        /// </summary>
        public static IEnumerable<string> FindEntityNames(this IEnumerable<EntityEntry> entryList)
        {
            var typesList = new List<Type>();
            foreach (var type in entryList.Select(entry => entry.Entity.GetType()))
            {
                typesList.Add(type);
                typesList.AddRange(type.FindBaseTypes().Where(t => t != typeof(object)).ToList());
            }

            var changedEntityNames = typesList
                .Select(type => type.FullName)
                .Distinct()
                .ToArray();

            return changedEntityNames;
        }

        /// <summary>
        /// Using the ChangeTracker to find types of the changed entities.
        /// </summary>
        public static IEnumerable<Type> FindChangedEntityTypes(this DbContext dbContext)
        {
            return dbContext.FindChangedEntries()
                .Select(dbEntityEntry => dbEntityEntry.Entity.GetType());
        }

        /// <summary>
        /// Find the base types of the given type, recursively.
        /// </summary>
        private static IEnumerable<Type> FindBaseTypes(this Type type)
        {
            if (type.GetTypeInfo().BaseType == null) return type.GetInterfaces();

            return Enumerable.Repeat(type.GetTypeInfo().BaseType, 1)
                .Concat(type.GetInterfaces())
                .Concat(type.GetInterfaces().SelectMany(FindBaseTypes))
                .Concat(type.GetTypeInfo().BaseType.FindBaseTypes());
        }

        public static void ThrowIfInvalidEntityExist(this DbContext context)
        {
            var errors = context.FindValidationErrors();
            if (string.IsNullOrWhiteSpace(errors)) return;

            var message = $"There are some validation errors while saving changes in EntityFramework:\n {errors}";

            throw new InvalidOperationException(message);
        }

        private static string FindValidationErrors(this DbContext context)
        {
            var errors = new StringBuilder();
            var entities = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity);
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                var validationResults = new List<ValidationResult>();

                if (Validator.TryValidateObject(entity, validationContext, validationResults, true)) continue;

                foreach (var validationResult in validationResults)
                {
                    var names = validationResult.MemberNames.Aggregate((s1, s2) => $"{s1}, {s2}");
                    errors.AppendFormat("{0}: {1}", names, validationResult.ErrorMessage);
                }
            }

            return errors.ToString();
        }

        public static IReadOnlyList<EntityEntry> FindChangedEntries(this DbContext context)
        {
            return context.ChangeTracker.Entries()
                .Where(x =>
                    x.State == EntityState.Added ||
                    x.State == EntityState.Modified ||
                    x.State == EntityState.Deleted)
                .ToList();
        }
        
        public static IEnumerable<string> FindPrimaryKeyNames<T>(this DbContext dbContext, T entity) {
            return from p in dbContext.FindPrimaryKeyProperties(entity) 
                select p.Name;
        }

        public static IEnumerable<object> FindPrimaryKeyValues<T>(this DbContext dbContext, T entity) {
            return from p in dbContext.FindPrimaryKeyProperties(entity) 
                select entity.GetPropertyValue(p.Name);
        }

        public static IReadOnlyList<IProperty> FindPrimaryKeyProperties<T>(this DbContext dbContext, T entity) {
            return dbContext.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties;
        }

        public static object GetPropertyValue<T>(this T entity, string name) {
            return entity.GetType().GetProperty(name)?.GetValue(entity, null);
        }
        
        public static void ApplySelectedValues<TEntity>(this DbContext context, TEntity entity, params Expression<Func<TEntity, object>>[] properties) where TEntity : class
        {
            var entry = context.Entry(entity);
            entry.State = EntityState.Unchanged;
            foreach (var prop in properties)
            {
                entry.Property(prop).IsModified = true;
            }
        }
    }
}