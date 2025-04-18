﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.ReflectionToolkit;
using Kharazmi.AspNetCore.Core.Threading;
using Kharazmi.AspNetCore.Core.Transaction;
using Kharazmi.AspNetCore.EFCore.Context;
using Microsoft.EntityFrameworkCore;

namespace Kharazmi.AspNetCore.EFCore.Transaction
{
    public sealed class TransactionInterceptor<TDbContext> : IInterceptor where TDbContext : DbContext
    {
        private readonly IUnitOfWork<TDbContext> _uow;

        public TransactionInterceptor(IUnitOfWork<TDbContext> uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public void Intercept(IInvocation invocation)
        {
            MethodInfo method;
            try
            {
                method = invocation.MethodInvocationTarget;
            }
            catch
            {
                method = invocation.GetConcreteMethod();
            }

            var attribute = FindTransactionalAttribute(method);

            //If there is a running transaction, just run the method
            if (!attribute.HasValue || _uow.HasTransaction)
            {
                invocation.Proceed();
                return;
            }

            Intercept(invocation, attribute.Value);
        }

        private void Intercept(IInvocation invocation, TransactionalAttribute attribute)
        {
            if (invocation.Method.IsAsync())
                InterceptAsync(invocation, attribute);
            else
                InterceptSync(invocation, attribute);
        }

        private static Maybe<TransactionalAttribute> FindTransactionalAttribute(MemberInfo methodInfo)
        {
            return ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<TransactionalAttribute>(
                methodInfo);
        }

        private void InterceptAsync(IInvocation invocation, TransactionalAttribute attribute)
        {
            _uow.BeginTransaction(attribute.IsolationLevel);

            try
            {
                invocation.Proceed();
            }
            catch (Exception)
            {
                _uow.RollbackTransaction();
                throw;
            }

            if (invocation.Method.ReturnType == typeof(Task))
                invocation.ReturnValue = InterceptAsync((Task) invocation.ReturnValue, _uow);
            else //Task<TResult>
                invocation.ReturnValue = typeof(TransactionInterceptor<TDbContext>)
                    .GetMethod(nameof(InterceptWitResultAsync),
                        BindingFlags.NonPublic | BindingFlags.Static)
                    ?.MakeGenericMethod(invocation.Method.ReturnType.GenericTypeArguments[0])
                    .Invoke(null, new[] {invocation.ReturnValue, _uow});
        }

        private static async Task InterceptAsync(Task task, IUnitOfWork uow)
        {
            try
            {
                await task.ConfigureAwait(false);
                uow.CommitTransaction();
            }
            catch (Exception)
            {
                uow.RollbackTransaction();
                throw;
            }
        }

        private static async Task<T> InterceptWitResultAsync<T>(Task<T> task, IUnitOfWork uow)
        {
            try
            {
                var result = await task.ConfigureAwait(false);
                if (result is Result returnValue && returnValue.Failed)
                    uow.RollbackTransaction();
                else
                    uow.CommitTransaction();

                return result;
            }
            catch (Exception)
            {
                uow.RollbackTransaction();
                throw;
            }
        }

        private void InterceptSync(IInvocation invocation, TransactionalAttribute attribute)
        {
            _uow.BeginTransaction(attribute.IsolationLevel);
            try
            {
                invocation.Proceed();

                if (invocation.ReturnValue is Result returnValue && returnValue.Failed)
                    _uow.RollbackTransaction();
                else
                    _uow.CommitTransaction();
            }
            catch (Exception)
            {
                _uow.RollbackTransaction();
                throw;
            }
        }
    }
}