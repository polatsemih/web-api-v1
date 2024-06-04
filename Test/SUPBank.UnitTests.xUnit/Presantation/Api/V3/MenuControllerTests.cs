using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Xunit;
using Moq;
using MediatR;
using SUPBank.UnitTests.xUnit.Utilities.Helpers;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Responses;
using SUPBank.Domain.Responses.Data;
using SUPBank.Application.Features.Menu.Queries.Requests;
using SUPBank.Application.Interfaces.Services;
using SUPBank.Application.Interfaces.Services.Controllers;
using SUPBank.Application.Features.Menu.Commands.Requests;
using SUPBank.Application.Interfaces.Repositories;
using AutoMapper;
using SUPBank.Api.Controllers.V3;

namespace SUPBank.UnitTests.xUnit.Presantation.Api.V3
{
    public class MenuControllerTests
    {
        private readonly Mock<IValidationService> _validationServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IRedisCacheService> _cacheServiceMock;
        private readonly Mock<IMenuService> _menuServiceMock;
        private readonly MenuController _controller;

        public MenuControllerTests()
        {
            _validationServiceMock = new();
            _mediatorMock = new();
            _cacheServiceMock = new();
            _menuServiceMock = new();
            _controller = new(_validationServiceMock.Object, _mediatorMock.Object, _cacheServiceMock.Object, _menuServiceMock.Object);

            ControllerTestHelper.SetHttpContext(_controller);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenCacheIsNotEmpty_AndReturnMenus()
        {
            // Arrange Data
            var request = new GetAllMenuQueryRequest();
            var cachedMenus = MenuControllerTestHelper.GetRecursiveMenusMock();

            // Arrange Service
            ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);

            // Act
            var objectResult = await _controller.GetAll(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkDataResponse<List<EntityMenu>>;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Empty(result.Message);

            Assert.NotNull(result.Data);
            Assert.Equal(cachedMenus, result.Data);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenCacheIsEmpty_AndReturnMenus()
        {
            // Arrange Data
            var request = new GetAllMenuQueryRequest();
            var cachedMenus = null as List<EntityMenu>;
            var mediatorMenus = MenuControllerTestHelper.GetMenusMock();
            var mediatorResponse = new OkDataResponse<List<EntityMenu>>(mediatorMenus);
            var recursiveMediatorMenus = MenuControllerTestHelper.GetRecursiveMenusMock();

            // Arrange Service
            ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);
            //ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            MenuControllerTestHelper.SetupMenuServiceRecursiveMenus(_menuServiceMock, mediatorMenus, recursiveMediatorMenus);

            // Act
            var objectResult = await _controller.GetAll(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkDataResponse<List<EntityMenu>>;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Empty(result.Message);

            Assert.NotNull(result.Data);
            Assert.Equal(recursiveMediatorMenus, result.Data);
        }

        [Fact]
        public async Task GetAll_ShouldReturnNotFound_WhenCacheIsEmpty_AndReturnNoMenu()
        {
            // Arrange Data
            var request = new GetAllMenuQueryRequest();
            var cachedMenus = null as List<EntityMenu>;
            var mediatorResponse = new NotFoundResponse(ResultMessages.MenuNoDatas);

            // Arrange Service
            ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);

            // Act
            var objectResult = await _controller.GetAll(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as NotFoundResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.Status);

            Assert.Equal(ResultMessages.MenuNoDatas, result.Message);
        }


        [Fact]
        public async Task GetById_ShouldReturnBadRequest_WhenInvalidRequestProvided()
        {
            // Arrange Data
            long id = -1;
            string expectedErrorMessage = ValidationMessages.IdPositive;
            var request = new GetMenuByIdQueryRequest() { Id = id };

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);

            // Act
            var objectResult = await _controller.GetById(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(expectedErrorMessage, result.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task GetById_ShouldReturnOk_WhenCacheIsNotEmpty_AndReturnMenu(long id)
        {
            // Arrange Data
            var request = new GetMenuByIdQueryRequest() { Id = id };
            var expectedErrorMessage = null as string;
            var cachedMenus = MenuControllerTestHelper.GetRecursiveMenusMock();
            var filteredMenu = MenuControllerTestHelper.GetDynamicMenuMock(id);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);
            MenuControllerTestHelper.SetupMenuServiceFilterRecursiveMenuById(_menuServiceMock, cachedMenus, id, filteredMenu);

            // Act
            var objectResult = await _controller.GetById(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkDataResponse<EntityMenu>;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Empty(result.Message);

            Assert.NotNull(result.Data);
            Assert.Equal(id, result.Data.Id);
            Assert.Equal(filteredMenu, result.Data);
            Assert.Empty(result.Data.SubMenus);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenCacheIsEmpty_AndReturnMenu()
        {
            // Arrange Data
            long id = 1;
            var request = new GetMenuByIdQueryRequest() { Id = id };
            var expectedErrorMessage = null as string;
            var cachedMenus = null as List<EntityMenu>;
            var mediatorMenu = MenuControllerTestHelper.GetMenuMock();
            var mediatorResponse = new OkDataResponse<EntityMenu>(mediatorMenu);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);

            // Act
            var objectResult = await _controller.GetById(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkDataResponse<EntityMenu>;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Empty(result.Message);

            Assert.NotNull(result.Data);
            Assert.Equal(mediatorMenu, result.Data);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenCacheIsEmpty_AndReturnNoMenu()
        {
            // Arrange Data
            var request = new GetMenuByIdQueryRequest() { Id = 1234 };
            var expectedErrorMessage = null as string;
            var cachedMenus = null as List<EntityMenu>;
            var mediatorResponse = new NotFoundResponse(ResultMessages.MenuNoData);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);

            // Act
            var objectResult = await _controller.GetById(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as NotFoundResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.Status);

            Assert.Equal(ResultMessages.MenuNoData, result.Message);
        }


        [Theory]
        [InlineData(0, ValidationMessages.IdEmpty)]
        [InlineData(-1, ValidationMessages.IdPositive)]
        public async Task GetByIdWithSubMenus_ShouldReturnBadRequest_WhenInvalidRequestProvided(long id, string expectedErrorMessage)
        {
            // Arrange Data
            var request = new GetMenuByIdWithSubMenusQueryRequest() { Id = id };

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);

            // Act
            var objectResult = await _controller.GetByIdWithSubMenus(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(expectedErrorMessage, result.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task GetByIdWithSubMenus_ShouldReturnOk_WhenCacheIsNotEmpty_AndReturnMenu(long id)
        {
            // Arrange Data
            var request = new GetMenuByIdWithSubMenusQueryRequest() { Id = id };
            var expectedErrorMessage = null as string;
            var cachedMenus = MenuControllerTestHelper.GetRecursiveMenusMock();
            var filteredMenu = MenuControllerTestHelper.GetDynamicRecursiveMenuMock(id);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);
            MenuControllerTestHelper.SetupMenuServiceFilterRecursiveMenuByIdWithSubMenus(_menuServiceMock, cachedMenus, id, filteredMenu);

            // Act
            var objectResult = await _controller.GetByIdWithSubMenus(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkDataResponse<EntityMenu>;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Empty(result.Message);

            Assert.NotNull(result.Data);
            Assert.Equal(id, result.Data.Id);
            Assert.Equal(filteredMenu, result.Data);
        }

        [Fact]
        public async Task GetByIdWithSubMenus_ShouldReturnOk_WhenCacheIsEmpty_AndReturnMenu()
        {
            // Arrange Data
            long id = 1;
            var request = new GetMenuByIdWithSubMenusQueryRequest() { Id = id };
            var expectedErrorMessage = null as string;
            var cachedMenus = null as List<EntityMenu>;
            var mediatorMenus = MenuControllerTestHelper.GetMenusMock();
            var mediatorResponse = new OkDataResponse<List<EntityMenu>>(mediatorMenus);
            var recursiveMediatorMenu = MenuControllerTestHelper.GetRecursiveMenuMock();

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            MenuControllerTestHelper.SetupMenuServiceRecursiveMenu(_menuServiceMock, mediatorMenus, id, recursiveMediatorMenu);

            // Act
            var objectResult = await _controller.GetByIdWithSubMenus(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkDataResponse<EntityMenu>;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Empty(result.Message);

            Assert.NotNull(result.Data);
            Assert.Equal(recursiveMediatorMenu, result.Data);
            Assert.NotEmpty(result.Data.SubMenus);
        }

        [Fact]
        public async Task GetByIdWithSubMenus_ShouldReturnNotFound_WhenCacheIsEmpty_AndReturnNoMenu()
        {
            // Arrange Data
            var request = new GetMenuByIdWithSubMenusQueryRequest() { Id = 1234 };
            var expectedErrorMessage = null as string;
            var cachedMenus = null as List<EntityMenu>;
            var mediatorResponse = new NotFoundResponse(ResultMessages.MenuNoData);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);

            // Act
            var objectResult = await _controller.GetByIdWithSubMenus(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as NotFoundResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.Status);

            Assert.Equal(ResultMessages.MenuNoData, result.Message);
        }


        [Fact]
        public async Task Search_ShouldReturnBadRequest_WhenInvalidRequestProvided()
        {
            // Arrange Data
            string keyword = "";
            string expectedErrorMessage = ValidationMessages.MenuKeywordEmpty;
            var request = new SearchMenuQueryRequest() { Keyword = keyword };

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);

            // Act
            var objectResult = await _controller.Search(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(expectedErrorMessage, result.Message);
        }

        [Fact]
        public async Task Search_ShouldReturnOk_WhenCacheIsNotEmpty_AndReturnMenu()
        {
            // Arrange Data
            string keyword = "Keyword";
            var request = new SearchMenuQueryRequest() { Keyword = keyword };
            var expectedErrorMessage = null as string;
            var cachedMenus = MenuControllerTestHelper.GetRecursiveMenusMock();
            var filteredMenu = MenuControllerTestHelper.GetMenusMock();

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);
            MenuControllerTestHelper.SetupMenuServiceFilterRecursiveMenusByKeyword(_menuServiceMock, cachedMenus, keyword, filteredMenu);

            // Act
            var objectResult = await _controller.Search(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkDataResponse<List<EntityMenu>>;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Empty(result.Message);

            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task Search_ShouldReturnOk_WhenCacheIsEmpty_AndReturnMenu()
        {
            // Arrange Data
            string keyword = "Keyword";
            var request = new SearchMenuQueryRequest() { Keyword = keyword };
            var expectedErrorMessage = null as string;
            var cachedMenus = null as List<EntityMenu>;
            var mediatorMenus = MenuControllerTestHelper.GetMenusMock();
            var mediatorResponse = new OkDataResponse<List<EntityMenu>>(mediatorMenus);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);

            // Act
            var objectResult = await _controller.Search(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkDataResponse<List<EntityMenu>>;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Empty(result.Message);

            Assert.NotNull(result.Data);
            Assert.Equal(mediatorMenus, result.Data);
        }

        [Fact]
        public async Task Search_ShouldReturnNotFound_WhenCacheIsEmpty_AndReturnNoMenu()
        {
            // Arrange Data
            string keyword = "Keyword";
            var request = new SearchMenuQueryRequest() { Keyword = keyword };
            var expectedErrorMessage = null as string;
            var cachedMenus = null as List<EntityMenu>;
            var mediatorResponse = new NotFoundResponse(ResultMessages.MenuNoData);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);

            // Act
            var objectResult = await _controller.Search(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as NotFoundResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.Status);

            Assert.Equal(ResultMessages.MenuNoData, result.Message);
        }


        [Fact]
        public async Task RemoveCache_ShouldReturnOk_WhenCacheIsEmpty()
        {
            // Arrange Data
            var cachedMenus = null as List<EntityMenu>;

            // Arrange Service
            ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);

            // Act
            var objectResult = await _controller.RemoveCache() as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Equal(ResultMessages.MenuCacheNotExist, result.Message);
        }

        [Fact]
        public async Task RemoveCache_ShouldReturnOk_WhenCacheIsNotEmpty_AndCacheRemoved()
        {
            // Arrange Data
            var cachedMenus = MenuControllerTestHelper.GetRecursiveMenusMock();

            // Arrange Service
            ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);
            _cacheServiceMock.Setup(c => c.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
                             .ReturnsAsync(true);

            // Act
            var objectResult = await _controller.RemoveCache() as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Equal(ResultMessages.MenuCacheRemoved, result.Message);
        }

        [Fact]
        public async Task RemoveCache_ShouldReturnInternalServerError_WhenCacheIsNotEmpty_AndCacheNotRemoved()
        {
            // Arrange Data
            var cachedMenus = MenuControllerTestHelper.GetRecursiveMenusMock();

            // Arrange Service
            ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);
            _cacheServiceMock.Setup(c => c.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
                             .ReturnsAsync(false);

            // Act
            var objectResult = await _controller.RemoveCache() as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as InternalServerErrorResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.Status);

            Assert.Equal(ResultMessages.MenuCacheCouldNotRemoved, result.Message);
        }


        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenInvalidRequestProvided()
        {
            // Arrange Data
            long parentId = -1;
            string expectedErrorMessage = ValidationMessages.MenuParentIdPositiveOrZero;
            var request = new CreateMenuCommandRequest
            {
                ParentId = parentId,
                Name_TR = "MenuName_TR",
                Name_EN = "MenuName_EN",
                ScreenCode = 100000,
                WebURL = "MenuWebURL",
                Type = 10,
                Priority = 100,
                IsSearch = true,
                Keyword = "Keyword",
                Authority = 10,
                Icon = "Icon",
                IsGroup = true,
                IsNew = true,
                NewStartDate = DateTime.Now,
                NewEndDate = DateTime.Now.AddDays(1),
                IsActive = true
            };

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);

            // Act
            var objectResult = await _controller.Create(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(expectedErrorMessage, result.Message);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenGivenParentIdNotExists()
        {
            // Arrange Data
            var request = new CreateMenuCommandRequest
            {
                ParentId = 1,
                Name_TR = "TestMenu_TR",
                Name_EN = "TestMenu_EN",
                ScreenCode = 100000,
                WebURL = "MenuWebURL",
                Type = 10,
                Priority = 100,
                IsSearch = true,
                Keyword = "Keyword",
                Authority = 10,
                Icon = "Icon",
                IsGroup = true,
                IsNew = true,
                NewStartDate = DateTime.Now,
                NewEndDate = DateTime.Now.AddDays(1),
                IsActive = true
            };
            var expectedErrorMessage = null as string;
            var mediatorResponse = new BadRequestResponse(ResultMessages.MenuParentIdNotExist);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsParentIdExistsInMenuAsync(request.ParentId, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _cacheServiceMock.Setup(c => c.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
                             .ReturnsAsync(false);

            // Act
            var objectResult = await _controller.Create(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(ResultMessages.MenuParentIdNotExist, result.Message);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenGivenScreenCodeAlreadyExists()
        {
            // Arrange Data
            var request = new CreateMenuCommandRequest
            {
                ParentId = 1,
                Name_TR = "TestMenu_TR",
                Name_EN = "TestMenu_EN",
                ScreenCode = 100000,
                WebURL = "MenuWebURL",
                Type = 10,
                Priority = 100,
                IsSearch = true,
                Keyword = "Keyword",
                Authority = 10,
                Icon = "Icon",
                IsGroup = true,
                IsNew = true,
                NewStartDate = DateTime.Now,
                NewEndDate = DateTime.Now.AddDays(1),
                IsActive = true
            };
            var expectedErrorMessage = null as string;
            var mediatorResponse = new BadRequestResponse(ResultMessages.MenuScreenCodeAlreadyExists);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsParentIdExistsInMenuAsync(request.ParentId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _menuQueryRepositoryMock.Setup(m => m.IsScreenCodeExistsInMenuAsync(request.ScreenCode.Value, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _cacheServiceMock.Setup(c => c.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
                             .ReturnsAsync(false);

            // Act
            var objectResult = await _controller.Create(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(ResultMessages.MenuScreenCodeAlreadyExists, result.Message);
        }

        [Fact]
        public async Task Create_ShouldReturnInternalServerError_WhenMenuIsNotCreated()
        {
            // Arrange Data
            var request = new CreateMenuCommandRequest
            {
                ParentId = 0,
                Name_TR = "TestMenu_TR",
                Name_EN = "TestMenu_EN",
                ScreenCode = 100000,
                WebURL = "MenuWebURL",
                Type = 10,
                Priority = 100,
                IsSearch = true,
                Keyword = "Keyword",
                Authority = 10,
                Icon = "Icon",
                IsGroup = true,
                IsNew = true,
                NewStartDate = DateTime.Now,
                NewEndDate = DateTime.Now.AddDays(1),
                IsActive = true
            };
            var expectedErrorMessage = null as string;
            var mediatorResponse = new InternalServerErrorResponse(ResultMessages.MenuCreateError);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsParentIdExistsInMenuAsync(request.ParentId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _menuQueryRepositoryMock.Setup(m => m.IsScreenCodeExistsInMenuAsync(request.ScreenCode.Value, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _cacheServiceMock.Setup(c => c.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
                             .ReturnsAsync(false);

            // Act
            var objectResult = await _controller.Create(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as InternalServerErrorResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.Status);

            Assert.Equal(ResultMessages.MenuCreateError, result.Message);
        }

        [Fact]
        public async Task Create_ShouldReturnOk_WhenMenuIsCreated()
        {
            // Arrange Data
            var request = new CreateMenuCommandRequest
            {
                ParentId = 0,
                Name_TR = "TestMenu_TR",
                Name_EN = "TestMenu_EN",
                ScreenCode = 100000,
                WebURL = "MenuWebURL",
                Type = 10,
                Priority = 100,
                IsSearch = true,
                Keyword = "Keyword",
                Authority = 10,
                Icon = "Icon",
                IsGroup = true,
                IsNew = true,
                NewStartDate = DateTime.Now,
                NewEndDate = DateTime.Now.AddDays(1),
                IsActive = true
            };
            var expectedErrorMessage = null as string;
            long id = 1;
            var menu = new EntityMenu();
            var mediatorResponse = new OkDataResponse<long>(ResultMessages.MenuCreateSuccess, id);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsParentIdExistsInMenuAsync(request.ParentId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _menuQueryRepositoryMock.Setup(m => m.IsScreenCodeExistsInMenuAsync(request.ScreenCode.Value, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            Mock<IMapper> _mapperMock = new();
            _mapperMock.Setup(m => m.Map<EntityMenu>(request)).Returns(menu);
            Mock<IMenuCommandRepository> _menuCommandRepository = new();
            _menuCommandRepository.Setup(m => m.CreateMenuAndGetIdAsync(menu, It.IsAny<CancellationToken>())).ReturnsAsync(id);
            _cacheServiceMock.Setup(c => c.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
                             .ReturnsAsync(true);

            // Act
            var objectResult = await _controller.Create(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkDataResponse<long>;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Equal(ResultMessages.MenuCreateSuccess, result.Message);

            Assert.Equal(id, result.Data);
        }


        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenInvalidRequestProvided()
        {
            // Arrange Data
            long parentId = -1;
            string expectedErrorMessage = ValidationMessages.MenuParentIdPositiveOrZero;
            var request = new UpdateMenuCommandRequest
            {
                Id = 1,
                ParentId = parentId,
                Name_TR = "MenuName_TR",
                Name_EN = "MenuName_EN",
                ScreenCode = 100000,
                WebURL = "MenuWebURL",
                Type = 10,
                Priority = 100,
                IsSearch = true,
                Keyword = "Keyword",
                Authority = 10,
                Icon = "Icon",
                IsGroup = true,
                IsNew = true,
                NewStartDate = DateTime.Now,
                NewEndDate = DateTime.Now.AddDays(1),
                IsActive = true
            };

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);

            // Act
            var objectResult = await _controller.Update(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(expectedErrorMessage, result.Message);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenGivenIdNotExists()
        {
            // Arrange Data
            var request = new UpdateMenuCommandRequest
            {
                Id = 1,
                ParentId = 1,
                Name_TR = "TestMenu_TR",
                Name_EN = "TestMenu_EN",
                ScreenCode = 100000,
                WebURL = "MenuWebURL",
                Type = 10,
                Priority = 100,
                IsSearch = true,
                Keyword = "Keyword",
                Authority = 10,
                Icon = "Icon",
                IsGroup = true,
                IsNew = true,
                NewStartDate = DateTime.Now,
                NewEndDate = DateTime.Now.AddDays(1),
                IsActive = true
            };
            var expectedErrorMessage = null as string;
            var mediatorResponse = new BadRequestResponse(ResultMessages.MenuIdNotExist);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsIdExistsInMenuAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _cacheServiceMock.Setup(c => c.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
                             .ReturnsAsync(false);

            // Act
            var objectResult = await _controller.Update(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(ResultMessages.MenuIdNotExist, result.Message);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenGivenParentIdNotExists()
        {
            // Arrange Data
            var request = new UpdateMenuCommandRequest
            {
                Id = 1,
                ParentId = 1,
                Name_TR = "TestMenu_TR",
                Name_EN = "TestMenu_EN",
                ScreenCode = 100000,
                WebURL = "MenuWebURL",
                Type = 10,
                Priority = 100,
                IsSearch = true,
                Keyword = "Keyword",
                Authority = 10,
                Icon = "Icon",
                IsGroup = true,
                IsNew = true,
                NewStartDate = DateTime.Now,
                NewEndDate = DateTime.Now.AddDays(1),
                IsActive = true
            };
            var expectedErrorMessage = null as string;
            var mediatorResponse = new BadRequestResponse(ResultMessages.MenuParentIdNotExist);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsIdExistsInMenuAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _menuQueryRepositoryMock.Setup(m => m.IsParentIdExistsInMenuAsync(request.ParentId, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _cacheServiceMock.Setup(c => c.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
                             .ReturnsAsync(false);

            // Act
            var objectResult = await _controller.Update(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(ResultMessages.MenuParentIdNotExist, result.Message);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenGivenScreenCodeAlreadyExists()
        {
            // Arrange Data
            var request = new UpdateMenuCommandRequest
            {
                Id = 1,
                ParentId = 1,
                Name_TR = "TestMenu_TR",
                Name_EN = "TestMenu_EN",
                ScreenCode = 100000,
                WebURL = "MenuWebURL",
                Type = 10,
                Priority = 100,
                IsSearch = true,
                Keyword = "Keyword",
                Authority = 10,
                Icon = "Icon",
                IsGroup = true,
                IsNew = true,
                NewStartDate = DateTime.Now,
                NewEndDate = DateTime.Now.AddDays(1),
                IsActive = true
            };
            var expectedErrorMessage = null as string;
            var menuWithScreenCode = new EntityMenu()
            {
                ScreenCode = 100001
            };
            var mediatorResponse = new BadRequestResponse(ResultMessages.MenuScreenCodeAlreadyExists);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsIdExistsInMenuAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _menuQueryRepositoryMock.Setup(m => m.IsParentIdExistsInMenuAsync(request.ParentId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _menuQueryRepositoryMock.Setup(m => m.GetMenuScreenCodeByIdAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(menuWithScreenCode);
            _menuQueryRepositoryMock.Setup(m => m.IsScreenCodeExistsInMenuAsync(menuWithScreenCode.ScreenCode.Value, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _cacheServiceMock.Setup(c => c.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
                             .ReturnsAsync(false);

            // Act
            var objectResult = await _controller.Update(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(ResultMessages.MenuScreenCodeAlreadyExists, result.Message);
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenMenuIsNotUpdated()
        {
            // Arrange Data
            var request = new UpdateMenuCommandRequest
            {
                Id = 1,
                ParentId = 0,
                Name_TR = "TestMenu_TR",
                Name_EN = "TestMenu_EN",
                ScreenCode = 100000,
                WebURL = "MenuWebURL",
                Type = 10,
                Priority = 100,
                IsSearch = true,
                Keyword = "Keyword",
                Authority = 10,
                Icon = "Icon",
                IsGroup = true,
                IsNew = true,
                NewStartDate = DateTime.Now,
                NewEndDate = DateTime.Now.AddDays(1),
                IsActive = true
            };
            var expectedErrorMessage = null as string;
            var menuWithScreenCode = new EntityMenu()
            {
                ScreenCode = 100001
            };
            var menu = new EntityMenu();
            var mediatorResponse = new OkResponse(ResultMessages.MenuUpdateNoChanges);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsIdExistsInMenuAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _menuQueryRepositoryMock.Setup(m => m.IsParentIdExistsInMenuAsync(request.ParentId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _menuQueryRepositoryMock.Setup(m => m.GetMenuScreenCodeByIdAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(menuWithScreenCode);
            _menuQueryRepositoryMock.Setup(m => m.IsScreenCodeExistsInMenuAsync(menuWithScreenCode.ScreenCode.Value, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            Mock<IMapper> _mapperMock = new();
            _mapperMock.Setup(m => m.Map<EntityMenu>(request)).Returns(menu);
            Mock<IMenuCommandRepository> _menuCommandRepository = new();
            _menuCommandRepository.Setup(m => m.UpdateMenuAsync(menu, It.IsAny<CancellationToken>())).ReturnsAsync(0);
            _cacheServiceMock.Setup(c => c.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
                             .ReturnsAsync(false);

            // Act
            var objectResult = await _controller.Update(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Equal(ResultMessages.MenuUpdateNoChanges, result.Message);
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenMenuIsUpdated()
        {
            // Arrange Data
            var request = new UpdateMenuCommandRequest
            {
                Id = 1,
                ParentId = 0,
                Name_TR = "TestMenu_TR",
                Name_EN = "TestMenu_EN",
                WebURL = "MenuWebURL",
                Type = 10,
                Priority = 100,
                IsSearch = true,
                Keyword = "Keyword",
                Authority = 10,
                Icon = "Icon",
                IsGroup = true,
                IsNew = true,
                NewStartDate = DateTime.Now,
                NewEndDate = DateTime.Now.AddDays(1),
                IsActive = true
            };
            var expectedErrorMessage = null as string;
            var menuWithScreenCode = new EntityMenu()
            {
                ScreenCode = 100001
            };
            var menu = new EntityMenu();
            var mediatorResponse = new OkResponse(ResultMessages.MenuUpdateSuccess);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsIdExistsInMenuAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _menuQueryRepositoryMock.Setup(m => m.IsParentIdExistsInMenuAsync(request.ParentId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _menuQueryRepositoryMock.Setup(m => m.GetMenuScreenCodeByIdAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(menuWithScreenCode);
            _menuQueryRepositoryMock.Setup(m => m.IsScreenCodeExistsInMenuAsync(menuWithScreenCode.ScreenCode.Value, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            Mock<IMapper> _mapperMock = new();
            _mapperMock.Setup(m => m.Map<EntityMenu>(request)).Returns(menu);
            Mock<IMenuCommandRepository> _menuCommandRepository = new();
            _menuCommandRepository.Setup(m => m.UpdateMenuAsync(menu, It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _cacheServiceMock.Setup(c => c.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
                             .ReturnsAsync(true);

            // Act
            var objectResult = await _controller.Update(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Equal(ResultMessages.MenuUpdateSuccess, result.Message);
        }


        [Fact]
        public async Task Delete_ShouldReturnBadRequest_WhenInvalidRequestProvided()
        {
            // Arrange Data
            long id = -1;
            string expectedErrorMessage = ValidationMessages.IdPositive;
            var request = new DeleteMenuCommandRequest { Id = id };

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);

            // Act
            var objectResult = await _controller.Delete(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(expectedErrorMessage, result.Message);
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest_WhenGivenIdNotExists()
        {
            // Arrange Data
            var request = new DeleteMenuCommandRequest { Id = 1 };
            var expectedErrorMessage = null as string;
            var mediatorResponse = new BadRequestResponse(ResultMessages.MenuIdNotExist);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsIdExistsInMenuAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _cacheServiceMock.Setup(c => c.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
                             .ReturnsAsync(false);

            // Act
            var objectResult = await _controller.Delete(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(ResultMessages.MenuIdNotExist, result.Message);
        }

        [Fact]
        public async Task Delete_ShouldReturnOk_WhenMenuIsNotDeleted()
        {
            // Arrange Data
            var request = new DeleteMenuCommandRequest { Id = 1 };
            var expectedErrorMessage = null as string;
            var menu = new EntityMenu();
            var mediatorResponse = new BadRequestResponse(ResultMessages.MenuDeleteError);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsIdExistsInMenuAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            Mock<IMenuCommandRepository> _menuCommandRepository = new();
            _menuCommandRepository.Setup(m => m.DeleteMenuAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _cacheServiceMock.Setup(c => c.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
                             .ReturnsAsync(false);

            // Act
            var objectResult = await _controller.Delete(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(ResultMessages.MenuDeleteError, result.Message);
        }

        [Fact]
        public async Task Delete_ShouldReturnOk_WhenMenuIsDeleted()
        {
            // Arrange Data
            var request = new DeleteMenuCommandRequest { Id = 1 };
            var expectedErrorMessage = null as string;
            var menu = new EntityMenu();
            var mediatorResponse = new OkResponse(ResultMessages.MenuDeleteSuccess);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsIdExistsInMenuAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            Mock<IMenuCommandRepository> _menuCommandRepository = new();
            _menuCommandRepository.Setup(m => m.DeleteMenuAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _cacheServiceMock.Setup(c => c.RemoveCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
                             .ReturnsAsync(true);

            // Act
            var objectResult = await _controller.Delete(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Equal(ResultMessages.MenuDeleteSuccess, result.Message);
        }


        [Fact]
        public async Task RolllbackById_ShouldReturnBadRequest_WhenInvalidRequestProvided()
        {
            // Arrange Data
            long id = -1;
            string expectedErrorMessage = ValidationMessages.IdPositive;
            var request = new RollbackMenuByIdCommandRequest { Id = id };

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);

            // Act
            var objectResult = await _controller.RolllbackById(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(expectedErrorMessage, result.Message);
        }

        [Fact]
        public async Task RolllbackById_ShouldReturnBadRequest_WhenGivenIdNotExists()
        {
            // Arrange Data
            var request = new RollbackMenuByIdCommandRequest { Id = 1 };
            var expectedErrorMessage = null as string;
            var mediatorResponse = new BadRequestResponse(ResultMessages.MenuIdNotExist);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsMenuIdExistsInMenuHAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var objectResult = await _controller.RolllbackById(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(ResultMessages.MenuIdNotExist, result.Message);
        }

        [Fact]
        public async Task RolllbackById_ShouldReturnOk_WhenMenuIsNotRollbacked()
        {
            // Arrange Data
            var request = new RollbackMenuByIdCommandRequest { Id = 1 };
            var expectedErrorMessage = null as string;
            var mediatorResponse = new OkResponse(ResultMessages.MenuRollbackNoChanges);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsMenuIdExistsInMenuHAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            Mock<IMenuCommandRepository> _menuCommandRepository = new();
            _menuCommandRepository.Setup(m => m.RollbackMenuByIdAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(0);

            // Act
            var objectResult = await _controller.RolllbackById(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Equal(ResultMessages.MenuRollbackNoChanges, result.Message);
        }

        [Fact]
        public async Task RolllbackById_ShouldReturnOk_WhenMenuIsRollbacked()
        {
            // Arrange Data
            var request = new RollbackMenuByIdCommandRequest { Id = 1 };
            var expectedErrorMessage = null as string;
            var mediatorResponse = new OkResponse(ResultMessages.MenuRollbackSuccess);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsMenuIdExistsInMenuHAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            Mock<IMenuCommandRepository> _menuCommandRepository = new();
            _menuCommandRepository.Setup(m => m.RollbackMenuByIdAsync(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var objectResult = await _controller.RolllbackById(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Equal(ResultMessages.MenuRollbackSuccess, result.Message);
        }


        [Fact]
        public async Task RolllbackByScreenCode_ShouldReturnBadRequest_WhenInvalidRequestProvided()
        {
            // Arrange Data
            int screenCode = 1;
            string expectedErrorMessage = ValidationMessages.MenuScreenCodeMinRange;
            var request = new RollbackMenuByScreenCodeCommandRequest { ScreenCode = screenCode };

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);

            // Act
            var objectResult = await _controller.RolllbackByScreenCode(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(expectedErrorMessage, result.Message);
        }

        [Fact]
        public async Task RolllbackByScreenCode_ShouldReturnBadRequest_WhenGivenIdNotExists()
        {
            // Arrange Data
            var request = new RollbackMenuByScreenCodeCommandRequest { ScreenCode = 10001 };
            var expectedErrorMessage = null as string;
            var mediatorResponse = new BadRequestResponse(ResultMessages.MenuScreenCodeNotExistInHistory);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsScreenCodeExistsInMenuHAsync(request.ScreenCode, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var objectResult = await _controller.RolllbackByScreenCode(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(ResultMessages.MenuScreenCodeNotExistInHistory, result.Message);
        }

        [Fact]
        public async Task RolllbackByScreenCode_ShouldReturnOk_WhenMenuIsNotRollbacked()
        {
            // Arrange Data
            var request = new RollbackMenuByScreenCodeCommandRequest { ScreenCode = 10001 };
            var expectedErrorMessage = null as string;
            var mediatorResponse = new OkResponse(ResultMessages.MenuRollbackNoChanges);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsScreenCodeExistsInMenuHAsync(request.ScreenCode, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            Mock<IMenuCommandRepository> _menuCommandRepository = new();
            _menuCommandRepository.Setup(m => m.RollbackMenuByScreenCodeAsync(request.ScreenCode, It.IsAny<CancellationToken>())).ReturnsAsync(0);

            // Act
            var objectResult = await _controller.RolllbackByScreenCode(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Equal(ResultMessages.MenuRollbackNoChanges, result.Message);
        }

        [Fact]
        public async Task RolllbackByScreenCode_ShouldReturnOk_WhenMenuIsRollbacked()
        {
            // Arrange Data
            var request = new RollbackMenuByScreenCodeCommandRequest { ScreenCode = 10001 };
            var expectedErrorMessage = null as string;
            var mediatorResponse = new OkResponse(ResultMessages.MenuRollbackSuccess);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsScreenCodeExistsInMenuHAsync(request.ScreenCode, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            Mock<IMenuCommandRepository> _menuCommandRepository = new();
            _menuCommandRepository.Setup(m => m.RollbackMenuByScreenCodeAsync(request.ScreenCode, It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var objectResult = await _controller.RolllbackByScreenCode(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Equal(ResultMessages.MenuRollbackSuccess, result.Message);
        }


        [Fact]
        public async Task RolllbackByToken_ShouldReturnBadRequest_WhenInvalidRequestProvided()
        {
            // Arrange Data
            Guid rollbackToken = Guid.NewGuid();
            string expectedErrorMessage = ValidationMessages.RollbackTokenEmpty;
            var request = new RollbackMenusByTokenCommandRequest { RollbackToken = rollbackToken };

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);

            // Act
            var objectResult = await _controller.RolllbackByToken(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(expectedErrorMessage, result.Message);
        }

        [Fact]
        public async Task RolllbackByToken_ShouldReturnBadRequest_WhenGivenIdNotExists()
        {
            // Arrange Data
            var request = new RollbackMenusByTokenCommandRequest { RollbackToken = Guid.NewGuid() };
            var expectedErrorMessage = null as string;
            var mediatorResponse = new BadRequestResponse(ResultMessages.MenuRollbackTokenNotExistInHistory);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsRollbackTokenExistsInMenuHAsync(request.RollbackToken, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var objectResult = await _controller.RolllbackByToken(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as BadRequestResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);

            Assert.Equal(ResultMessages.MenuRollbackTokenNotExistInHistory, result.Message);
        }

        [Fact]
        public async Task RolllbackByToken_ShouldReturnOk_WhenMenuIsNotRollbacked()
        {
            // Arrange Data
            var request = new RollbackMenusByTokenCommandRequest { RollbackToken = Guid.NewGuid() };
            var expectedErrorMessage = null as string;
            var mediatorResponse = new OkResponse(ResultMessages.MenuRollbackNoChanges);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsRollbackTokenExistsInMenuHAsync(request.RollbackToken, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            Mock<IMenuCommandRepository> _menuCommandRepository = new();
            _menuCommandRepository.Setup(m => m.RollbackMenusByTokenAsync(request.RollbackToken, It.IsAny<CancellationToken>())).ReturnsAsync(0);

            // Act
            var objectResult = await _controller.RolllbackByToken(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Equal(ResultMessages.MenuRollbackNoChanges, result.Message);
        }

        [Fact]
        public async Task RolllbackByToken_ShouldReturnOk_WhenMenuIsRollbacked()
        {
            // Arrange Data
            var request = new RollbackMenusByTokenCommandRequest { RollbackToken = Guid.NewGuid() };
            var expectedErrorMessage = null as string;
            var mediatorResponse = new OkResponse(ResultMessages.MenuRollbackSuccess);

            // Arrange Service
            ControllerTestHelper.SetupValidationService(_validationServiceMock, request, expectedErrorMessage);
            // ControllerTestHelper.SetupMediator(_mediatorMock, request, mediatorResponse);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(mediatorResponse);
            Mock<IMenuQueryRepository> _menuQueryRepositoryMock = new();
            _menuQueryRepositoryMock.Setup(m => m.IsRollbackTokenExistsInMenuHAsync(request.RollbackToken, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            Mock<IMenuCommandRepository> _menuCommandRepository = new();
            _menuCommandRepository.Setup(m => m.RollbackMenusByTokenAsync(request.RollbackToken, It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var objectResult = await _controller.RolllbackByToken(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkResponse;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.Status);

            Assert.Equal(ResultMessages.MenuRollbackSuccess, result.Message);
        }
    }
}
