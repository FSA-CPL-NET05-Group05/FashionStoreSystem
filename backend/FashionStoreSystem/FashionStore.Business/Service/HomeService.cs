using AutoMapper;
using FashionStore.Business.Dtos;
using FashionStore.Business.Interfaces;
using FashionStore.Data.Interfaces;
using FashionStore.Data.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Service
{
    public class HomeService : IHomeService
    {
        private readonly IHomeRepository _homeRepo;
        private readonly IMapper _mapper;
        private const int DEFAULT_PRODUCT_COUNT = 4;

        public HomeService(IHomeRepository homeRepo,IMapper mapper)
        {
            _homeRepo = homeRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<HomeProductDto>> GetTopRatedProductsAsync()
        {
            var homeProducts = await _homeRepo.GetTopRatedProductsAsync(DEFAULT_PRODUCT_COUNT);
            var result = _mapper.Map<IEnumerable<HomeProductDto>>(homeProducts); ;
            return result;
        }
    }
}
