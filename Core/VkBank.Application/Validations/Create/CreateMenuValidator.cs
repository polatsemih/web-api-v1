using FluentValidation;
using VkBank.Application.Validations.Common;
using VkBank.Domain.Entities;

namespace VkBank.Application.Validations.Create
{
    public class CreateMenuValidator : AbstractValidator<Menu>
    {
        public CreateMenuValidator()
        {
            RuleFor(m => m.ParentId).ValidateMenuParentId();
            RuleFor(m => m.Name_TR).ValidateMenuName();
            RuleFor(m => m.Name_EN).ValidateMenuName();
            RuleFor(m => m.ScreenCode).ValidateMenuScreenCode();
            RuleFor(m => m.Type).ValidateMenuType();
            RuleFor(m => m.Priority).ValidateMenuPriority();
            RuleFor(m => m.Keyword).ValidateMenuKeyword();
            RuleFor(m => m.Icon).ValidateMenuIcon();
            RuleFor(m => m.IsGroup).ValidateMenuIsGroup();
            RuleFor(m => m.IsNew).ValidateMenuIsNew();
            RuleFor(m => m.NewStartDate).ValidateMenuNewStartDate();
            RuleFor(m => m.NewEndDate).ValidateMenuNewEndDate(m => m.NewStartDate);
            RuleFor(m => m.IsActive).ValidateIsActive();
        }
    }
}
