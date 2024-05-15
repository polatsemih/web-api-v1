using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkBank.Domain.Contstants
{
    public class LengthLimits
    {
        //Menu
        public const int MenuNameMinLength = 3;
        public const int MenuNameMaxLength = 500;
        public const int MenuKeywordMinLength = 3;
        public const int MenuKeywordMaxLength = 500;

        public const int MenuTypeMinRange = 1;
        public const int MenuTypeMaxRange = 255;
    }
}
