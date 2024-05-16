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
        public long ParentId { get; set; }
        public string Name_TR { get; set; }
        public string Name_EN { get; set; }
        public int ScreenCode { get; set; }
        public byte Type { get; set; }
        public int Priority { get; set; }
        public string Keyword { get; set; }
        public string? Icon { get; set; }
        public bool IsGroup { get; set; }
        public bool IsNew { get; set; }
        public DateTime? NewStartDate { get; set; }
        public DateTime? NewEndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommandRequest, IResult>
    {
        private readonly IMapper _mapper;
        private readonly CreateMenuValidator _validator;
        private readonly IMenuRepository _menuRepository;

        public CreateMenuCommandHandler(IMapper mapper, CreateMenuValidator validator, IMenuRepository menuRepository)
        {
            _mapper = mapper;
            _validator = validator;
            _menuRepository = menuRepository;
        }

        public Task<IResult> Handle(CreateMenuCommandRequest request, CancellationToken cancellationToken)
        {
            Menu menu = _mapper.Map<Menu>(request);
            var result = _validator.Validate(menu);
            if (result.IsValid)
            {
                _menuRepository.CreateMenu(menu);
                return Task.FromResult<IResult>(new SuccessResult(ResultMessages.MenuCreated));
            }

            return Task.FromResult<IResult>(new ErrorResult(result.Errors.First().ErrorMessage));







            //var result = _validator.ValidateAsync(menu, cancellationToken);
            //if (result.IsCompletedSuccessfully)
            //{
            //    _menuRepository.Create(menu);
            //    return Task.FromResult<IResult>(new SuccessResult(ResultMessages.MenuCreated));
            //}

            //return Task.FromResult<IResult>(new ErrorResult(result.Result.Errors.First().ErrorMessage));
        }
    }
}
