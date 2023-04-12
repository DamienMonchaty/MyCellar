using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCellar.API.Utils
{
    public class CustomResponse<T>
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public T Result { get; set; }
    }
}
