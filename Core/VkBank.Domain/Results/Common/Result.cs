using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkBank.Domain.Results.Common
{
    public class Result : IResult
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public Result(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public Result(bool success)
        {
            IsSuccess = success;
            Message = string.Empty;
        }
    }
}
