using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SUPBank.Api.Controllers;
using SUPBank.Application.Features.Menu.Queries;
using SUPBank.Application.Interfaces.Services;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Results.Data;
using SUPBank.UnitTests.xUnit.Utilities.Helpers;
using Xunit;

namespace SUPBank.UnitTests.xUnit.Presantation.Api
{
    public class MenuControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IRedisCacheService> _cacheServiceMock;
        private readonly MenuController _controller;

        public MenuControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _cacheServiceMock = new Mock<IRedisCacheService>();
            _controller = new MenuController(_mediatorMock.Object, _cacheServiceMock.Object);

            ControllerTestHelper.SetHttpContext(_controller);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenCacheIsNotEmpty()
        {
            // Arrange
            var cachedMenus = new List<EntityMenu>
            {
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", SubMenus = [
                    new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", SubMenus = [] },
                    new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", SubMenus = [
                        new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", SubMenus = [] }
                    ] }
                ] },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", SubMenus = [] }
            };

            _cacheServiceMock.Setup(c => c.GetCacheAsync<List<EntityMenu>>(CacheKeys.CacheKeyMenu))
                .ReturnsAsync(cachedMenus);

            var request = new GetAllMenuQueryRequest();

            // Act
            var result = await _controller.GetAll(request) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var successDataResult = result.Value as SuccessDataResult<List<EntityMenu>>;
            Assert.NotNull(successDataResult);
            Assert.True(successDataResult.IsSuccess);
            Assert.Equal(cachedMenus, successDataResult.Data);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenCacheIsEmptyAndMediatorReturnsSuccessWithData()
        {
            // Arrange
            List<EntityMenu> cachedMenus = null;
            var menusFromMediator = new List<EntityMenu>
            {
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", SubMenus = [] },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", SubMenus = [] },
                new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", SubMenus = [] },
                new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", SubMenus = [] },
                new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", SubMenus = [] }
            };

            _cacheServiceMock.Setup(c => c.GetCacheAsync<List<EntityMenu>>(CacheKeys.CacheKeyMenu))
                .ReturnsAsync(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllMenuQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<List<EntityMenu>>(menusFromMediator));

            var request = new GetAllMenuQueryRequest();

            // Act
            var result = await _controller.GetAll(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var successDataResult = result.Value as SuccessDataResult<List<EntityMenu>>;
            Assert.NotNull(successDataResult);
            Assert.True(successDataResult.IsSuccess);
            Assert.Equal(menusFromMediator.Where(menu => menu.ParentId == 0), successDataResult.Data);
        }

        [Fact]
        public async Task GetAll_ShouldReturnBadRequest_WhenCacheIsEmptyAndMediatorReturnsFailure()
        {
            // Arrange
            List<EntityMenu> cachedMenus = null;

            _cacheServiceMock.Setup(c => c.GetCacheAsync<List<EntityMenu>>(CacheKeys.CacheKeyMenu))
                .ReturnsAsync(cachedMenus);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllMenuQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ErrorDataResult<List<EntityMenu>>("error message"));

            var request = new GetAllMenuQueryRequest();

            // Act
            var result = await _controller.GetAll(request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }


        [Fact]
        public async Task GetById_ShouldReturnOk_WhenCacheContainsMenu()
        {
            // Arrange
            var cachedMenus = new List<EntityMenu>
            {
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", SubMenus = [
                    new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", SubMenus = [] },
                    new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", SubMenus = [
                        new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", SubMenus = [] }
                    ] }
                ] },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", SubMenus = [] }
            };

            _cacheServiceMock.Setup(c => c.GetCacheAsync<List<EntityMenu>>(CacheKeys.CacheKeyMenu))
                .ReturnsAsync(cachedMenus);

            var request = new GetMenuByIdQueryRequest { Id = 1 };

            // Act
            var result = await _controller.GetById(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var successDataResult = result.Value as SuccessDataResult<EntityMenu>;
            Assert.NotNull(successDataResult);
            Assert.True(successDataResult.IsSuccess);
            Assert.Equal(1, successDataResult.Data.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenCacheIsEmptyAndMediatorReturnsSuccess()
        {
            // Arrange
            List<EntityMenu> cachedMenus = null;
            var menuFromMediator = new EntityMenu { Id = 1, ParentId = 0, Name_EN = "Menu1" };

            _cacheServiceMock.Setup(c => c.GetCacheAsync<List<EntityMenu>>(CacheKeys.CacheKeyMenu))
                .ReturnsAsync(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMenuByIdQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<EntityMenu>(menuFromMediator));

            var request = new GetMenuByIdQueryRequest { Id = 1 };

            // Act
            var result = await _controller.GetById(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var successDataResult = result.Value as SuccessDataResult<EntityMenu>;
            Assert.NotNull(successDataResult);
            Assert.True(successDataResult.IsSuccess);
            Assert.Equal(menuFromMediator, successDataResult.Data);
        }

        [Fact]
        public async Task GetById_ShouldReturnBadRequest_WhenCacheIsEmptyAndMediatorReturnsFailure()
        {
            // Arrange
            List<EntityMenu> cachedMenus = null;

            _cacheServiceMock.Setup(c => c.GetCacheAsync<List<EntityMenu>>(CacheKeys.CacheKeyMenu))
                .ReturnsAsync(cachedMenus);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMenuByIdQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ErrorDataResult<EntityMenu>("error message"));

            var request = new GetMenuByIdQueryRequest { Id = 1 };

            // Act
            var result = await _controller.GetById(request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }



        [Fact]
        public async Task GetByIdWitSubMenus_ShouldReturnOk_WhenCacheContainsMenu()
        {
            // Arrange
            var cachedMenus = new List<EntityMenu>
            {
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", SubMenus = [
                    new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", SubMenus = [] },
                    new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", SubMenus = [
                        new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", SubMenus = [] }
                    ] }
                ] },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", SubMenus = [] }
            };

            _cacheServiceMock.Setup(c => c.GetCacheAsync<List<EntityMenu>>(CacheKeys.CacheKeyMenu))
                .ReturnsAsync(cachedMenus);

            var request = new GetMenuByIdWithSubMenusQueryRequest { Id = 1 };

            // Act
            var result = await _controller.GetByIdWitSubMenus(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var successDataResult = result.Value as SuccessDataResult<EntityMenu>;
            Assert.NotNull(successDataResult);
            Assert.True(successDataResult.IsSuccess);
            Assert.Equal(1, successDataResult.Data.Id);
            Assert.NotEmpty(successDataResult.Data.SubMenus);
        }

        [Fact]
        public async Task GetByIdWitSubMenus_ShouldReturnOk_WhenCacheIsEmptyAndMediatorReturnsSuccess()
        {
            // Arrange
            List<EntityMenu> cachedMenus = null;
            var menusFromMediator = new List<EntityMenu>
            {
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", SubMenus = [] },
                new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", SubMenus = [] },
                new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", SubMenus = [] },
                new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", SubMenus = [] }
            };

            _cacheServiceMock.Setup(c => c.GetCacheAsync<List<EntityMenu>>(CacheKeys.CacheKeyMenu))
                .ReturnsAsync(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMenuByIdWithSubMenusQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<List<EntityMenu>>(menusFromMediator));

            var request = new GetMenuByIdWithSubMenusQueryRequest { Id = 1 };

            // Act
            var result = await _controller.GetByIdWitSubMenus(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var successDataResult = result.Value as SuccessDataResult<EntityMenu>;
            Assert.NotNull(successDataResult);
            Assert.True(successDataResult.IsSuccess);
            Assert.Equal(1, successDataResult.Data.Id);
            Assert.NotEmpty(successDataResult.Data.SubMenus);
        }

        [Fact]
        public async Task GetByIdWitSubMenus_ShouldReturnBadRequest_WhenCacheIsEmptyAndMediatorReturnsFailure()
        {
            // Arrange
            List<EntityMenu> cachedMenus = null;

            _cacheServiceMock.Setup(c => c.GetCacheAsync<List<EntityMenu>>(CacheKeys.CacheKeyMenu))
                .ReturnsAsync(cachedMenus);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMenuByIdWithSubMenusQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ErrorDataResult<List<EntityMenu>>("error message"));

            var request = new GetMenuByIdWithSubMenusQueryRequest { Id = 1 };

            // Act
            var result = await _controller.GetByIdWitSubMenus(request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }









    }
}
