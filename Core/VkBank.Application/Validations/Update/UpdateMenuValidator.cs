using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkBank.Application.Validations.Common;
using VkBank.Domain.Entities;

namespace VkBank.Application.Validations.Update
{
    public class UpdateMenuValidator : AbstractValidator<Menu>
    {
        public UpdateMenuValidator()
        {
            RuleFor(m => m.Id).ValidateMenuId();
            RuleFor(m => m.Name).ValidateMenuName();
        }
    }
}
