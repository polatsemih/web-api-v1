using MediatR;
using Microsoft.AspNetCore.Mvc;
using VkBank.Application.Features.Queries.GetAllEvent;
using VkBank.Application.Features.Queries.GetEvent;
using VkBank.Application.Features.Commands.CreateEvent;
using VkBank.Application.Features.Commands.DeleteEvent;
using VkBank.Application.Features.Commands.UpdateEvent;
using VkBank.Domain.Entities;
using VkBank.Domain.Results.Data;
using VkBank.Infrastructure.Cache.Abstract;

namespace VkBank.Api.Controllers
{
    [Route("api/menu")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly string CacheKeyAllMenu = "CacheAllMenu";

        private readonly IMediator _mediator;
        private readonly ICacheManager _cacheManager;

        public MenuController(IMediator mediator, ICacheManager cacheManager)
        {
            _mediator = mediator;
            _cacheManager = cacheManager;
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
            TimeSpan CacheDuration = TimeSpan.FromMinutes(10); // TimeSpan.Zero for infinite caching

            var cachedMenus = _cacheManager.GetCache<List<Menu>>(CacheKeyAllMenu);
            if (cachedMenus != null)
            {
                return Ok(new SuccessDataResult<List<Menu>>(cachedMenus));
            }

            var result = await _mediator.Send(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            Dictionary<long, Menu> menuDictionary = result.Data.ToDictionary(menu => menu.Id);

            foreach (var menu in result.Data)
            {
                if (menu.ParentId != 0 && menuDictionary.TryGetValue(menu.ParentId, out var parentMenu))
                {
                    parentMenu.Children.Add(menu);
                }
            }

            result.Data = result.Data.Where(menu => menu.ParentId == 0).ToList();

            _cacheManager.AddCache(CacheKeyAllMenu, result.Data, CacheDuration);

            return Ok(result);
        }

        /// <summary>
        /// Get menu by Id
        /// </summary>
        /// <param name="request">Menu Id</param>
        /// <returns>A menu</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("get")]
        public async Task<IActionResult> Get([FromQuery] GetMenuQueryRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.IsSuccess == false)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create Menu
        /// </summary>
        /// <param name="request">Menu Features</param>
        /// <returns>True if the menu is created successfully, false otherwise</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateMenuCommandRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.IsSuccess == false)
            {
                return BadRequest(result.Message);
            }

            _cacheManager.RemoveCache(CacheKeyAllMenu);

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
            var result = await _mediator.Send(request);
            if (result.IsSuccess == false)
            {
                return BadRequest(result.Message);
            }

            _cacheManager.RemoveCache(CacheKeyAllMenu);

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
            var result = await _mediator.Send(request);
            if (result.IsSuccess == false)
            {
                return BadRequest(result.Message);
            }

            _cacheManager.RemoveCache(CacheKeyAllMenu);

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
        public async Task<IActionResult> Search([FromQuery] GetSearchedMenuQueryRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.IsSuccess == false)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }


        /// <summary>
        /// Rollback Menu
        /// </summary>
        /// <param name="request">Menu Id, Action Type: 0 For Delete, 1 For Update</param>
        /// <returns>True if the menu is rollbacked successfully, false otherwise</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("rollback-by-id")]
        public async Task<IActionResult> RolllbackById([FromBody] RollbackMenuByIdCommandRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.IsSuccess == false)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        /// <summary>
        /// Rollback Menu
        /// </summary>
        /// <param name="request">Screen Code, Action Type: 0 For Delete, 1 For Update</param>
        /// <returns>True if the menu is rollbacked successfully, false otherwise</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("rollback-by-screencode")]
        public async Task<IActionResult> RolllbackByScreenCode([FromBody] RollbackMenuByScreenCodeCommandRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.IsSuccess == false)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        /// <summary>
        /// Rollback Menu
        /// </summary>
        /// <param name="request">Date, Action Type: 0 For Delete, 1 For Update</param>
        /// <returns>True if the menu is rollbacked successfully, false otherwise</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("rollback-by-date")]
        public async Task<IActionResult> RolllbackByDate([FromBody] RollbackMenuByDateCommandRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.IsSuccess == false)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
    }
}
