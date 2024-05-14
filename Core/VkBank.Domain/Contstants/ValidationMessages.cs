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
        //Menu
        public const string MenuIdEmpty = "Menu Id cannot be empty";
        public const string MenuNameEmpty = "Menu name cannot be empty.";
        public static readonly string MenuNameMinLength = "Menu name must be at least " + LengthLimits.MenuNameMinLength.ToString() + " characters long.";
        public static readonly string MenuNameMaxLength = "Menu name cannot exceed " + LengthLimits.MenuNameMaxLength.ToString() + " characters.";
    }
}
