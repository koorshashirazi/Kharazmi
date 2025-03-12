using System;
using System.Linq;
using System.Linq.Expressions;

namespace Kharazmi.AspNetCore.Core.Linq;

/// <summary>
/// Partial evaluator for Expressions that evaluates and replaces constant expressions
/// </summary>
public class PartialEvaluator(Func<Expression, bool> canEvaluate) : ExpressionVisitor
{
    public PartialEvaluator() : this(CanEvaluateLocally)
    {
    }

    public Expression Evaluate(Expression expression)
    {
        return Visit(expression);
    }

    public override Expression? Visit(Expression? node)
    {
        if (node is null) return null;

        return canEvaluate(node) ? EvaluateAndConvertToConstant(node) : base.Visit(node);
    }

    private static bool CanEvaluateLocally(Expression expr)
    {
        Expression? expression = expr;
        while (true)
        {
            // If it is a parameter, it cannot be evaluated
            if (expression is ParameterExpression) return false;

            // Check for MemberExpressions
            if (expression is MemberExpression memberExpr)
            {
                if (memberExpr.Expression is ParameterExpression) return false;

                // If this is a member of a parameter (like x.Name), it cannot be evaluated
                expression = memberExpr.Expression;
                continue;
            }

            // Check for MethodCalls
            if (expression is MethodCallExpression methodExpr)
            {
                // Cannot be evaluated if the Instance method is a parameter
                if (methodExpr.Object is ParameterExpression) return false;

                // Check the Instance method and all arguments
                if (methodExpr.Object != null && !CanEvaluateLocally(methodExpr.Object)) return false;

                return methodExpr.Arguments.All(CanEvaluateLocally);
            }

            // Check subexpressions in binary expressions
            if (expression is BinaryExpression binaryExpr)
            {
                return CanEvaluateLocally(binaryExpr.Left) && CanEvaluateLocally(binaryExpr.Right);
            }

            // Default: Evaluates if it contains no parameters
            return !HasParameterExpression(expression);
        }
    }

    private static bool HasParameterExpression(Expression? expression)
    {
        if (expression is null) return false;
        var parameterFinder = new ParameterFinderVisitor();
        parameterFinder.Visit(expression);
        return parameterFinder.HasParameter;
    }

    private static ConstantExpression EvaluateAndConvertToConstant(Expression expression)
    {
        var lambdaExpr = Expression.Lambda(expression);
        var compiledLambda = lambdaExpr.Compile();
        var result = compiledLambda.DynamicInvoke();

        return Expression.Constant(result, expression.Type);
    }

    private class ParameterFinderVisitor : ExpressionVisitor
    {
        public bool HasParameter { get; private set; }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            HasParameter = true;
            return base.VisitParameter(node);
        }
    }
}

public static partial class ExpressionExtensions
{
    public static Expression<T> PartiallyEvaluate<T>(this Expression<T> expression)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        var evaluator = new PartialEvaluator();
        var evaluatedBody = evaluator.Evaluate(expression.Body);
        return Expression.Lambda<T>(evaluatedBody, expression.Parameters);
    }
}