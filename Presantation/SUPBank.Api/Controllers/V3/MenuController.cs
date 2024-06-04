using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using MediatR;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Responses.Data;
using SUPBank.Domain.Responses;
using SUPBank.Application.Interfaces.Services;
using SUPBank.Application.Features.Menu.Queries.Requests;
using SUPBank.Application.Features.Menu.Commands.Requests;
using SUPBank.Application.Interfaces.Services.Controllers;

namespace SUPBank.Api.Controllers.V3
{
    [ApiVersion("3.0")]
    [Route("api/v{version:apiVersion}/menu")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IValidationService _validationService;
        private readonly IMediator _mediator;
        private readonly IRedisCacheService _cacheService;
        private readonly IMenuService _menuService;

        public MenuController(IValidationService validationService, IMediator mediator, IRedisCacheService cacheService, IMenuService menuService)
        {
            _validationService = validationService;
            _mediator = mediator;
            _cacheService = cacheService;
            _menuService = menuService;
        }

        /// <summary>
        /// Retrieves all menus.
        /// </summary>
        /// <param name="request">Empty request body.</param>
        /// <returns>A list of menus.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllMenuQueryRequest request)
        {
            // Check the cache first if it exists
            var cachedMenus = await _cacheService.GetCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu);
            if (cachedMenus != null && cachedMenus.Count != 0)
            {
                return StatusCode(StatusCodes.Status200OK, new OkDataResponse<List<EntityMenu>>(cachedMenus));
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result.Status == StatusCodes.Status200OK && result is IDataResponse<List<EntityMenu>> dataResult && dataResult != null)
            {
                // Generate recursive menus
                dataResult.Data = _menuService.RecursiveMenus(dataResult.Data);

                // Cache menu
                await _cacheService.AddCacheAsync(Cache.CacheKeyMenu, dataResult.Data);

                return StatusCode(dataResult.Status, dataResult);
            }
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Retrieves a menu by its ID.
        /// </summary>
        /// <param name="request">The ID of the menu to retrieve.</param>
        /// <returns>The requested menu.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById([FromQuery] GetMenuByIdQueryRequest request)
        {
            // Validate the request
            var validationResult = await _validationService.ValidateAsync(request, HttpContext.RequestAborted);
            if (validationResult != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestResponse(validationResult));
            }

            // Check the cache first if it exists
            var cachedMenus = await _cacheService.GetCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu);
            if (cachedMenus != null && cachedMenus.Count != 0)
            {
                // Filter menu by ID from cache
                var filteredMenu = _menuService.FilterRecursiveMenuById(cachedMenus, request.Id);
                if (filteredMenu != null)
                {
                    return StatusCode(StatusCodes.Status200OK, new OkDataResponse<EntityMenu>(filteredMenu));
                }
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Retrieves a menu by its ID along with its sub-menus.
        /// </summary>
        /// <param name="request">The ID of the menu to retrieve.</param>
        /// <returns>The requested menu with sub-menus.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("get-by-id-with-submenus")]
        public async Task<IActionResult> GetByIdWithSubMenus([FromQuery] GetMenuByIdWithSubMenusQueryRequest request)
        {
            // Validate the request
            var validationResult = await _validationService.ValidateAsync(request, HttpContext.RequestAborted);
            if (validationResult != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestResponse(validationResult));
            }

            // Check the cache first if it exists
            var cachedMenus = await _cacheService.GetCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu);
            if (cachedMenus != null && cachedMenus.Count != 0)
            {
                // Filter menu by Id from cache
                var filteredMenu = _menuService.FilterRecursiveMenuByIdWithSubMenus(cachedMenus, request.Id);
                if (filteredMenu != null)
                {
                    return StatusCode(StatusCodes.Status200OK, new OkDataResponse<EntityMenu>(filteredMenu));
                }
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result.Status == StatusCodes.Status200OK && result is IDataResponse<List<EntityMenu>> dataResult && dataResult != null)
            {
                // Generate recursive menu
                var parentMenu = _menuService.RecursiveMenu(dataResult.Data, request.Id);

                return StatusCode(dataResult.Status, new OkDataResponse<EntityMenu>(parentMenu));
            }
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Search Menu
        /// </summary>
        /// <param name="request">Menu Keyword</param>
        /// <returns>List of menus</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] SearchMenuQueryRequest request)
        {
            // Validate the request
            var validationResult = await _validationService.ValidateAsync(request, HttpContext.RequestAborted);
            if (validationResult != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestResponse(validationResult));
            }

