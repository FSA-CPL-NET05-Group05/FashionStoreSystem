using FashionStore.Business.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Interfaces
{
    public interface IHomeService
    {
        Task<IEnumerable<HomeProductDto>> GetTopRatedProductsAsync();
    }
}
