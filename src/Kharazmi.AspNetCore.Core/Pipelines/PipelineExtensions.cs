using Kharazmi.AspNetCore.Core.Domain.Commands;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.Domain.Queries;
using Kharazmi.AspNetCore.Core.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.Pipelines
{
    public static class PipelineExtensions
    {
        public static PipelineBuilder WithTransientFaultCommandPipeline(
            this PipelineBuilder builder, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            builder.Services.Decorate(typeof(ICommandHandler<>), typeof(TransientFaultCommandPipeline<>));
            return builder;
        }

        public static PipelineBuilder WithTransientFaultEventPipeline(
            this PipelineBuilder builder, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            builder.Services.Decorate(typeof(IEventHandler<>), typeof(TransientFaultEventPipeline<>));
            return builder;
        }

        public static PipelineBuilder WithTransientFaultQueryPipeline(
            this PipelineBuilder builder, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            builder.Services.Decorate(typeof(IQueryHandler<,>), typeof(TransientFaultQueryPipeline<,>));
            return builder;
        }

        public static PipelineBuilder WithLoggerCommandPipeline(
            this PipelineBuilder builder, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            builder.Services.Decorate(typeof(ICommandHandler<>), typeof(LoggerCommandPipeline<>));
            return builder;
        }

        public static PipelineBuilder WithLoggerEventPipeline(
            this PipelineBuilder builder, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            builder.Services.Decorate(typeof(IEventHandler<>), typeof(LoggerEventPipeline<>));
            return builder;
        }

        public static PipelineBuilder WithLoggerQueryPipeline(
            this PipelineBuilder builder, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            builder.Services.Decorate(typeof(IQueryHandler<,>), typeof(LoggerQueryPipeline<,>));
            return builder;
        }

        public static PipelineBuilder AddCommandPipeline<TPipeline, TCommand>(
            this PipelineBuilder builder, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            where TPipeline : ICommandHandler<TCommand>
            where TCommand : ICommand
        {
            builder.Services.Decorate(typeof(ICommandHandler<>), typeof(TPipeline));
            return builder;
        }

        public static PipelineBuilder AddEventPipeline<TPipeline, TEvent>(
            this PipelineBuilder builder, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            where TPipeline : IEventHandler<TEvent>
            where TEvent : class, IDomainEvent
        {
            builder.Services.Decorate(typeof(IEventHandler<>), typeof(TPipeline));
            return builder;
        }

        public static PipelineBuilder AddQueryPipeline<TPipeline, TQuery, TResult>(
            this PipelineBuilder builder, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            where TPipeline : IQueryHandler<TQuery, TResult>
            where TQuery : IQuery<TResult>
        {
            builder.Services.Decorate(typeof(IQueryHandler<,>), typeof(TPipeline));
            return builder;
        }
    }
}