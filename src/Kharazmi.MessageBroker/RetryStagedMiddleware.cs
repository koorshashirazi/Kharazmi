using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using RawRabbit.Common;
using RawRabbit.Pipe;
using RawRabbit.Pipe.Middleware;

namespace Kharazmi.MessageBroker
{
    internal class RetryStagedMiddleware : StagedMiddleware
    {
        public override string StageMarker { get; } = RawRabbit.Pipe.StageMarker.MessageDeserialized;

        public override async Task InvokeAsync(IPipeContext context,
            CancellationToken token = new CancellationToken())
        {
            var retry = context.GetRetryInformation();
            if (context.GetMessageContext() is DomainContext message)
            {
                message.UpdateRetrying(retry.NumberOfRetries);
            }

            await Next.InvokeAsync(context, token).ConfigureAwait(false);
        }
    }
}