namespace SUPBank.Domain.Contstants
{
    public class ValidationMessages
    {
        public const string IdEmpty = "Id cannot be empty";
        public const string IsActiveNull = "IsActive cannot be null";
        public const string RollbackTokenEmpty = "RollbackToken cannot be empty";


        // Menu
        public const string MenuParentIdNull = "Menu ParentId cannot be null";
        public const string MenuParentIdPositiveOrZero = "Menu ParentId must be greater than or equal to 0";

        public const string MenuNameEmpty = "Menu Name cannot be empty";
        public const string MenuNameMinLength = "Menu Name must be at least {0} characters long";
        public const string MenuNameMaxLength = "Menu Name cannot exceed {0} characters";

        public const string MenuScreenCodeEmpty = "Menu ScreenCode cannot be empty.";
        public const string MenuScreenCodeMinRange = "Menu ScreenCode must be greater than {0}";

        public const string MenuTypeEmpty = "Menu Type cannot be empty";

        public const string MenuPriorityNull = "Menu Priority cannot be null";
        public const string MenuPriorityPositiveOrZero = "Menu Priority must be greater than or equal to 0";

        public const string MenuKeywordEmpty = "Menu Keyword cannot be empty";
        public const string MenuKeywordMinLength = "Menu Keyword must be at least {0} characters long";
        public const string MenuKeywordMaxLength = "Menu Keyword cannot exceed {0} characters";

        public const string MenuIconEmpty = "Menu Icon cannot be empty";
        public const string MenuIconMinLength = "Menu Icon must be at least {0} characters long";
        public const string MenuIconMaxLength = "Menu Icon cannot exceed {0} characters";

        public const string MenuIsGroupNull = "Menu IsGroup cannot be null";

        public const string MenuIsNewNull = "Menu IsNew cannot be null";

        public const string MenuNewStartDateEmpty = "Menu NewStartDate cannot be empty";
        public const string MenuNewStartDateInvalid = "Menu NewStartDate is invalid";

        public const string MenuNewEndDateEmpty = "Menu NewStartDate cannot be empty";
        public const string MenuNewEndDateInvalid = "Menu NewEndDate is invalid";
        public const string MenuNewEndDateMustLater = "Menu NewEndDate must be later than the NewStartDate";
    }
}
