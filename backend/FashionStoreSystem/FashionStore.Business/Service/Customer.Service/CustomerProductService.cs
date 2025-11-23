using FashionStore.Business.Dtos;
using FashionStore.Business.Interfaces;
using FashionStore.Business.Interfaces.Interfaces.Customer;
using FashionStore.Business.Mapping;
using FashionStore.Data.Interfaces;
using FashionStore.Data.Interfaces.Interfaces.Customer;
using FashionStore.Data.Models;
using FashionStore.Shared.Shared.Admin;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FashionStore.Business.Service
{
    public class CustomerProductService : ICustomerProductService
    {
        private readonly ICustomerProductRepository _productRepository;

        public CustomerProductService(ICustomerProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        

        // Phương thức phân trang và trả về ProductDTO
        public async Task<PagedResult<ProductDTO>> GetProductsAsync(string? searchTerm, int? categoryId, string? sortOrder, int pageNumber, int pageSize, CancellationToken ct)
        {
            // Nếu pageNumber hoặc pageSize là null, gán giá trị mặc định
            pageNumber = pageNumber == 0 ? 1 : pageNumber;  // Gán 1 nếu pageNumber là 0
            pageSize = pageSize == 0 ? 10 : pageSize;       // Gán 10 nếu pageSize là 0

            // Chuyển đổi sortOrder thành bool sortByPriceAsc
            bool? sortByPriceAsc = null; // Mặc định là null (không sắp xếp)
            if (string.Equals(sortOrder, "price-low", StringComparison.OrdinalIgnoreCase))
            {
                sortByPriceAsc = true;  // Nếu sortOrder là "price-low", sắp xếp từ thấp đến cao
            }
            else if (string.Equals(sortOrder, "price-high", StringComparison.OrdinalIgnoreCase))
            {
                sortByPriceAsc = false; // Nếu sortOrder là "price-high", sắp xếp từ cao đến thấp
            }

            // Lấy danh sách sản phẩm từ Repository với phân trang
            var products = await _productRepository.GetProductsAsync(searchTerm, categoryId, sortByPriceAsc, pageNumber, pageSize, ct);

            // Kiểm tra nếu không có sản phẩm nào (null hoặc danh sách rỗng)
            if (products == null || products.Items == null || products.Items.Count == 0)
            {
                // Trả về PagedResult rỗng nếu không có sản phẩm
                return new PagedResult<ProductDTO>
                {
                    TotalCount = 0,
                    Page = pageNumber,
                    PageSize = pageSize,
                    Items = new List<ProductDTO>(),  // Trả về danh sách rỗng
                };
            }

            // Mapping từ Product sang ProductDTO
            var productDTOs = products.Items.Select(p => p.ToProductDTO()).ToList();

            return new PagedResult<ProductDTO>
            {
                TotalCount = products.TotalCount,
                Page = products.Page,
                PageSize = products.PageSize,
                Items = productDTOs
            };
        }

    }
}
