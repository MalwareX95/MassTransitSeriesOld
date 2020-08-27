using MassTransit;
using Microsoft.Extensions.Logging;
using Sample.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Components.Consumers
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        private readonly ILogger<SubmitOrderConsumer> logger;

        public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
        {
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            logger.LogDebug("SubmitOrderConsumer: {CustomerNumber}", context.Message.CustomerNumber);
            if (context.Message.CustomerNumber.Contains("TEST"))
            {
                if (context.RequestId != null)
                {
                    await context.RespondAsync<OrderSubmissionRejected>(new
                    {
                        context.Message.OrderId,
                        Timestamp = InVar.Timestamp,
                        context.Message.CustomerNumber,
                        Reason = $"Test Customer cannot submit orders: {context.Message.CustomerNumber}"
                    });
                }

                return;
            }

            await context.Publish<OrderSubmitted>(new
            {
                context.Message.OrderId,
                context.Message.CustomerNumber,
                context.Message.Timestamp
            });

            if (context.RequestId != null)
            {
                await context.RespondAsync<OrderSubmissionAccepted>(new
                {
                    context.Message.OrderId,
                    context.Message.CustomerNumber,
                    Timestamp = InVar.Timestamp,
                });
            }
        }
    }
}
