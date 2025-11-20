using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos.Dtos.Admin
{
    public class OperationResult
    {

        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public static OperationResult Ok(string message = "Ok") =>
            new OperationResult { Success = true, Message = message };

        public static OperationResult Fail(string message) =>
            new OperationResult { Success = false, Message = message };

    }
}
