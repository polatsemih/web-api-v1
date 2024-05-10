using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkBank.Domain.Contstants
{
    public class LengthLimits
    {
        public const int MenuNameMinLength = 3;
        public const int MenuNameMaxLength = 255;



        public static readonly string MenuNameMaxLength3 = MenuNameMaxLength.ToString();
    }
}
