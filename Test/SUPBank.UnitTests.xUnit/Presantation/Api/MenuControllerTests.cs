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

        private void SetupCacheService(List<EntityMenu>? cachedMenus)
        {
            _cacheServiceMock.Setup(c => c.GetCacheAsync<List<EntityMenu>>(CacheKeys.CacheKeyMenu))
                .ReturnsAsync(cachedMenus);
        }

        //private void SetupMediator<TRequest, TResponse>(TResponse response) where TRequest : class
        //{
        //    _mediatorMock.Setup(m => m.Send(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(response);
        //}


        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenCacheIsNotEmpty()
        {
            // Arrange
            List<EntityMenu> cachedMenus =
            [
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", SubMenus = [
                    new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", SubMenus = [] },
                    new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", SubMenus = [
                        new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", SubMenus = [] }
                    ] }
                ] },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", SubMenus = [] }
            ];

            SetupCacheService(cachedMenus);

            var request = new GetAllMenuQueryRequest();

            // Act
            var result = await _controller.GetAll(request) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var successDataResult = result.Value as SuccessDataResult<List<EntityMenu>>;
            Assert.NotNull(successDataResult);
            Assert.True(successDataResult.IsSuccess);
            Assert.NotNull(successDataResult.Data);
            Assert.Equal(cachedMenus, successDataResult.Data);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenCacheIsEmpty_ReturnsMenu()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;
            var menusFromMediator = new List<EntityMenu>
            {
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", SubMenus = [] },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", SubMenus = [] },
                new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", SubMenus = [] },
                new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", SubMenus = [] },
                new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", SubMenus = [] }
            };

            SetupCacheService(cachedMenus);

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
            Assert.NotNull(successDataResult.Data);
            Assert.Equal(menusFromMediator.Where(menu => menu.ParentId == 0), successDataResult.Data);
        }

        [Fact]
        public async Task GetAll_ShouldReturnNotFound_WhenCacheIsEmpty_ReturnsNoMenu()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;

            SetupCacheService(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllMenuQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ErrorDataResult<List<EntityMenu>>(ResultMessages.MenuNoDatas));

            var request = new GetAllMenuQueryRequest();

            // Act
            var result = await _controller.GetAll(request) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);

            var errorDataResult = result.Value as ErrorDataResult<List<EntityMenu>>;
            Assert.NotNull(errorDataResult);
            Assert.False(errorDataResult.IsSuccess);
            Assert.Null(errorDataResult.Data);
        }

        [Fact]
        public async Task GetAll_ShouldReturnNotFound_WhenCacheIsEmpty_ReturnsNull()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;

            SetupCacheService(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllMenuQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IDataResult<List<EntityMenu>>?)null!);


            var request = new GetAllMenuQueryRequest();

            // Act
            var result = await _controller.GetAll(request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }


        [Fact]
        public async Task GetById_ShouldReturnOk_WhenCacheIsNotEmpty_ReturnsMenu()
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

            SetupCacheService(cachedMenus);

            var request = new GetMenuByIdQueryRequest { Id = 1 };

            // Act
            var result = await _controller.GetById(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var successDataResult = result.Value as SuccessDataResult<EntityMenu>;
            Assert.NotNull(successDataResult);
            Assert.True(successDataResult.IsSuccess);
            Assert.NotNull(successDataResult.Data);
            Assert.Equal(1, successDataResult.Data.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenCacheIsNotEmpty_ReturnsNoMenu()
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

            SetupCacheService(cachedMenus);

            var request = new GetMenuByIdQueryRequest { Id = 100 };

            // Act
            var result = await _controller.GetById(request) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);

            var errorDataResult = result.Value as ErrorDataResult<EntityMenu>;
            Assert.NotNull(errorDataResult);
            Assert.False(errorDataResult.IsSuccess);
            Assert.Null(errorDataResult.Data);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenCacheIsEmpty_ReturnsMenu()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;
            var menuFromMediator = new EntityMenu { Id = 1, ParentId = 0, Name_EN = "Menu1" };

            SetupCacheService(cachedMenus);

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
            Assert.NotNull(successDataResult.Data);
            Assert.Equal(menuFromMediator, successDataResult.Data);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenCacheIsEmpty_ReturnsNoMenu()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;

            SetupCacheService(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMenuByIdQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ErrorDataResult<EntityMenu>(ResultMessages.MenuNoData));

            var request = new GetMenuByIdQueryRequest { Id = 10000 };

            // Act
            var result = await _controller.GetById(request) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);

            var errorDataResult = result.Value as ErrorDataResult<EntityMenu>;
            Assert.NotNull(errorDataResult);
            Assert.False(errorDataResult.IsSuccess);
            Assert.Null(errorDataResult.Data);
        }

        [Fact]
        public async Task GetById_ShouldReturnBadRequest_WhenCacheIsEmpty_ReturnsNull()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;

            SetupCacheService(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMenuByIdQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IDataResult<EntityMenu>?)null!);

            var request = new GetMenuByIdQueryRequest { Id = 1 };

            // Act
            var result = await _controller.GetById(request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }


        [Fact]
        public async Task GetByIdWitSubMenus_ShouldReturnOk_WhenCacheIsNotEmpty_ReturnsMenu()
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

            SetupCacheService(cachedMenus);

            var request = new GetMenuByIdWithSubMenusQueryRequest { Id = 1 };

            // Act
            var result = await _controller.GetByIdWithSubMenus(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var successDataResult = result.Value as SuccessDataResult<EntityMenu>;
            Assert.NotNull(successDataResult);
            Assert.True(successDataResult.IsSuccess);
            Assert.NotNull(successDataResult.Data);
            Assert.Equal(1, successDataResult.Data.Id);
            Assert.NotEmpty(successDataResult.Data.SubMenus);
        }

        [Fact]
        public async Task GetByIdWitSubMenus_ShouldReturnNotFound_WhenCacheIsNotEmpty_ReturnsNoMenu()
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

            SetupCacheService(cachedMenus);

            var request = new GetMenuByIdWithSubMenusQueryRequest { Id = 100 };

            // Act
            var result = await _controller.GetByIdWithSubMenus(request) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);

            var errorDataResult = result.Value as ErrorDataResult<EntityMenu>;
            Assert.NotNull(errorDataResult);
            Assert.False(errorDataResult.IsSuccess);
            Assert.Null(errorDataResult.Data);
        }

        [Fact]
        public async Task GetByIdWitSubMenus_ShouldReturnOk_WhenCacheIsEmpty_ReturnsMenu()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;
            var menusFromMediator = new List<EntityMenu>
            {
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", SubMenus = [] },
                new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", SubMenus = [] },
                new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", SubMenus = [] },
                new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", SubMenus = [] }
            };

            SetupCacheService(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMenuByIdWithSubMenusQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<List<EntityMenu>>(menusFromMediator));

            var request = new GetMenuByIdWithSubMenusQueryRequest { Id = 1 };

            // Act
            var result = await _controller.GetByIdWithSubMenus(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var successDataResult = result.Value as SuccessDataResult<EntityMenu>;
            Assert.NotNull(successDataResult);
            Assert.True(successDataResult.IsSuccess);
            Assert.NotNull(successDataResult.Data);
            Assert.Equal(1, successDataResult.Data.Id);
            Assert.NotEmpty(successDataResult.Data.SubMenus);
        }

        [Fact]
        public async Task GetByIdWitSubMenus_ShouldReturnNotFound_WhenCacheIsEmpty_ReturnsNoMenu()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;

            SetupCacheService(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMenuByIdWithSubMenusQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ErrorDataResult<List<EntityMenu>>(ResultMessages.MenuNoData));

            var request = new GetMenuByIdWithSubMenusQueryRequest { Id = 10000 };

            // Act
            var result = await _controller.GetByIdWithSubMenus(request) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);

            var errorDataResult = result.Value as ErrorDataResult<List<EntityMenu>>;
            Assert.NotNull(errorDataResult);
            Assert.False(errorDataResult.IsSuccess);
            Assert.Null(errorDataResult.Data);
        }

        [Fact]
        public async Task GetByIdWitSubMenus_ShouldReturnBadRequest_WhenCacheIsEmpty_ReturnsNull()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;

            SetupCacheService(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMenuByIdWithSubMenusQueryRequest>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((IDataResult<List<EntityMenu>>?)null!);

            var request = new GetMenuByIdWithSubMenusQueryRequest { Id = 1 };

            // Act
            var result = await _controller.GetByIdWithSubMenus(request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }


        [Fact]
        public async Task Search_ShouldReturnOk_WhenCacheIsNotEmpty_AndMenusMatchKeyword()
        {
            // Arrange
            var cachedMenus = new List<EntityMenu>
            {
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", Keyword = "Keyword1", SubMenus = [
                    new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", Keyword = "Keyword3", SubMenus = [] },
                    new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", Keyword = "Keyword4", SubMenus = [
                        new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", Keyword = "Keyword5", SubMenus = [] }
                    ] }
                ] },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", Keyword = "Keyword2", SubMenus = [] }
            };

            SetupCacheService(cachedMenus);

            var request = new SearchMenuQueryRequest { Keyword = "Keyword1" };

            // Act
            var result = await _controller.Search(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var successDataResult = result.Value as SuccessDataResult<List<EntityMenu>>;
            Assert.NotNull(successDataResult);
            Assert.True(successDataResult.IsSuccess);
            Assert.NotNull(successDataResult.Data);
            Assert.Single(successDataResult.Data);
            Assert.Equal(1, successDataResult.Data.First().Id);
        }

        [Fact]
        public async Task Search_ShouldReturnOk_WhenCacheIsNotEmpty_AndNoMenusMatchKeyword()
        {
            // Arrange
            var cachedMenus = new List<EntityMenu>
            {
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", Keyword = "Keyword1", SubMenus = [
                    new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", Keyword = "Keyword3", SubMenus = [] },
                    new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", Keyword = "Keyword4", SubMenus = [
                        new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", Keyword = "Keyword5", SubMenus = [] }
                    ] }
                ] },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", Keyword = "Keyword2", SubMenus = [] }
            };

            SetupCacheService(cachedMenus);

            var request = new SearchMenuQueryRequest { Keyword = "Keyword6" };

            // Act
            var result = await _controller.Search(request) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);

            var errorDataResult = result.Value as ErrorDataResult<List<EntityMenu>>;
            Assert.NotNull(errorDataResult);
            Assert.False(errorDataResult.IsSuccess);
            Assert.Null(errorDataResult.Data);
        }

        [Fact]
        public async Task Search_ShouldReturnOk_WhenCacheIsEmpty_AndMenusMatchKeyword()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;
            var menusFromMediator = new List<EntityMenu>
            {
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", Keyword = "Keyword1", SubMenus = [] },
                new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", Keyword = "Keyword3", SubMenus = [] },
                new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", Keyword = "Keyword4", SubMenus = [] },
                new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", Keyword = "Keyword5", SubMenus = [] }
            };

            SetupCacheService(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<SearchMenuQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<List<EntityMenu>>(menusFromMediator));

            var request = new SearchMenuQueryRequest { Keyword = "Keyword6" };

            // Act
            var result = await _controller.Search(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var successDataResult = result.Value as SuccessDataResult<List<EntityMenu>>;
            Assert.NotNull(successDataResult);
            Assert.True(successDataResult.IsSuccess);
            Assert.NotNull(successDataResult.Data);
        }

        [Fact]
        public async Task Search_ShouldReturnOk_WhenCacheIsEmpty_AndNoMenusMatchKeyword()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;
            var menusFromMediator = new List<EntityMenu>
            {
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", Keyword = "Keyword1", SubMenus = [] },
                new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", Keyword = "Keyword3", SubMenus = [] },
                new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", Keyword = "Keyword4", SubMenus = [] },
                new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", Keyword = "Keyword5", SubMenus = [] }
            };

            SetupCacheService(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<SearchMenuQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<List<EntityMenu>>(menusFromMediator));

            var request = new SearchMenuQueryRequest { Keyword = "Keyword6" };

            // Act
            var result = await _controller.Search(request) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);

            var errorDataResult = result.Value as ErrorDataResult<List<EntityMenu>>;
            Assert.NotNull(errorDataResult);
            Assert.False(errorDataResult.IsSuccess);
            Assert.Null(errorDataResult.Data);
        }

        [Fact]
        public async Task Search_ShouldReturnOk_WhenCacheIsEmptyAndMediatorReturnsFailure()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;

            SetupCacheService(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<SearchMenuQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ErrorDataResult<List<EntityMenu>>("error message"));

            var request = new SearchMenuQueryRequest { Keyword = "Keyword6" };

            // Act
            var result = await _controller.Search(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var errorDataResult = result.Value as ErrorDataResult<List<EntityMenu>>;
            Assert.NotNull(errorDataResult);
            Assert.False(errorDataResult.IsSuccess);
            Assert.Null(errorDataResult.Data);
        }
    }
}
