﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using VkBank.Domain.Entities;
using VkBank.Domain.Results.Data;
using VkBank.Application.Features.Menu.Commands;
using VkBank.Application.Features.Menu.Queries;
using VkBank.Infrastructure.Services.Caching.Abstract;

namespace VkBank.Api.Controllers
{
    [Route("api/menu")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly string CacheKeyMenu = "CacheAllMenu";

        private readonly IMediator _mediator;
        private readonly ICacheService _cacheManager;

        public MenuController(IMediator mediator, ICacheService cacheManager)
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
            var cachedMenus = _cacheManager.GetCache<List<EntityMenu>>(CacheKeyMenu);
            if (cachedMenus != null)
            {
                return Ok(new SuccessDataResult<List<EntityMenu>>(cachedMenus));
            }

            var result = await _mediator.Send(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            Dictionary<long, EntityMenu> menuDictionary = result.Data.ToDictionary(menu => menu.Id);

            foreach (var menu in result.Data)
            {
                if (menu.ParentId != 0 && menuDictionary.TryGetValue(menu.ParentId, out var parentMenu))
                {
                    parentMenu.SubMenu.Add(menu);
                }
            }

            result.Data = result.Data.Where(menu => menu.ParentId == 0).ToList();

            _cacheManager.AddCache(CacheKeyMenu, result.Data, TimeSpan.Zero); // TimeSpan.FromMinutes(10) for 10 minutes caching

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
        public async Task<IActionResult> GetById([FromQuery] GetMenuByIdQueryRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.IsSuccess == false)
            {
                return BadRequest(result.Message);
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
        /// <returns>Created Menu Id if the menu is created successfully, false otherwise</returns>
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

            _cacheManager.RemoveCache(CacheKeyMenu);

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

            _cacheManager.RemoveCache(CacheKeyMenu);

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

            _cacheManager.RemoveCache(CacheKeyMenu);

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
            var result = await _mediator.Send(request);
            if (result.IsSuccess == false)
            {
                return BadRequest(result.Message);
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
            var result = await _mediator.Send(request);
            if (result.IsSuccess == false)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        /// <summary>
        /// Rollback Menu By Token
        /// </summary>
        /// <param name="request">Token</param>
        /// <returns>Rollbacked row count if the menu is rollbacked successfully, false otherwise</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("rollback-by-token")]
        public async Task<IActionResult> RolllbackByToken([FromBody] RollbackMenusByTokenCommandRequest request)
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
