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
        public const string IdEmpty = "Id cannot be empty";
        public const string IsActiveNull = "IsActive cannot be null";

        
        //Menu
        public const string MenuParentIdNull = "Menu ParentId cannot be null";
        public const string MenuParentIdPositiveOrZero = "Menu ParentId must be greater than or equal to 0";

        public const string MenuNameEmpty = "Menu Name cannot be empty";
        public static readonly string MenuNameMinLength = "Menu Name must be at least " + LengthLimits.MenuNameMinLength.ToString() + " characters long";
        public static readonly string MenuNameMaxLength = "Menu Name cannot exceed " + LengthLimits.MenuNameMaxLength.ToString() + " characters";

        public const string MenuScreenCodeEmpty = "Menu ScreenCode cannot be empty.";
        public static readonly string MenuScreenCodePositive = "Menu ScreenCode must be greater than " + LengthLimits.MenuScreenCodeMinRange.ToString();

        public const string MenuTypeEmpty = "Menu Type cannot be empty";

        public const string MenuPriorityNull = "Menu uPriority cannot be null";
        public const string MenuPriorityPositiveOrZero = "Menu Priority must be greater than or equal to 0";

        public const string MenuKeywordEmpty = "Menu Keyword cannot be empty";
        public static readonly string MenuKeywordMinLength = "Menu Keyword must be at least " + LengthLimits.MenuKeywordMinLength.ToString() + " characters long";
        public static readonly string MenuKeywordMaxLength = "Menu Keyword cannot exceed " + LengthLimits.MenuKeywordMaxLength.ToString() + " characters";

        public const string MenuIconEmpty = "Menu Icon cannot be empty";
        public static readonly string MenuIconMinLength = "Menu Icon must be at least " + LengthLimits.MenuIconMinLength.ToString() + " characters long";
        public static readonly string MenuIconMaxLength = "Menu Icon cannot exceed " + LengthLimits.MenuIconMaxLength.ToString() + " characters";

        public const string MenuIsGroupNull = "Menu IsGroup cannot be null";

        public const string MenuIsNewEmpty = "Menu IsNew cannot be empty";

        public const string MenuNewStartDateEmpty = "Menu NewStartDate cannot be empty";
        public const string MenuNewStartDateInvalid = "Menu NewStartDate is invalid";

        public const string MenuNewEndDateEmpty = "Menu NewStartDate cannot be empty";
        public const string MenuNewEndDateInvalid = "Menu NewEndDate is invalid";
        public const string MenuNewEndDateMustLater = "Menu NewEndDate must be later than the NewStartDate";
    }
}
