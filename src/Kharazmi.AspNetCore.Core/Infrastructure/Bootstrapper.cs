﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Infrastructure.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.Infrastructure
{
    public static class Bootstrapper
    {
        public static void RunOnStartup(IServiceProvider resolver)
        {
            using (var scope = resolver.CreateScope())
            {
                var tasksToRun = scope.ServiceProvider.GetServices<IStartupTask>().ToList();

                foreach (var task in tasksToRun)
                {
                    task.Execute();
                }
            }
        }

        public static async Task RunOnEnd(IServiceProvider resolver)
        {
            using (var scope = resolver.CreateScope())
            {
                var tasksToRun = scope.ServiceProvider.GetServices<IEndTask>().ToList();

                foreach (var task in tasksToRun)
                {
                    await task.ExecuteAsync().ConfigureAwait(false);
                }
            }
        }

        public static async Task RunOnError(IServiceProvider resolver, Exception exception)
        {
            using (var scope = resolver.CreateScope())
            {
                var tasksToRun = scope.ServiceProvider.GetServices<IErrorTask>().ToList();

                foreach (var task in tasksToRun)
                {
                    await task.ExecuteAsync(exception).ConfigureAwait(false);
                }
            }
        }

        public static async Task RunOnBeginRequest(IServiceProvider resolver)
        {
            using (var scope = resolver.CreateScope())
            {
                var tasksToRun = scope.ServiceProvider.GetServices<IBeginRequestTask>().ToList();

                foreach (var task in tasksToRun)
                {
                    await task.ExecuteAsync().ConfigureAwait(false);
                }
            }
        }

        public static async Task RunOnEndRequest(IServiceProvider resolver)
        {
            using (var scope = resolver.CreateScope())
            {
                var tasksToRun = scope.ServiceProvider.GetServices<IEndRequestTask>().ToList();

                foreach (var task in tasksToRun)
                {
                    await task.ExecuteAsync().ConfigureAwait(false);
                }
            }
        }
    }
}