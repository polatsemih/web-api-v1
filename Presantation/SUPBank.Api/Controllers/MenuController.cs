using Microsoft.AspNetCore.Mvc;
using MediatR;
using StackExchange.Redis;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Responses.Data;
using SUPBank.Domain.Responses;
using SUPBank.Application.Interfaces.Services;
using SUPBank.Application.Features.Menu.Queries.Requests;
using SUPBank.Application.Features.Menu.Commands.Requests;

namespace SUPBank.Api.Controllers
{
    [ApiController]
    [Route("api/menu")]
    public class MenuController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRedisCacheService _cacheService;
        private readonly IValidationService _validationService;

        public MenuController(IMediator mediator, IRedisCacheService cacheService, IValidationService validationService)
        {
            _mediator = mediator;
            _cacheService = cacheService;
            _validationService = validationService;
        }

        private List<EntityMenu> RecursiveMenu(List<EntityMenu> menus)
        {
            var menuDictionary = menus.ToDictionary(menu => menu.Id);
            foreach (var menu in menus)
            {
                if (menu.ParentId != 0 && menuDictionary.TryGetValue(menu.ParentId, out var parentMenu))
                {
                    parentMenu.SubMenus.Add(menu);
                }
            }
            return menus;
        }

        private EntityMenu? FilterMenuById(List<EntityMenu> menus, long id)
        {
            foreach (var menu in menus)
            {
                if (menu.Id == id)
                {
                    return menu;
                }

                if (menu.SubMenus != null)
                {
                    var subMenu = FilterMenuById(menu.SubMenus, id);
                    if (subMenu != null)
                    {
                        return subMenu;
                    }
                }
            }
            return default;
        }

        private List<EntityMenu> FilterMenusByKeyword(List<EntityMenu> menus, string keyword)
        {
            List<EntityMenu> filteredMenus = [];

            foreach (var menu in menus)
            {
                if (menu.Keyword != null && menu.Keyword.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    filteredMenus.Add(menu);
                }

                if (menu.SubMenus != null)
                {
                    var subMenus = FilterMenusByKeyword(menu.SubMenus, keyword);
                    if (subMenus.Count != 0)
                    {
                        filteredMenus.AddRange(subMenus);
                    }
                }
            }

            return filteredMenus.OrderBy(menu => menu.Id).ToList();
        }



        /// <summary>
        /// Get all menus
        /// </summary>
        /// <param name="request">Empty request body</param>
        /// <returns>List of menus</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllMenuQueryRequest request)
        {
            // Check the cache first if it exists
            var cachedMenus = await _cacheService.GetCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu);
            if (cachedMenus != null)
            {
                return StatusCode(StatusCodes.Status200OK, new OkDataResponse<List<EntityMenu>>(cachedMenus));
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result.Status == StatusCodes.Status200OK && result is IDataResponse<List<EntityMenu>> dataResult && dataResult != null)
            {
                // Recursive menu
                dataResult.Data = RecursiveMenu(dataResult.Data).Where(menu => menu.ParentId == 0).ToList();

                // Cache menu
                await _cacheService.AddCacheAsync(Cache.CacheKeyMenu, dataResult.Data);

                return StatusCode(dataResult.Status, dataResult);
            }
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Get menu by Id
        /// </summary>
        /// <param name="request">Menu Id</param>
        /// <returns>A menu</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById([FromQuery] GetMenuByIdQueryRequest request)
        {
            // Validate request
            var validationResult = await _validationService.ValidateAsync(request, HttpContext.RequestAborted);
            if (validationResult != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestResponse(validationResult));
            }

