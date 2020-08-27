using System;

namespace Sample.Contracts
{
    public interface OrderStatus
    {
        Guid Order { get; }
        string State { get; }
    }
}
