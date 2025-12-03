using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos.Dtos.Customer
{
    public  class ProductVarianDTO
    {
        public int SizeId { get; set; }
        public string SizeName { get; set; }

        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }

        public int Stock { get; set; }
    }
}
