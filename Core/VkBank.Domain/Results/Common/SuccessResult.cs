using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkBank.Domain.Results.Common
{
    public class SuccessResult : Result
    {
        public SuccessResult(string message) : base(true, message)
        {

        }
    }
}
