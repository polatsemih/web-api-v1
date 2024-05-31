using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SUPBank.Api.Controllers;
using SUPBank.Application.Features.Menu.Commands;
using SUPBank.Application.Features.Menu.Queries;
using SUPBank.Application.Interfaces.Services;
using SUPBank.Application.Validations.Menu;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Results;
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
            _mediatorMock = new();
            _cacheServiceMock = new();
            _controller = new(_mediatorMock.Object, _cacheServiceMock.Object);

            ControllerTestHelper.SetHttpContext(_controller);
        }

        private void SetupCacheService(List<EntityMenu>? cachedMenus)
        {
            _cacheServiceMock.Setup(c => c.GetCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
                             .ReturnsAsync(cachedMenus);
        }


        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenCacheIsNotEmpty_AndReturnMenus()
        {
            // Arrange Data
            GetAllMenuQueryRequest request = new();
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

            // Arrange Service
            SetupCacheService(cachedMenus);

            // Act
            var result = await _controller.GetAll(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            // Assert Data
            var dataResult = result.Value as SuccessDataResult<List<EntityMenu>>;
            Assert.NotNull(dataResult);
            Assert.True(dataResult.IsSuccess);

            Assert.NotNull(dataResult.Data);
            Assert.Equal(cachedMenus, dataResult.Data);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenCacheIsEmpty_AndReturnMenus()
        {
            // Arrange Data
            GetAllMenuQueryRequest request = new();
            List<EntityMenu>? cachedMenus = null;
            List<EntityMenu> menusFromMediator =
            [
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", SubMenus = [] },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", SubMenus = [] },
                new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", SubMenus = [] },
                new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", SubMenus = [] },
                new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", SubMenus = [] }
            ];

            // Arrange Service
            SetupCacheService(cachedMenus);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new SuccessDataResult<List<EntityMenu>>(menusFromMediator));

            // Act
            var result = await _controller.GetAll(request) as OkObjectResult;

            // Assert Result
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            // Assert Data
            var dataResult = result.Value as SuccessDataResult<List<EntityMenu>>;
            Assert.NotNull(dataResult);
            Assert.True(dataResult.IsSuccess);

            Assert.NotNull(dataResult.Data);
            Assert.Equal(menusFromMediator.Where(menu => menu.ParentId == 0), dataResult.Data);
        }

        [Fact]
        public async Task GetAll_ShouldReturnNotFound_WhenCacheIsEmpty_AndReturnNoMenu()
        {
            // Arrange
            GetAllMenuQueryRequest request = new();
            List<EntityMenu>? cachedMenus = null;

            SetupCacheService(cachedMenus);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new ErrorDataResult<List<EntityMenu>>(ResultMessages.MenuNoDatas));

            // Act
            var result = await _controller.GetAll(request) as NotFoundObjectResult;

            // Assert Result
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);

            // Assert Data
            var dataResult = result.Value as ErrorDataResult<List<EntityMenu>>;
            Assert.NotNull(dataResult);
            Assert.False(dataResult.IsSuccess);

            Assert.Equal(ResultMessages.MenuNoDatas, dataResult.Message);
            Assert.Null(dataResult.Data);
        }


        [Fact]
        public async Task GetById_ShouldReturnOk_WhenCacheIsNotEmpty_AndReturnMenu()
        {
            // Arrange
            GetMenuByIdQueryRequest request = new() { Id = 1 };
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

            // Act
            var result = await _controller.GetById(request) as OkObjectResult;

            // Assert Result
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            // Assert Data
            var dataResult = result.Value as SuccessDataResult<EntityMenu>;
            Assert.NotNull(dataResult);
            Assert.True(dataResult.IsSuccess);
            Assert.NotNull(dataResult.Data);
            Assert.Equal(1, dataResult.Data.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenCacheIsEmpty_AndReturnMenu()
        {
            // Arrange
            GetMenuByIdQueryRequest request = new() { Id = 1 };
            List<EntityMenu>? cachedMenus = null;
            EntityMenu menuFromMediator = new() { Id = 1, ParentId = 0, Name_EN = "Menu1" };

            SetupCacheService(cachedMenus);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new SuccessDataResult<EntityMenu>(menuFromMediator));

            // Act
            var result = await _controller.GetById(request) as OkObjectResult;

            // Assert Result
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            // Assert Data
            var dataResult = result.Value as SuccessDataResult<EntityMenu>;
            Assert.NotNull(dataResult);
            Assert.True(dataResult.IsSuccess);
            Assert.NotNull(dataResult.Data);
            Assert.Equal(menuFromMediator, dataResult.Data);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenCacheIsEmpty_AndReturnNoMenu()
        {
            // Arrange
            GetMenuByIdQueryRequest request = new() { Id = 1234 };
            List<EntityMenu>? cachedMenus = null;

            SetupCacheService(cachedMenus);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new ErrorDataResult<EntityMenu>(ResultMessages.MenuNoData));

            // Act
            var result = await _controller.GetById(request) as NotFoundObjectResult;

            // Assert Result
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);

            // Assert Data
            var dataResult = result.Value as ErrorDataResult<EntityMenu>;
            Assert.NotNull(dataResult);
            Assert.False(dataResult.IsSuccess);

            Assert.Equal(ResultMessages.MenuNoData, dataResult.Message);
            Assert.Null(dataResult.Data);
        }

        //[Fact]
        //public async Task GetById_ShouldReturnBadRequest_WhenInvalidIdProvided()
        //{
        //    // Arrange
        //    var request = new GetMenuByIdQueryRequest { Id = 0 };
        //    _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
        //                 .ReturnsAsync(new ErrorDataResult<EntityMenu>("Id must be greater than 0"));

        //    // Act
        //    var result = await _controller.GetById(request) as BadRequestObjectResult;

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(400, result.StatusCode);
        //    Assert.Equal("Id must be greater than 0", (result.Value as ErrorDataResult<EntityMenu>).Message);
        //}



        [Fact]
        public async Task GetByIdWithSubMenus_ShouldReturnOk_WhenCacheIsNotEmpty_AndReturnMenu()
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

            GetMenuByIdWithSubMenusQueryRequest request = new() { Id = 1 };

            // Act
            var result = await _controller.GetByIdWithSubMenus(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var dataResult = result.Value as SuccessDataResult<EntityMenu>;
            Assert.NotNull(dataResult);
            Assert.True(dataResult.IsSuccess);
            Assert.NotNull(dataResult.Data);
            Assert.Equal(1, dataResult.Data.Id);
            Assert.NotEmpty(dataResult.Data.SubMenus);
        }

        [Fact]
        public async Task GetByIdWithSubMenus_ShouldReturnNotFound_WhenCacheIsNotEmpty_AndReturnNoMenu()
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

            GetMenuByIdWithSubMenusQueryRequest request = new() { Id = 1234 };

            // Act
            var result = await _controller.GetByIdWithSubMenus(request) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);

            var dataResult = result.Value as ErrorDataResult<EntityMenu>;
            Assert.NotNull(dataResult);
            Assert.False(dataResult.IsSuccess);
            Assert.Null(dataResult.Data);
        }

        [Fact]
        public async Task GetByIdWithSubMenus_ShouldReturnOk_WhenCacheIsEmpty_AndReturnMenu()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;
            List<EntityMenu> menusFromMediator =
            [
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", SubMenus = [] },
                new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", SubMenus = [] },
                new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", SubMenus = [] },
                new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", SubMenus = [] }
            ];

            SetupCacheService(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMenuByIdWithSubMenusQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<List<EntityMenu>>(menusFromMediator));

            GetMenuByIdWithSubMenusQueryRequest request = new() { Id = 1 };

            // Act
            var result = await _controller.GetByIdWithSubMenus(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var dataResult = result.Value as SuccessDataResult<EntityMenu>;
            Assert.NotNull(dataResult);
            Assert.True(dataResult.IsSuccess);
            Assert.NotNull(dataResult.Data);
            Assert.Equal(1, dataResult.Data.Id);
            Assert.NotEmpty(dataResult.Data.SubMenus);
        }

        [Fact]
        public async Task GetByIdWithSubMenus_ShouldReturnNotFound_WhenCacheIsEmpty_AndReturnNoMenu()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;

            SetupCacheService(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMenuByIdWithSubMenusQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ErrorDataResult<List<EntityMenu>>(ResultMessages.MenuNoData));

            GetMenuByIdWithSubMenusQueryRequest request = new() { Id = 1234 };

            // Act
            var result = await _controller.GetByIdWithSubMenus(request) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);

            var dataResult = result.Value as ErrorDataResult<List<EntityMenu>>;
            Assert.NotNull(dataResult);
            Assert.False(dataResult.IsSuccess);
            Assert.Null(dataResult.Data);
        }

        [Fact]
        public async Task GetByIdWithSubMenus_ShouldReturnBadRequest_WhenCacheIsEmpty_AndReturnNull()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;

            SetupCacheService(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMenuByIdWithSubMenusQueryRequest>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((IDataResult<List<EntityMenu>>?)null!);

            GetMenuByIdWithSubMenusQueryRequest request = new() { Id = 1 };

            // Act
            var result = await _controller.GetByIdWithSubMenus(request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }


        [Fact]
        public async Task Search_ShouldReturnOk_WhenCacheIsNotEmpty_AndReturnMenu()
        {
            // Arrange
            List<EntityMenu> cachedMenus =
            [
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", Keyword = "Keyword1", SubMenus = [
                    new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", Keyword = "Keyword3", SubMenus = [] },
                    new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", Keyword = "Keyword4", SubMenus = [
                        new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", Keyword = "Keyword5", SubMenus = [] }
                    ] }
                ] },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", Keyword = "Keyword2", SubMenus = [] }
            ];

            SetupCacheService(cachedMenus);

            SearchMenuQueryRequest request = new() { Keyword = "Keyword1" };

            // Act
            var result = await _controller.Search(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var dataResult = result.Value as SuccessDataResult<List<EntityMenu>>;
            Assert.NotNull(dataResult);
            Assert.True(dataResult.IsSuccess);
            Assert.NotNull(dataResult.Data);
            Assert.Single(dataResult.Data);
            Assert.Equal(1, dataResult.Data.First().Id);
        }

        [Fact]
        public async Task Search_ShouldReturnOk_WhenCacheIsNotEmpty_AndReturnNoMenu()
        {
            // Arrange
            List<EntityMenu> cachedMenus =
            [
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", Keyword = "Keyword1", SubMenus = [
                    new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", Keyword = "Keyword3", SubMenus = [] },
                    new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", Keyword = "Keyword4", SubMenus = [
                        new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", Keyword = "Keyword5", SubMenus = [] }
                    ] }
                ] },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", Keyword = "Keyword2", SubMenus = [] }
            ];

            SetupCacheService(cachedMenus);

            SearchMenuQueryRequest request = new() { Keyword = "Keyword6" };

            // Act
            var result = await _controller.Search(request) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);

            var dataResult = result.Value as ErrorDataResult<List<EntityMenu>>;
            Assert.NotNull(dataResult);
            Assert.False(dataResult.IsSuccess);
            Assert.Null(dataResult.Data);
        }

        [Fact]
        public async Task Search_ShouldReturnOk_WhenCacheIsEmpty_AndReturnMenu()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;
            List<EntityMenu> menusFromMediator =
            [
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", Keyword = "Keyword1" },
                new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", Keyword = "Keyword3" },
            ];

            SetupCacheService(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<SearchMenuQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<List<EntityMenu>>(menusFromMediator));

            SearchMenuQueryRequest request = new() { Keyword = "Keyword" };

            // Act
            var result = await _controller.Search(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var dataResult = result.Value as SuccessDataResult<List<EntityMenu>>;
            Assert.NotNull(dataResult);
            Assert.True(dataResult.IsSuccess);
            Assert.NotNull(dataResult.Data);
            Assert.Equal(2, dataResult.Data.Count);
            Assert.Contains(dataResult.Data, menu => menu.Id == 1);
            Assert.Contains(dataResult.Data, menu => menu.Id == 3);
        }

        [Fact]
        public async Task Search_ShouldReturnOk_WhenCacheIsEmpty_AndReturnNoMenu()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;

            SetupCacheService(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<SearchMenuQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ErrorDataResult<List<EntityMenu>>(ResultMessages.MenuNoDatas));

            SearchMenuQueryRequest request = new() { Keyword = "Keyword6" };

            // Act
            var result = await _controller.Search(request) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);

            var dataResult = result.Value as ErrorDataResult<List<EntityMenu>>;
            Assert.NotNull(dataResult);
            Assert.False(dataResult.IsSuccess);
            Assert.Null(dataResult.Data);
        }

        [Fact]
        public async Task Search_ShouldReturnBadRequest_WhenCacheIsEmpty_AndReturnNull()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;

            SetupCacheService(cachedMenus);

            _mediatorMock.Setup(m => m.Send(It.IsAny<SearchMenuQueryRequest>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((IDataResult<List<EntityMenu>>?)null!);

            SearchMenuQueryRequest request = new() { Keyword = "Keyword1" };

            // Act
            var result = await _controller.Search(request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }


        [Fact]
        public async Task RemoveCache_ShouldReturnOk_WhenCacheIsEmpty()
        {
            // Arrange
            List<EntityMenu>? cachedMenus = null;

            SetupCacheService(cachedMenus);

            // Act
            var result = await _controller.RemoveCache() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var errorResult = result.Value as ErrorResult;
            Assert.NotNull(errorResult);
            Assert.False(errorResult.IsSuccess);
        }

        [Fact]
        public async Task RemoveCache_ShouldReturnOk_WhenCacheIsNotEmpty_AndCacheRemoved()
        {
            // Arrange
            List<EntityMenu> cachedMenus =
            [
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", Keyword = "Keyword1", SubMenus = [
                    new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", Keyword = "Keyword3", SubMenus = [] },
                    new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", Keyword = "Keyword4", SubMenus = [
                        new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", Keyword = "Keyword5", SubMenus = [] }
                    ] }
                ] },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", Keyword = "Keyword2", SubMenus = [] }
            ];

            SetupCacheService(cachedMenus);
            _cacheServiceMock.Setup(c => c.RemoveCacheAsync(Cache.CacheKeyMenu)).ReturnsAsync(true);

            // Act
            var result = await _controller.RemoveCache() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var successResult = result.Value as SuccessResult;
            Assert.NotNull(successResult);
            Assert.True(successResult.IsSuccess);
        }

        [Fact]
        public async Task RemoveCache_ShouldReturnInternalServerError_WhenCacheIsNotEmpty_AndCacheNotRemoved()
        {
            // Arrange
            List<EntityMenu> cachedMenus =
            [
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", Keyword = "Keyword1", SubMenus = [
                    new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", Keyword = "Keyword3", SubMenus = [] },
                    new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", Keyword = "Keyword4", SubMenus = [
                        new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", Keyword = "Keyword5", SubMenus = [] }
                    ] }
                ] },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", Keyword = "Keyword2", SubMenus = [] }
            ];

            SetupCacheService(cachedMenus);
            _cacheServiceMock.Setup(c => c.RemoveCacheAsync(Cache.CacheKeyMenu)).ReturnsAsync(false);

            // Act
            var result = await _controller.RemoveCache() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);

            var errorResult = result.Value as ErrorResult;
            Assert.NotNull(errorResult);
            Assert.False(errorResult.IsSuccess);
        }


        [Fact]
        public async Task Create_ShouldReturnOk_WhenMenuIsCreated()
        {
            // Arrange
            var request = new CreateMenuCommandRequest
            {
                ParentId = 0,
                Name_TR = "TestMenu_TR",
                Name_EN = "TestMenu_EN",
                ScreenCode = 1,
                Type = 1,
                Priority = 1,
                Keyword = "test-keyword",
                Icon = "test-icon",
                IsGroup = true,
                IsNew = true,
                NewStartDate = DateTime.Now,
                NewEndDate = DateTime.Now.AddDays(1),
                IsActive = true
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateMenuCommandRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SuccessDataResult<long>(ResultMessages.MenuCreateSuccess, 1));

            _cacheServiceMock.Setup(c => c.RemoveCacheAsync(Cache.CacheKeyMenu))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Create(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var dataResult = result.Value as SuccessDataResult<long>;
            Assert.NotNull(dataResult);
            Assert.True(dataResult.IsSuccess);
            Assert.Equal(1, dataResult.Data);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenMenuIsNotCreated()
        {
            // Arrange
            var request = new CreateMenuCommandRequest
            {
                ParentId = 0,
                Name_TR = "TestMenu_TR",
                Name_EN = "TestMenu_EN",
                ScreenCode = 1,
                Type = 1,
                Priority = 1,
                Keyword = "test-keyword",
                Icon = "test-icon",
                IsGroup = false,
                IsNew = true,
                NewStartDate = DateTime.Now,
                NewEndDate = DateTime.Now.AddDays(1),
                IsActive = true
            };

            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ErrorDataResult<long>(ResultMessages.MenuCreateError));

            _cacheServiceMock.Setup(c => c.RemoveCacheAsync(Cache.CacheKeyMenu))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Create(request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);

            var dataResult = result.Value as ErrorDataResult<long>;
            Assert.NotNull(dataResult);
            Assert.False(dataResult.IsSuccess);
        }

        //[Fact]
        //public async Task Create_ShouldReturnBadRequest_WhenInvalidDataProvided()
        //{
        //    // Arrange
        //    var request = new CreateMenuCommandRequest
        //    {
        //        ParentId = -1, // Invalid value
        //        Name_TR = "", // Invalid value
        //        Name_EN = "TestMenu", // Valid value
        //        ScreenCode = 0, // Invalid value
        //        Type = 0, // Invalid value
        //        Priority = -1, // Invalid value
        //        Keyword = "", // Invalid value
        //        Icon = "test-icon", // Valid value
        //        IsGroup = false, // Valid value
        //        IsNew = true, // Valid value
        //        NewStartDate = DateTime.Now,
        //        NewEndDate = DateTime.Now,
        //        IsActive = true // Valid value
        //    };

        //    // Act
        //    Mock<CreateMenuValidator> _validatorMock = new();
        //    var result = await _validatorMock.Setup(c => c.ValidateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync();

        //    // Assert
        //    result.ShouldHaveValidationErrorFor(x => x.ParentId);
        //    result.ShouldHaveValidationErrorFor(x => x.Name_TR);
        //    result.ShouldNotHaveValidationErrorFor(x => x.Name_EN); // This property should be valid
        //    result.ShouldHaveValidationErrorFor(x => x.ScreenCode);
        //    result.ShouldHaveValidationErrorFor(x => x.Type);
        //    result.ShouldHaveValidationErrorFor(x => x.Priority);
        //    result.ShouldHaveValidationErrorFor(x => x.Keyword);
        //    result.ShouldNotHaveValidationErrorFor(x => x.Icon); // This property should be valid
        //    result.ShouldNotHaveValidationErrorFor(x => x.IsGroup); // These properties should be valid
        //    result.ShouldNotHaveValidationErrorFor(x => x.IsNew);
        //    result.ShouldNotHaveValidationErrorFor(x => x.IsActive);
        //}
    }
}
