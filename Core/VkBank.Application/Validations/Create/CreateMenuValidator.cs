using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkBank.Application.Validations.Common;
using VkBank.Domain.Entities;

namespace VkBank.Application.Validations.Create
{
    public class CreateMenuValidator : AbstractValidator<Menu>
    {
        public CreateMenuValidator()
        {
            RuleFor(m => m.Name).CheckMenuName();
        }
    }
}
