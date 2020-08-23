﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Contracts
{
    public interface SubmitOrder
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }
    }

}
