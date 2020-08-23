﻿using MassTransit;
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
            logger.LogDebug("");
            if (context.Message.CustomerNumber.Contains("TEST"))
            {

            }
            await context.RespondAsync<OrderSubmissionAccepted>(new
            {
                context.Message.OrderId,
                Timestamp = InVar.Timestamp,
                context.Message.CustomerNumber
            });
        }
    }
}
