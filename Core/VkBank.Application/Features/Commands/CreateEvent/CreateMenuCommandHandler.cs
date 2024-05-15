using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Create;
using VkBank.Domain.Common.Result;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;

namespace VkBank.Application.Features.Commands.CreateEvent
{
    public class CreateMenuCommandRequest : IRequest<IResult>
    {
        public long? ParentId { get; set; }
        public required string Name_TR { get; set; }
        public required string Name_EN { get; set; }
        public required byte Type { get; set; }
        public required byte Priority { get; set; }
        public required string Keyword { get; set; }
        public byte? Icon { get; set; }
        public bool IsGroup { get; set; } = false;
        public bool IsNew { get; set; } = true;
        public DateTime NewStartDate { get; set; } = DateTime.Now;
        public DateTime NewEndDate { get; set; } = DateTime.Now.AddDays(7);
        public bool IsActive { get; set; } = true;
        public int MyProperty { get; set; }
    }

    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommandRequest, IResult>
    {
        private readonly IMapper _mapper;
        private readonly CreateMenuValidator _validator;
        private readonly IMenuRepository _menuRepository;

        public CreateMenuCommandHandler(IMenuRepository menuRepository, IMapper mapper, CreateMenuValidator validator)
        {
            _mapper = mapper;
            _validator = validator;
            _menuRepository = menuRepository;
        }

        public Task<IResult> Handle(CreateMenuCommandRequest request, CancellationToken cancellationToken)
        {
            Menu menu = _mapper.Map<Menu>(request);
            var result = _validator.ValidateAsync(menu, cancellationToken);
            if (result.IsCompletedSuccessfully)
            {
                _menuRepository.Add(menu);
                return Task.FromResult<IResult>(new SuccessResult(ResultMessages.MenuCreated));
            }

            return Task.FromResult<IResult>(new ErrorResult(result.Result.Errors.First().ErrorMessage));
        }
    }
}
