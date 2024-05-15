using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Delete;
using VkBank.Domain.Common.Result;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;

namespace VkBank.Application.Features.Commands.DeleteEvent
{
    public class DeleteMenuCommandRequest : IRequest<IResult>
    {
        public long Id { get; set; }
    }

    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommandRequest, IResult>
    {
        private readonly IMapper _mapper;
        private readonly DeleteMenuValidator _validator;
        private readonly IMenuRepository _menuRepository;

        public DeleteMenuCommandHandler(IMenuRepository menuRepository, IMapper mapper, DeleteMenuValidator validator)
        {
            _mapper = mapper;
            _validator = validator;
            _menuRepository = menuRepository;
        }

        public Task<IResult> Handle(DeleteMenuCommandRequest request, CancellationToken cancellationToken)
        {
            Menu menu = _mapper.Map<Menu>(request);

            var result = _validator.ValidateAsync(menu, cancellationToken);
            if (result.IsCompletedSuccessfully)
            {
                _menuRepository.Delete(menu);
                return Task.FromResult<IResult>(new SuccessResult(ResultMessages.MenuDeleted));
            }

            return Task.FromResult<IResult>(new ErrorResult(result.Result.Errors.First().ErrorMessage));
        }
    }
}
