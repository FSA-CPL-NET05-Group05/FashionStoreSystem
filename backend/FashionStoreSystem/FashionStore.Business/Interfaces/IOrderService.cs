using FashionStore.Business.Dtos;
using FashionStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Interfaces
{
    public interface IOrderService
    {
        Task<bool> PlaceOrderAsync(CheckoutDto dto);
    }
}
