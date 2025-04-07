using System;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.Pipelines
{
    public static class PipelineExtensions
    {
        public static PipelineBuilder WithTransientFaultCommandPipeline(this PipelineBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.Decorate(typeof(IDomainCommandHandler<>), typeof(TransientFaultDomainCommandPipeline<>));
            return builder;
        }

        public static PipelineBuilder WithTransientFaultEventPipeline(this PipelineBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.Decorate(typeof(IDomainEventHandler<>), typeof(TransientFaultDomainEventPipeline<>));
            return builder;
        }

        public static PipelineBuilder WithTransientFaultQueryPipeline(this PipelineBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.Decorate(typeof(IDomainQueryHandler<,>), typeof(TransientFaultDomainQueryPipeline<,>));
            return builder;
        }

        public static PipelineBuilder WithLoggerCommandPipeline(this PipelineBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.Decorate(typeof(IDomainCommandHandler<>), typeof(LoggerDomainCommandPipeline<>));
            return builder;
        }

        public static PipelineBuilder WithLoggerEventPipeline(this PipelineBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.Decorate(typeof(IDomainEventHandler<>), typeof(LoggerDomainEventPipeline<>));
            return builder;
        }

        public static PipelineBuilder WithLoggerQueryPipeline(this PipelineBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.Decorate(typeof(IDomainQueryHandler<,>), typeof(LoggerDomainQueryPipeline<,>));
            return builder;
        }

        public static PipelineBuilder AddCommandPipeline<TPipeline, TCommand>(this PipelineBuilder builder)
            where TPipeline : IDomainCommandHandler<TCommand>
            where TCommand : IDomainCommand
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.Decorate(typeof(IDomainCommandHandler<>), typeof(TPipeline));
            return builder;
        }

        public static PipelineBuilder AddEventPipeline<TPipeline, TEvent>(this PipelineBuilder builder)
            where TPipeline : IDomainEventHandler<TEvent>
            where TEvent : class, IDomainEvent
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.Decorate(typeof(IDomainEventHandler<>), typeof(TPipeline));
            return builder;
        }

        public static PipelineBuilder AddQueryPipeline<TPipeline, TQuery, TResult>(
            this PipelineBuilder builder)
            where TPipeline : IDomainQueryHandler<TQuery, TResult>
            where TQuery : IDomainQuery
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.Decorate(typeof(IDomainQueryHandler<,>), typeof(TPipeline));
            return builder;
        }
    }
}