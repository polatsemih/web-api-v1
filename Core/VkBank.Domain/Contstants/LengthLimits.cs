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
        public const int MenuNameMinLength = 2;
        public const int MenuNameMaxLength = 500;

        public const int MenuScreenCodeMinRange = 10000;

        public const int MenuKeywordMinLength = 2;
        public const int MenuKeywordMaxLength = 500;

        public const int MenuIconMinLength = 2;
        public const int MenuIconMaxLength = 500;
    }
}
