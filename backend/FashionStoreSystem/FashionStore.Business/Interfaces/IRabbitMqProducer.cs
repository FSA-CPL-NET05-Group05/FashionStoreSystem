using FashionStore.Business.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Interfaces
{
    public interface IRabbitMqProducer
    {
        Task PublishOrderAsync(OrderMessage message);
    }
}
