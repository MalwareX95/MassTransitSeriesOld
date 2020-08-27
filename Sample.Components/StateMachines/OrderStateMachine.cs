using Automatonymous;
using MassTransit;
using MassTransit.RedisIntegration;
using Sample.Contracts;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Sample.Components.StateMachines
{
    public class OrderState : 
        SagaStateMachineInstance, 
        IVersionedSaga
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public DateTime? Updated { get; set; }
        public string CustomerNumber { get; set; }
        public int Version { get; set; }
        public DateTime? SubmitDate { get;   set; }
    }

    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderStatusRequested, x =>
            {
                x.CorrelateById(m => m.Message.OrderId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context => 
                {
                    if (context.RequestId.HasValue)
                    {
                        await context.RespondAsync<OrderNotFound>(new { context.Message.OrderId });
                    }
                }));
            });


            InstanceState(x => x.CurrentState);

            Initially(
                When(OrderSubmitted)
                    .Then(context =>
                    {
                        context.Instance.CustomerNumber = context.Data.CustomerNumber;
                        context.Instance.Updated = DateTime.UtcNow;
                        context.Instance.SubmitDate = context.Data.Timestamp;
                    })
                    .TransitionTo(Submitted));

            During(Submitted,
                Ignore(OrderSubmitted));

            DuringAny(
                When(OrderSubmitted)
                    .Then(context => 
                    {
                        context.Instance.SubmitDate ??= context.Data.Timestamp;
                        context.Instance.CustomerNumber ??= context.Data.CustomerNumber;
                    }));

            DuringAny(
                When(OrderStatusRequested)
                    .RespondAsync(x => x.Init<OrderStatus>(new
                    {
                        Order = x.Instance.CorrelationId,
                        State = x.Instance.CurrentState
                    }))
                ) ;
        }

        public State Submitted { get; private set; }

        public Event<OrderSubmitted> OrderSubmitted { get; private set; }

        public Event<CheckOrder> OrderStatusRequested { get; private set; }
    }


}