            // Check the cache first if it exists
            var cachedMenus = await _cacheService.GetCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu);
            if (cachedMenus != null && cachedMenus.Count != 0)
            {
                // Filter menu by Keyword from cache
                var filteredMenus = _menuService.FilterRecursiveMenusByKeyword(cachedMenus, request.Keyword);
                if (filteredMenus != null && filteredMenus.Count != 0)
                {
                    return StatusCode(StatusCodes.Status200OK, new OkDataResponse<List<EntityMenu>>(filteredMenus));
                }
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Removes the cached menu.
        /// </summary>
        /// <returns>True if the menu cache is removed successfully; otherwise, false.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("remove-cache")]
        public async Task<IActionResult> RemoveCache()
        {
            // Check the cache first if it exists
            var cachedMenus = await _cacheService.GetCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu);
            if (cachedMenus == null)
            {
                return StatusCode(StatusCodes.Status200OK, new OkResponse(ResultMessages.MenuCacheNotExist));
            }

            // Remove the cache
            if (await _cacheService.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
            {
                return StatusCode(StatusCodes.Status200OK, new OkResponse(ResultMessages.MenuCacheRemoved));
            }

            // If cache removal fails
            return StatusCode(StatusCodes.Status500InternalServerError, new InternalServerErrorResponse(ResultMessages.MenuCacheCouldNotRemoved));
        }

        /// <summary>
        /// Creates a menu.
        /// </summary>
        /// <param name="request">The features of the menu to create.</param>
        /// <returns>The ID of the created menu if successful; otherwise, false.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateMenuCommandRequest request)
        {
            // Validate the request
            var validationResult = await _validationService.ValidateAsync(request, HttpContext.RequestAborted);
            if (validationResult != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestResponse(validationResult));
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result.Status == StatusCodes.Status200OK && result is IDataResponse<long> dataResult && dataResult != null)
            {
                // If menu creation is successful, remove the menu cache
                await _cacheService.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu);
            }
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Updates a menu.
        /// </summary>
        /// <param name="request">The features of the menu to update.</param>
        /// <returns>True if the menu is updated successfully; otherwise, false.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateMenuCommandRequest request)
        {
            // Validate the request
            var validationResult = await _validationService.ValidateAsync(request, HttpContext.RequestAborted);
            if (validationResult != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestResponse(validationResult));
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result.Status == StatusCodes.Status200OK)
            {
                // If menu update is successful, remove the menu cache
                await _cacheService.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu);
            }
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Deletes a menu.
        /// </summary>
        /// <param name="request">The ID of the menu to delete.</param>
        /// <returns>True if the menu is deleted successfully; otherwise, false.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteMenuCommandRequest request)
        {
            // Validate the request
            var validationResult = await _validationService.ValidateAsync(request, HttpContext.RequestAborted);
            if (validationResult != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestResponse(validationResult));
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result.Status == StatusCodes.Status200OK)
            {
                // If menu deletion is successful, remove the menu cache
                await _cacheService.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu);
            }
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Rolls back a menu by its ID.
        /// </summary>
        /// <param name="request">The ID of the menu to rollback.</param>
        /// <returns>True if the menu is successfully rolled back; otherwise, false.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("rollback-by-id")]
        public async Task<IActionResult> RolllbackById([FromBody] RollbackMenuByIdCommandRequest request)
        {
            // Validate the request
            var validationResult = await _validationService.ValidateAsync(request, HttpContext.RequestAborted);
            if (validationResult != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestResponse(validationResult));
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Rolls back a menu by its screen code.
        /// </summary>
        /// <param name="request">The screen code of the menu to rollback.</param>
        /// <returns>True if the menu is successfully rolled back; otherwise, false.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("rollback-by-screencode")]
        public async Task<IActionResult> RolllbackByScreenCode([FromBody] RollbackMenuByScreenCodeCommandRequest request)
        {
            // Validate the request
            var validationResult = await _validationService.ValidateAsync(request, HttpContext.RequestAborted);
            if (validationResult != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestResponse(validationResult));
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Rolls back menus by token.
        /// </summary>
        /// <param name="request">The token used to identify menus to rollback.</param>
        /// <returns>The number of rows rollbacked if the menu is rollbacked successfully; otherwise, false.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("rollback-by-token")]
        public async Task<IActionResult> RolllbackByToken([FromBody] RollbackMenusByTokenCommandRequest request)
        {
            // Validate the request
            var validationResult = await _validationService.ValidateAsync(request, HttpContext.RequestAborted);
            if (validationResult != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestResponse(validationResult));
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            return StatusCode(result.Status, result);
        }
    }
}
