using System;
using System.Linq.Expressions;
using System.Reflection;
using Kharazmi.AspNetCore.Core.Linq;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    public static partial class Core
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Combine<T>(this Expression<Func<T, bool>> expression1,
            Expression<Func<T, bool>> expression2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expression1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expression2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(left ?? throw new InvalidOperationException(nameof(left)),
                    right ?? throw new InvalidOperationException(nameof(right))), parameter);
        }


        /// <summary>
        /// Return object value of Expresion body
        /// </summary>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetObjectValue<T>(this Expression<Func<T, bool>> predicate) where T : class
        {
            BinaryExpression eq = (BinaryExpression) predicate.Body;
            MemberExpression rightEq = (MemberExpression) eq.Right;
            MemberExpression rightEqExpression = (MemberExpression) rightEq.Expression;
            ConstantExpression captureConst = (ConstantExpression) rightEqExpression.Expression;
            return (T) ((FieldInfo) rightEqExpression.Member).GetValue(captureConst.Value);
        }
    }
}