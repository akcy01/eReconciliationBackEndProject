using Core.Utilities.Results.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Results.Concrete
{
    public class Result : IResult
    {
        /* Geriye sonuç döndürdüğümüzde ya sadece successi ya da messageyi veya ikisini bir döndürebiliriz diye iki adet constructor oluşturduk. */
        public Result(bool success)
        {
            Success = success;
        }
        public Result(bool success, string message) : this(success)
        {
            Message = message;
        }

        public bool Success { get; }
        public string Message { get; }
    }
}
