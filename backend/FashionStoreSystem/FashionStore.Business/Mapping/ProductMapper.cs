using FashionStore.Business.Dtos;
using FashionStore.Business.Dtos.Dtos.Customer;
using FashionStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Mapping
{
    public static class ProductMapper
    {
        // Map tuừ Product sang ProductDTO để trả về cho client 
        public static ProductDTO ToProductDTO(this Product product)
        {
            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl
            };
        }

        public static ProductDetailDTO ToProductDetailDTO(this Product product)
        {
            return new ProductDetailDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryName = product.Category.Name,
                TotalStock = product.ProductSizes.Sum(ps => ps.Stock),

                Images = product.Images.Select(img => img.Url).ToList(),
                                              

                Variants = product.ProductSizes.Select(ps => new ProductVarianDTO
                {
                    SizeId = ps.SizeId,
                    SizeName = ps.Size.Name,
                    ColorId = ps.ColorId,
                    ColorName = ps.Color.Name,
                    Stock = ps.Stock
                }).ToList()
            };
        }
    }
}