            // Check the cache first if it exists
            var cachedMenus = await _cacheService.GetCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu);
            if (cachedMenus != null)
            {
                // Filter menu by Id from cache
                var filteredMenu = FilterMenuById(cachedMenus, request.Id);
                if (filteredMenu != null)
                {
                    // Remove SubMenus property
                    EntityMenu filteredMenuWithoutSubMenus = new()
                    {
                        Id = filteredMenu.Id,
                        ParentId = filteredMenu.ParentId,
                        Name_TR = filteredMenu.Name_TR,
                        Name_EN = filteredMenu.Name_EN,
                        ScreenCode = filteredMenu.ScreenCode,
                        Type = filteredMenu.Type,
                        Priority = filteredMenu.Priority,
                        Keyword = filteredMenu.Keyword,
                        Icon = filteredMenu.Icon,
                        IsGroup = filteredMenu.IsGroup,
                        IsNew = filteredMenu.IsNew,
                        NewStartDate = filteredMenu.NewStartDate,
                        NewEndDate = filteredMenu.NewEndDate,
                        IsActive = filteredMenu.IsActive,
                        CreatedDate = filteredMenu.CreatedDate,
                        LastModifiedDate = filteredMenu.LastModifiedDate
                    };

                    return StatusCode(StatusCodes.Status200OK, new OkDataResponse<EntityMenu>(filteredMenuWithoutSubMenus));
                }
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Get menu by Id with sub menus
        /// </summary>
        /// <param name="request">Menu Id</param>
        /// <returns>A menu with sub menus</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("get-by-id-with-submenus")]
        public async Task<IActionResult> GetByIdWithSubMenus([FromQuery] GetMenuByIdWithSubMenusQueryRequest request)
        {
            // Validate request
            var validationResult = await _validationService.ValidateAsync(request, HttpContext.RequestAborted);
            if (validationResult != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestResponse(validationResult));
            }

            // Check the cache first if it exists
            var cachedMenus = await _cacheService.GetCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu);
            if (cachedMenus != null)
            {
                // Filter menu by Id from cache
                var filteredMenu = FilterMenuById(cachedMenus, request.Id);
                if (filteredMenu != null)
                {
                    return StatusCode(StatusCodes.Status200OK, new OkDataResponse<EntityMenu>(filteredMenu));
                }
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result.Status == StatusCodes.Status200OK && result is IDataResponse<List<EntityMenu>> dataResult && dataResult != null)
            {
                // Recursive menu
                var parentMenu = RecursiveMenu(dataResult.Data).First(menu => menu.Id == request.Id);

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
            // Validate request
            var validationResult = await _validationService.ValidateAsync(request, HttpContext.RequestAborted);
            if (validationResult != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestResponse(validationResult));
            }

            // Check the cache first if it exists
            var cachedMenus = await _cacheService.GetCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu);
            if (cachedMenus != null)
            {
                // Filter menus by Keyword from cache
                var filteredMenus = FilterMenusByKeyword(cachedMenus, request.Keyword);
                if (filteredMenus.Count != 0)
                {
                    // Remove SubMenus property
                    var filteredMenusWithoutSubMenus = filteredMenus.Select(filteredMenu => new EntityMenu
                    {
                        Id = filteredMenu.Id,
                        ParentId = filteredMenu.ParentId,
                        Name_TR = filteredMenu.Name_TR,
                        Name_EN = filteredMenu.Name_EN,
                        ScreenCode = filteredMenu.ScreenCode,
                        Type = filteredMenu.Type,
                        Priority = filteredMenu.Priority,
                        Keyword = filteredMenu.Keyword,
                        Icon = filteredMenu.Icon,
                        IsGroup = filteredMenu.IsGroup,
                        IsNew = filteredMenu.IsNew,
                        NewStartDate = filteredMenu.NewStartDate,
                        NewEndDate = filteredMenu.NewEndDate,
                        IsActive = filteredMenu.IsActive,
                        CreatedDate = filteredMenu.CreatedDate,
                        LastModifiedDate = filteredMenu.LastModifiedDate
                    }).ToList();

                    return StatusCode(StatusCodes.Status200OK, new OkDataResponse<List<EntityMenu>>(filteredMenusWithoutSubMenus));
                }
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Remove Cached Menu
        /// </summary>
        /// <param name="request">Empty request body</param>
        /// <returns>True if the menu cache removed successfully, false otherwise</returns>
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
            if (await _cacheService.RemoveCacheAsync(Cache.CacheKeyMenu))
            {
                return StatusCode(StatusCodes.Status200OK, new OkResponse(ResultMessages.MenuCacheRemoved));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new InternalServerErrorResponse(ResultMessages.MenuCacheCouldNotRemoved));
        }

        /// <summary>
        /// Create Menu
        /// </summary>
        /// <param name="request">Menu Features</param>
        /// <returns>Created Menu Id if the menu is created successfully, false otherwise</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateMenuCommandRequest request)
        {
            // Validate request
            var validationResult = await _validationService.ValidateAsync(request, HttpContext.RequestAborted);
            if (validationResult != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestResponse(validationResult));
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result.Status == StatusCodes.Status200OK && result is IDataResponse<long> dataResult && dataResult != null)
            {
                // Remove cache
                await _cacheService.RemoveCacheAsync(Cache.CacheKeyMenu);
            }
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Update Menu
        /// </summary>
        /// <param name="request">Menu Features</param>
        /// <returns>True if the menu is updated successfully, false otherwise</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateMenuCommandRequest request)
        {
            // Validate request
            var validationResult = await _validationService.ValidateAsync(request, HttpContext.RequestAborted);
            if (validationResult != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestResponse(validationResult));
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result.Status == StatusCodes.Status200OK)
            {
                // Remove cache
                await _cacheService.RemoveCacheAsync(Cache.CacheKeyMenu);
            }
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Delete Menu
        /// </summary>
        /// <param name="request">Menu Id</param>
        /// <returns>True if the menu is deleted successfully, false otherwise</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteMenuCommandRequest request)
        {
            // Validate request
            var validationResult = await _validationService.ValidateAsync(request, HttpContext.RequestAborted);
            if (validationResult != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new BadRequestResponse(validationResult));
            }

            // Send request to mediator
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result.Status == StatusCodes.Status200OK)
            {
                // Remove cache
                await _cacheService.RemoveCacheAsync(Cache.CacheKeyMenu);
            }
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Rollback Menu By Id
        /// </summary>
        /// <param name="request">Menu Id</param>
        /// <returns>True if the menu is rollbacked successfully, false otherwise</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("rollback-by-id")]
        public async Task<IActionResult> RolllbackById([FromBody] RollbackMenuByIdCommandRequest request)
        {
            // Validate request
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
        /// Rollback Menu By Screen Code
        /// </summary>
        /// <param name="request">Screen Code</param>
        /// <returns>True if the menu is rollbacked successfully, false otherwise</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("rollback-by-screencode")]
        public async Task<IActionResult> RolllbackByScreenCode([FromBody] RollbackMenuByScreenCodeCommandRequest request)
        {
            // Validate request
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
        /// Rollback Menus By Token
        /// </summary>
        /// <param name="request">Token</param>
        /// <returns>Rollbacked row count if the menu is rollbacked successfully, false otherwise</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("rollback-by-token")]
        public async Task<IActionResult> RolllbackByToken([FromBody] RollbackMenusByTokenCommandRequest request)
        {
            // Validate request
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
