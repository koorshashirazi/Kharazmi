using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kharazmi.AspNetCore.Core.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// Represents the parameter rebinder used for rebinding the parameters
        /// for the given expressions. This is part of the solution which solves
        /// the expression parameter problem when going to Entity Framework.
        /// For more information about this solution please refer to http://blogs.msdn.com/b/meek/archive/2008/05/02/linq-to-entities-combining-predicates.aspx.
        /// </summary>
        private class RebindParameterVisitor : ExpressionVisitor
        {
            private readonly Dictionary<ParameterExpression, ParameterExpression> map;

            public RebindParameterVisitor(
                Dictionary<ParameterExpression,
                ParameterExpression> map)
            {
                this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(
                Dictionary<ParameterExpression,
                ParameterExpression> map,
                Expression exp)
            {
                return new RebindParameterVisitor(map).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                if (map.TryGetValue(p, out var replacement))
                {
                    p = replacement;
                }
                return base.VisitParameter(p);
            }
        }
        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            return f => false;
        }

        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = RebindParameterVisitor.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        /// <summary>
        /// Combines two given expressions by using the AND semantics.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="first">The first part of the expression.</param>
        /// <param name="second">The second part of the expression.</param>
        /// <returns>The combined expression.</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }
        /// <summary>
        /// Combines two given expressions by using the OR semantics.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="first">The first part of the expression.</param>
        /// <param name="second">The second part of the expression.</param>
        /// <returns>The combined expression.</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }
    }


    // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/how-to-use-expression-trees-to-build-dynamic-queries
     //https://www.codeproject.com/Articles/1079028/Build-Lambda-Expressions-Dynamically
    // public class Filter<T> : IFilter<T> where T : class
    // {
    //     public List<IFilterStatement> Statements { get; set; }
    //     public Expression<Func<T, bool>> BuildExpression()
    //     {
    //         //this is in case the list of statements is empty
    //         Expression finalExpression = Expression.Constant(true);
    //         var parameter = Expression.Parameter(typeof(T), "x");
    //         foreach (var statement in Statements)
    //         {
    //             var member = Expression.Property(parameter, statement.PropertyName);
    //             var constant = Expression.Constant(statement.Value);
    //             Expression expression = null;
    //             switch (statement.Operation)
    //             {
    //                 case Operation.EqualTo:
    //                     expression = Expression.Equal(member, constant);
    //                     break;
    //                 case Operation.LessThanOrEqualTo:
    //                     expression = Expression.GreaterThanOrEqual(member, constant);
    //                     "".Contains("");
    //                     break;
    //                 case Operation.Contains:
    //                     expression = Expression.Call(member, typeof(string).GetMethod("Contains", System.Type.EmptyTypes));
    //                     break;
    //                 case Operation.StartsWith:
    //                     expression = Expression.Call(member, typeof(string).GetMethod("StartsWith", System.Type.EmptyTypes));
    //                     expression = Expression.st(member, constant);
    //                     break;
    //                 case Operation.EndsWith:
    //                     break;
    //                 case Operation.NotEqualTo:
    //                     break;
    //                 case Operation.GreaterThan:
    //                     break;
    //                 case Operation.GreaterThanOrEqualTo:
    //                     break;
    //                 case Operation.LessThan:
    //                     break;
    //                 default:
    //                     throw new ArgumentOutOfRangeException();
    //             }
    // 	
    //             finalExpression = Expression.AndAlso(finalExpression, expression);
    //         }
    //
    //         return finalExpression;
    //     }
    //     
    //     MemberExpression GetMemberExpression(Expression param, string propertyName)
    //     {
    //         if (propertyName.Contains("."))
    //         {
    //             int index = propertyName.IndexOf(".", StringComparison.Ordinal);
    //             var subParam = Expression.Property(param, propertyName.Substring(0, index));
    //             return GetMemberExpression(subParam, propertyName.Substring(index + 1));
    //         }
    //
    //         return Expression.Property(param, propertyName);
    //     }
    //     
    // }
    //
    // /// <summary>
    // /// Defines a filter from which a expression will be built.
    // /// </summary>
    // public interface IFilter<TClass> where TClass : class
    // {
    //     /// <summary>
    //     /// Group of statements that compose this filter.
    //     /// </summary>
    //     List<IFilterStatement> Statements { get; set; }
    //     /// <summary>
    //     /// Builds a LINQ expression based upon the statements included in this filter.
    //     /// </summary>
    //     /// <returns></returns>
    //     Expression<Func<TClass, bool>> BuildExpression();
    // }

    /// <summary>
    /// Defines how a property should be filtered.
    /// </summary>
    public interface IFilterStatement
    {
        /// <summary>
        /// Establishes how this filter statement will connect to the next one.
        /// </summary>
        FilterStatementConnector Connector { get; set; }
        
        /// <summary>
        /// AggregateType of the property.
        /// </summary>
        string PropertyName { get; set; }
        /// <summary>
        /// Express the interaction between the property and the constant value 
        /// defined in this filter statement.
        /// </summary>
        Operation Operation { get; set; }
        /// <summary>
        /// Constant value that will interact with the property defined in this filter statement.
        /// </summary>
        object Value { get; set; }
    }

    public enum FilterStatementConnector { And, Or }
    
    public enum Operation
    {
        EqualTo,
        Contains,
        StartsWith,
        EndsWith,
        NotEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo
    }
}
