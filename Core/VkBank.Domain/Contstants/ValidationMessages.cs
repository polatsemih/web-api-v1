using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkBank.Domain.Contstants;

namespace VkBank.Domain.Contstants
{
    public class ValidationMessages
    {
        public const string MenuNameEmptyError = "Menu name cannot be empty.";
        //public const string MenuNameMinLengthError = string.Format("{0} {1} {2}", "aaaa", LengthLimits.MenuNameMinLength.ToString(), "bbbbb");
        //public readonly string MenuNameMinLengthError = "Menu name must be at least " + LengthLimits.MenuNameMinLength.ToString() + " characters long.";
        public const string MenuNameMaxLengthError = "Menu name cannot exceed 255 characters.";

        public static readonly string MenuNameMinLengthError = LengthLimits.MenuNameMaxLength.ToString();
    }
}
