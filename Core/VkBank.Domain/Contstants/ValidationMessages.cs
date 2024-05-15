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
        public const string MenuParentIdEmpty = "Menu Parent Id cannot be empty";
        public const string MenuTypeEmpty = "Menu Type cannot be empty";

        //public static readonly string MenuTypeRange = "Type must be between " + LengthLimits.MenuTypeMinRange.ToString() + " and " + LengthLimits.MenuTypeMaxRange.ToString() + ".";
        

        public const string MenuNameEmpty = "Menu name cannot be empty.";
        public static readonly string MenuNameMinLength = "Menu name must be at least " + LengthLimits.MenuNameMinLength.ToString() + " characters long.";
        public static readonly string MenuNameMaxLength = "Menu name cannot exceed " + LengthLimits.MenuNameMaxLength.ToString() + " characters.";

        public const string MenuKeywordEmpty = "Menu keyword cannot be empty.";
        public static readonly string MenuKeywordMinLength = "Menu keyword must be at least " + LengthLimits.MenuKeywordMinLength.ToString() + " characters long.";
        public static readonly string MenuKeywordMaxLength = "Menu keyword cannot exceed " + LengthLimits.MenuKeywordMaxLength.ToString() + " characters.";
    }
}
