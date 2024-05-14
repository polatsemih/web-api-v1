using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VkBank.Application.Features.Queries.GetAllEvent;
using VkBank.Application.Features.Queries.GetEvent;
using VkBank.Application.Features.Commands.CreateEvent;
using VkBank.Application.Features.Commands.DeleteEvent;
using VkBank.Application.Features.Commands.UpdateEvent;

namespace VkBank.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all menus
        /// </summary>
        /// <param name="request">Empty request body</param>
        /// <returns>List of menus</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("get-all-menu")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllMenuQueryRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.IsSuccess == false)
                return BadRequest(result.Message);
            return Ok(result);
        }

        /// <summary>
        /// Get menu by Id
        /// </summary>
        /// <param name="request">Menu Id</param>
        /// <returns>A menu</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet(":Id")]
        public async Task<IActionResult> Get([FromQuery] GetMenuQueryRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.IsSuccess == false)
                return BadRequest(result.Message);
            return Ok(result);
        }

        /// <summary>
        /// Create Menu
        /// </summary>
        /// <param name="request">Menu Features</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMenuCommandRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.IsSuccess == false)
                return BadRequest(result.Message);
            return Ok(result);
        }

        /// <summary>
        /// Update Menu
        /// </summary>
        /// <param name="request">Menu Features</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateMenuCommandRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.IsSuccess == false)
                return BadRequest(result.Message);
            return Ok(result);
        }

        /// <summary>
        /// Delete Menu
        /// </summary>
        /// <param name="request">Menu Id</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteMenuCommandRequest request)
        {
            var result = await _mediator.Send(request);
            if (result.IsSuccess == false)
                return BadRequest(result.Message);
            return Ok(result);
        }
    }
}
