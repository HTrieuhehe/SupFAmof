using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SupFAmof.Service.DTO.Response
{
    public class BaseResponseViewModel<T>
    {
        public StatusViewModel Status { get; set; }
        public T Data { get; set; }
    }

    public class StatusViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
    }
}
