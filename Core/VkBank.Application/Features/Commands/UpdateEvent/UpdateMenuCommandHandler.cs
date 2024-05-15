using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Update;
using VkBank.Domain.Common.Result;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;

namespace VkBank.Application.Features.Commands.UpdateEvent
{
    public class UpdateMenuCommandRequest : IRequest<IResult>
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string Name_TR { get; set; }
        public string Name_EN { get; set; }
        public byte Type { get; set; }
        public byte Priority { get; set; }
        public string? Keyword { get; set; }
        public byte? Icon { get; set; }
        public bool IsGroup { get; set; } = false;
        public bool IsNew { get; set; } = true;
        public DateTime NewStartDate { get; set; } = DateTime.Now;
        public DateTime NewEndDate { get; set; } = DateTime.Now.AddDays(7);
        public bool IsActive { get; set; } = true;
    }

    public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommandRequest, IResult>
    {
        private readonly IMapper _mapper;
        private readonly UpdateMenuValidator _validator;
        private readonly IMenuRepository _menuRepository;

        public UpdateMenuCommandHandler(IMapper mapper, UpdateMenuValidator validator, IMenuRepository menuRepository)
        {
            _mapper = mapper;
            _validator = validator;
            _menuRepository = menuRepository;
        }

        public Task<IResult> Handle(UpdateMenuCommandRequest request, CancellationToken cancellationToken)
        {
            Menu menu = _mapper.Map<Menu>(request);
            menu.LastModifiedDate = DateTime.Now;

            var result = _validator.ValidateAsync(menu, cancellationToken);
            if (result.IsCompletedSuccessfully)
            {
                _menuRepository.Update(menu);
                return Task.FromResult<IResult>(new SuccessResult(ResultMessages.MenuUpdated));
            }

            return Task.FromResult<IResult>(new ErrorResult(result.Result.Errors.First().ErrorMessage));
        }
    }
}
