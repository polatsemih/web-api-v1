using MediatR;
using Microsoft.AspNetCore.Mvc;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Results.Data;
using SUPBank.Application.Features.Menu.Commands;
using SUPBank.Application.Features.Menu.Queries;
using SUPBank.Domain.Results;
using SUPBank.Domain.Contstants;
using StackExchange.Redis;
using SUPBank.Application.Interfaces.Services;
using System.Reflection;

namespace SUPBank.Api.Controllers
{
    [ApiController]
    [Route("api/menu")]
    public class MenuController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRedisCacheService _cacheService;

        public MenuController(IMediator mediator, IRedisCacheService cacheService)
        {
            _mediator = mediator;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Get all menus
        /// </summary>
        /// <param name="request">Empty request body</param>
        /// <returns>List of menus</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllMenuQueryRequest request)
        {
            var cachedMenus = await _cacheService.GetCacheAsync<List<EntityMenu>>(CacheKeys.CacheKeyMenu);
            if (cachedMenus != null)
            {
                return Ok(new SuccessDataResult<List<EntityMenu>>(cachedMenus));
            }

            var result = await _mediator.Send(request, HttpContext.RequestAborted);
            if (result == null || !result.IsSuccess)
            {
                return BadRequest(result);
            }

            if (result.Data != null && result.Data.Count != 0)
            {
                Dictionary<long, EntityMenu> menuDictionary = result.Data.ToDictionary(menu => menu.Id);
                foreach (var menu in result.Data)
                {
                    if (menu.ParentId != 0 && menuDictionary.TryGetValue(menu.ParentId, out var parentMenu))
                    {
                        parentMenu.SubMenus.Add(menu);
                    }
                }

                result.Data = result.Data.Where(menu => menu.ParentId == 0).ToList();
                await _cacheService.AddCacheAsync(CacheKeys.CacheKeyMenu, result.Data, TimeSpan.Zero); // TimeSpan.FromMinutes(10) for 10 minutes caching
            }

            return Ok(result);
        }

        /// <summary>
        /// Get menu by Id
        /// </summary>
        /// <param name="request">Menu Id</param>
        /// <returns>A menu</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById([FromQuery] GetMenuByIdQueryRequest request)
        {
            var cachedMenus = await _cacheService.GetCacheAsync<List<EntityMenu>>(CacheKeys.CacheKeyMenu);
            if (cachedMenus != null)
            {
                var filteredMenu = cachedMenus.FirstOrDefault(menu => menu.Id == request.Id);
                if (filteredMenu != null)
                {
                    var filteredMenuWithoutSubMenus = new EntityMenu
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
                    return Ok(new SuccessDataResult<EntityMenu>(filteredMenuWithoutSubMenus));
                }
            }

            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result == null || !result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get menu by Id with sub menus
        /// </summary>
        /// <param name="request">Menu Id</param>
        /// <returns>A menu with sub menus</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("get-by-id-with-submenus")]
        public async Task<IActionResult> GetByIdWitSubMenus([FromQuery] GetMenuByIdWithSubMenusQueryRequest request)
        {
            var cachedMenus = await _cacheService.GetCacheAsync<List<EntityMenu>>(CacheKeys.CacheKeyMenu);
            if (cachedMenus != null)
            {
                var filteredMenu = cachedMenus.FirstOrDefault(menu => menu.Id == request.Id);
                if (filteredMenu != null)
                {
                    return Ok(new SuccessDataResult<EntityMenu>(filteredMenu));
                }
            }

            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result == null || !result.IsSuccess)
            {
                return BadRequest(result);
            }

            if (result.Data != null && result.Data.Count != 0)
            {
                Dictionary<long, EntityMenu> menuDictionary = result.Data.ToDictionary(menu => menu.Id);
                foreach (var menu in result.Data)
                {
                    if (menu.ParentId != 0 && menuDictionary.TryGetValue(menu.ParentId, out var parentMenu))
                    {
                        parentMenu.SubMenus.Add(menu);
                    }
                }

                var rootMenu = result.Data.First(menu => menu.Id == request.Id);
                return Ok(new SuccessDataResult<EntityMenu>(rootMenu));
            }

            return Ok(result);
        }

        /// <summary>
        /// Search Menu
        /// </summary>
        /// <param name="request">Menu Keyword</param>
        /// <returns>List of menus</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] SearchMenuQueryRequest request)
        {
            var cachedMenus = await _cacheService.GetCacheAsync<List<EntityMenu>>(CacheKeys.CacheKeyMenu);
            if (cachedMenus != null)
            {
                var filteredMenus = cachedMenus
                    .Where(menu => menu.Keyword.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (filteredMenus.Count != 0)
                {
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

                    return Ok(new SuccessDataResult<List<EntityMenu>>(filteredMenusWithoutSubMenus));
                }
            }

            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result == null || !result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Remove Cached Menu
        /// </summary>
        /// <param name="request">Empty request body</param>
        /// <returns>True if the menu cache removed successfully, false otherwise</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("remove-cache")]
        public async Task<IActionResult> RemoveCache()
        {
            var cachedMenus = await _cacheService.GetCacheAsync<List<EntityMenu>>(CacheKeys.CacheKeyMenu);
            if (cachedMenus == null)
            {
                return Ok(new ErrorResult(ResultMessages.MenuCacheNotExist));
            }

            await _cacheService.RemoveCacheAsync(CacheKeys.CacheKeyMenu);
            return Ok(new SuccessResult(ResultMessages.MenuCacheRemoved));
        }

        /// <summary>
        /// Create Menu
        /// </summary>
        /// <param name="request">Menu Features</param>
        /// <returns>Created Menu Id if the menu is created successfully, false otherwise</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateMenuCommandRequest request)
        {
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result == null || !result.IsSuccess)
            {
                return BadRequest(result);
            }

            await _cacheService.RemoveCacheAsync(CacheKeys.CacheKeyMenu);
            return Ok(result);
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
            string source = MethodBase.GetCurrentMethod().DeclaringType.FullName + MethodBase.GetCurrentMethod().Name;

            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result == null || !result.IsSuccess)
            {
                return BadRequest(result);
            }

            await _cacheService.RemoveCacheAsync(CacheKeys.CacheKeyMenu);
            return Ok(result);
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
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result == null || !result.IsSuccess)
            {
                return BadRequest(result);
            }

            await _cacheService.RemoveCacheAsync(CacheKeys.CacheKeyMenu);
            return Ok(result);
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
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result == null || !result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
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
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result == null || !result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
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
            var result = await _mediator.Send(request: request, cancellationToken: HttpContext.RequestAborted);
            if (result == null || !result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
