﻿using MediatR;
using Microsoft.IdentityModel.Tokens;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Application.Validations.Menu;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Results;
using SUPBank.Domain.Results.Data;
using System.Collections.Generic;

namespace SUPBank.Application.Features.Menu.Queries
{
    public class GetMenuByIdWithSubMenusQueryRequest : IRequest<IDataResult<List<EntityMenu>>>
    {
        public long Id { get; set; }
    }

    public class GetMenuByIdWithSubMenusQueryHandler : IRequestHandler<GetMenuByIdWithSubMenusQueryRequest, IDataResult<List<EntityMenu>>>
    {
        private readonly GetMenuByIdWithSubMenusValidator _validator;
        private readonly IMenuQueryRepository _menuQueryRepository;

        public GetMenuByIdWithSubMenusQueryHandler(GetMenuByIdWithSubMenusValidator validator, IMenuQueryRepository menuQueryRepository)
        {
            _validator = validator;
            _menuQueryRepository = menuQueryRepository;
        }

        public async Task<IDataResult<List<EntityMenu>>> Handle(GetMenuByIdWithSubMenusQueryRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new ErrorDataResult<List<EntityMenu>>(string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage)), new List<EntityMenu>());
            }

            var result = await _menuQueryRepository.GetMenuByIdWithSubMenusAsync(request.Id, cancellationToken);
            if (result.IsNullOrEmpty())
            {
                return new ErrorDataResult<List<EntityMenu>>(ResultMessages.MenuNoData, new List<EntityMenu>());
            }
            return new SuccessDataResult<List<EntityMenu>>(result.ToList());
        }
    }
}
