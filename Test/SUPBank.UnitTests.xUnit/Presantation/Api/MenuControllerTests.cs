using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Xunit;
using Moq;
using MediatR;
using SUPBank.Api.Controllers;
using SUPBank.UnitTests.xUnit.Utilities.Helpers;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Responses;
using SUPBank.Domain.Responses.Data;
using SUPBank.Application.Features.Menu.Queries.Requests;
using SUPBank.Application.Interfaces.Services;
using SUPBank.Application.Interfaces.Services.Controllers;
using SUPBank.Application.Features.Menu.Commands.Requests;

namespace SUPBank.UnitTests.xUnit.Presantation.Api
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


        [Theory]
        [InlineData(0, ValidationMessages.IdEmpty)]
        [InlineData(-1, ValidationMessages.IdPositive)]
        public async Task GetById_ShouldReturnBadRequest_WhenInvalidRequestProvided(long id, string expectedErrorMessage)
        {
            // Arrange Data
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
            Assert.NotEmpty(recursiveMediatorMenu.SubMenus);
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


        [Theory]
        [InlineData("", ValidationMessages.MenuKeywordEmpty)]
        [InlineData("a", ValidationMessages.MenuKeywordMinLength)]
        [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Lorem ipsum dolor sit amet, consectetur adi.", ValidationMessages.MenuKeywordMaxLength)]
        public async Task Search_ShouldReturnBadRequest_WhenInvalidRequestProvided(string keyword, string expectedErrorMessage)
        {
            // Arrange Data
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






        [Theory]
        [InlineData("Keyword", true, 5)]
        [InlineData("Keyword1", false, 1)]
        [InlineData("Keyword2", true, 3)]
        public async Task Search_ShouldReturnOk_WhenCacheIsNotEmpty_AndReturnMenu(string keyword, bool isList, long idOrCount)
        {
            // Arrange Data
            var request = new SearchMenuQueryRequest() { Keyword = keyword };
            var cachedMenus = MenuControllerTestHelper.GetRecursiveMenusMock();

            // Arrange Service
            ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);

            // Act
            var objectResult = await _controller.Search(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkDataResponse<List<EntityMenu>>;
            Assert.NotNull(result);

            Assert.Empty(result.Message);

            Assert.NotNull(result.Data);
            if (isList)
            {
                Assert.Equal(idOrCount, result.Data.Count);
            }
            else
            {
                Assert.Single(result.Data);
                Assert.Equal(idOrCount, result.Data.First().Id);
            }
        }

        [Fact]
        public async Task Search_ShouldReturnOk_WhenCacheIsEmpty_AndReturnMenu()
        {
            // Arrange Data
            var request = new SearchMenuQueryRequest() { Keyword = "Keyword" };
            var cachedMenus = null as List<EntityMenu>;
            var mediatorMenus = MenuControllerTestHelper.GetMenusMock();
            var mediatorResponse = new OkDataResponse<List<EntityMenu>>(mediatorMenus);

            // Arrange Service
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

            Assert.Empty(result.Message);

            Assert.NotNull(result.Data);
            Assert.Equal(5, result.Data.Count);
        }

        [Fact]
        public async Task Search_ShouldReturnOk_WhenCacheIsEmpty_AndReturnNoMenu()
        {
            // Arrange Data
            var request = new SearchMenuQueryRequest() { Keyword = "Keyword6" };
            var cachedMenus = null as List<EntityMenu>;
            var mediatorResponse = new NotFoundResponse(ResultMessages.MenuNoDatas);

            // Arrange Service
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

            Assert.Equal(ResultMessages.MenuNoDatas, result.Message);
        }



        //[Theory]
        //[InlineData("")]
        //[InlineData("a")]
        //[InlineData("ab")]
        //[InlineData("abc")]
        //public async Task Search_ShouldReturnBadRequest_WhenInvalidRequestProvided(string keyword)
        //{
        //    // Arrange Data
        //    SearchMenuQueryRequest request = new() { Keyword = keyword };

        //    // Arrange Service
        //    Mock<AbstractValidator<SearchMenuQueryRequest>> _validatorMock = new();
        //    _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()));

        //    // Act
        //    var objectResult = await _controller.Search(request) as ObjectResult;

        //    // Assert Result
        //    Assert.NotNull(objectResult);
        //    Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

        //    // Assert Data Result
        //    var result = objectResult.Value as BadRequestResponse;
        //    Assert.NotNull(result);

        //    List<string> errorMessages = [
        //        ValidationMessages.MenuKeywordEmpty,
        //        string.Format(ValidationMessages.MenuKeywordMinLength, LengthLimits.MenuKeywordMinLength),
        //        string.Format(ValidationMessages.MenuKeywordMaxLength, LengthLimits.MenuKeywordMaxLength)
        //    ];
        //    Assert.Equal(ResultMessages.MenuNoDatas, result.Message);
        //}

        //[Theory]
        //[InlineData("")]
        //[InlineData("a")]
        //[InlineData("ab")]
        //[InlineData("abc")]
        //public async Task Search_ShouldReturnBadRequest_WhenInvalidRequestProvided(string keyword)
        //{
        //    // Arrange Data
        //    SearchMenuQueryRequest request = new() { Keyword = keyword };
        //    var validationResult = new ValidationResult(new List<ValidationFailure>
        //    {
        //        new ValidationFailure(nameof(request.Keyword), ValidationMessages.MenuKeywordEmpty),
        //        new ValidationFailure(nameof(request.Keyword), string.Format(ValidationMessages.MenuKeywordMinLength, LengthLimits.MenuKeywordMinLength)),
        //        new ValidationFailure(nameof(request.Keyword), string.Format(ValidationMessages.MenuKeywordMaxLength, LengthLimits.MenuKeywordMaxLength))
        //    });
        //    List<string> errorMessages =
        //    [
        //        ValidationMessages.MenuKeywordEmpty,
        //        string.Format(ValidationMessages.MenuKeywordMinLength, LengthLimits.MenuKeywordMinLength),
        //        string.Format(ValidationMessages.MenuKeywordMaxLength, LengthLimits.MenuKeywordMaxLength)
        //    ];

        //    // Arrange Service
        //    Mock<SearchMenuValidator> _validatorMock = new();
        //    _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
        //                 .ReturnsAsync(validationResult);

        //    // Act
        //    var objectResult = await _controller.Search(request) as ObjectResult;

        //    // Assert Result
        //    Assert.NotNull(objectResult);
        //    Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

        //    // Assert Data Result
        //    var result = objectResult.Value as BadRequestResponse;
        //    Assert.NotNull(result);

        //    Assert.Contains(result.Message, errorMessages);
        //}





        //[Fact]
        //public async Task RemoveCache_ShouldReturnOk_WhenCacheIsEmpty()
        //{
        //    // Arrange
        //    var cachedMenus = null as List<EntityMenu>;

        //    ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);

        //    // Act
        //    var objectResult = await _controller.RemoveCache() as ObjectResult;

        //    // Assert
        //    Assert.NotNull(objectResult);
        //    Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

        //    var errorResult = objectResult.Value as ErrorResult;
        //    Assert.NotNull(errorResult);
        //    Assert.False(errorResult.IsSuccess);
        //}

        //[Fact]
        //public async Task RemoveCache_ShouldReturnOk_WhenCacheIsNotEmpty_AndCacheRemoved()
        //{
        //    // Arrange
        //    List<EntityMenu> cachedMenus =
        //    [
        //        new() { Id = 1, ParentId = 0, Name_EN = "Menu1", Keyword = "Keyword1", SubMenus = [
        //            new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", Keyword = "Keyword3", SubMenus = [] },
        //            new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", Keyword = "Keyword4", SubMenus = [
        //                new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", Keyword = "Keyword5", SubMenus = [] }
        //            ] }
        //        ] },
        //        new() { Id = 2, ParentId = 0, Name_EN = "Menu2", Keyword = "Keyword2", SubMenus = [] }
        //    ];

        //    ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);
        //    _cacheServiceMock.Setup(c => c.RemoveCacheAsync(Cache.CacheKeyMenu)).ReturnsAsync(true);

        //    // Act
        //    var objectResult = await _controller.RemoveCache() as ObjectResult;

        //    // Assert
        //    Assert.NotNull(objectResult);
        //    Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

        //    var successResult = objectResult.Value as SuccessResult;
        //    Assert.NotNull(successResult);
        //    Assert.True(successResult.IsSuccess);
        //}

        //[Fact]
        //public async Task RemoveCache_ShouldReturnInternalServerError_WhenCacheIsNotEmpty_AndCacheNotRemoved()
        //{
        //    // Arrange
        //    List<EntityMenu> cachedMenus =
        //    [
        //        new() { Id = 1, ParentId = 0, Name_EN = "Menu1", Keyword = "Keyword1", SubMenus = [
        //            new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", Keyword = "Keyword3", SubMenus = [] },
        //            new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", Keyword = "Keyword4", SubMenus = [
        //                new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", Keyword = "Keyword5", SubMenus = [] }
        //            ] }
        //        ] },
        //        new() { Id = 2, ParentId = 0, Name_EN = "Menu2", Keyword = "Keyword2", SubMenus = [] }
        //    ];

        //    ControllerTestHelper.SetupCacheService(_cacheServiceMock, cachedMenus);
        //    _cacheServiceMock.Setup(c => c.RemoveCacheAsync(Cache.CacheKeyMenu)).ReturnsAsync(false);

        //    // Act
        //    var objectResult = await _controller.RemoveCache() as ObjectResult;

        //    // Assert
        //    Assert.NotNull(objectResult);
        //    Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

        //    var errorResult = objectResult.Value as ErrorResult;
        //    Assert.NotNull(errorResult);
        //    Assert.False(errorResult.IsSuccess);
        //}


        [Theory]
        [InlineData(0, "ParentId", ValidationMessages.MenuParentIdPositiveOrZero)]
        [InlineData(-1, "ParentId", ValidationMessages.MenuParentIdPositiveOrZero)]
        [InlineData(null, "Name_TR", ValidationMessages.MenuNameEmpty)]
        [InlineData("", "Name_TR", ValidationMessages.MenuNameEmpty)]
        [InlineData("a", "Name_TR", ValidationMessages.MenuNameMinLength)]
        [InlineData("thisisaverylongmenunamethatexceedsthemaximumlengthallowedforthemenu", "Name_TR", ValidationMessages.MenuNameMaxLength)]
        [InlineData(null, "Name_EN", ValidationMessages.MenuNameEmpty)]
        [InlineData("", "Name_EN", ValidationMessages.MenuNameEmpty)]
        [InlineData("a", "Name_EN", ValidationMessages.MenuNameMinLength)]
        [InlineData("thisisaverylongmenunamethatexceedsthemaximumlengthallowedforthemenu", "Name_EN", ValidationMessages.MenuNameMaxLength)]
        [InlineData(0, "ScreenCode", ValidationMessages.MenuScreenCodeMinRange)]
        [InlineData(-1, "ScreenCode", ValidationMessages.MenuScreenCodeMinRange)]
        [InlineData(0, "Priority", ValidationMessages.MenuPriorityPositiveOrZero)]
        [InlineData(-1, "Priority", ValidationMessages.MenuPriorityPositiveOrZero)]
        [InlineData(null, "Keyword", ValidationMessages.MenuKeywordEmpty)]
        [InlineData("", "Keyword", ValidationMessages.MenuKeywordEmpty)]
        [InlineData("a", "Keyword", ValidationMessages.MenuKeywordMinLength)]
        [InlineData("thisisaverylongkeywordthatexceedsthemaximumlengthallowedforthekeyword", "Keyword", ValidationMessages.MenuKeywordMaxLength)]
        [InlineData("a", "Icon", ValidationMessages.MenuIconMinLength)]
        [InlineData("thisisaverylongiconthatexceedsthemaximumlengthallowedfortheicon", "Icon", ValidationMessages.MenuIconMaxLength)]
        public async Task CreateMenu_ShouldReturnBadRequest_WhenInvalidRequestProvided(object? value, string field, string expectedErrorMessage)
        {
            // Arrange Data
            var request = new CreateMenuCommandRequest
            {
                ParentId = 1,
                Name_TR = "MenuName_TR",
                Name_EN = "MenuName_EN",
                ScreenCode = 100,
                Type = 1,
                Priority = 1,
                Keyword = "Keyword",
                Icon = "Icon",
                IsGroup = true,
                IsNew = true,
                NewStartDate = DateTime.Now,
                NewEndDate = DateTime.Now.AddDays(1),
                IsActive = true
            };

            // Modify the field based on the test case
            switch (field)
            {
                case "ParentId":
                    request.ParentId = value is long longValue ? longValue : request.ParentId;
                    break;
                case "Name_TR":
                    request.Name_TR = value as string ?? request.Name_TR;
                    break;
                case "Name_EN":
                    request.Name_EN = value as string ?? request.Name_EN;
                    break;
                case "ScreenCode":
                    request.ScreenCode = value is int intValue ? intValue : request.ScreenCode;
                    break;
                case "Priority":
                    request.Priority = value is int intPriority ? intPriority : request.Priority;
                    break;
                case "Keyword":
                    request.Keyword = value as string ?? request.Keyword;
                    break;
                case "Icon":
                    request.Icon = value as string ?? request.Icon;
                    break;
            }

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




        //[Fact]
        //public async Task Create_ShouldReturnOk_WhenMenuIsCreated()
        //{
        //    // Arrange
        //    var request = new CreateMenuCommandRequest
        //    {
        //        ParentId = 0,
        //        Name_TR = "TestMenu_TR",
        //        Name_EN = "TestMenu_EN",
        //        ScreenCode = 1,
        //        Type = 1,
        //        Priority = 1,
        //        Keyword = "test-keyword",
        //        Icon = "test-icon",
        //        IsGroup = true,
        //        IsNew = true,
        //        NewStartDate = DateTime.Now,
        //        NewEndDate = DateTime.Now.AddDays(1),
        //        IsActive = true
        //    };

        //    _mediatorMock.Setup(m => m.Send(It.IsAny<CreateMenuCommandRequest>(), It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(new OkDataResult<long>(ResultMessages.MenuCreateSuccess, 1));

        //    _cacheServiceMock.Setup(c => c.RemoveCacheAsync(Cache.CacheKeyMenu))
        //        .ReturnsAsync(true);

        //    // Act
        //    var objectResult = await _controller.Create(request) as ObjectResult;

        //    // Assert
        //    Assert.NotNull(objectResult);
        //    Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

        //    var result = objectResult.Value as OkDataResult<long>;
        //    Assert.NotNull(result);
        //    Assert.Equal(1, result.Data);
        //}

        //[Fact]
        //public async Task Create_ShouldReturnBadRequest_WhenMenuIsNotCreated()
        //{
        //    // Arrange
        //    var request = new CreateMenuCommandRequest
        //    {
        //        ParentId = 0,
        //        Name_TR = "TestMenu_TR",
        //        Name_EN = "TestMenu_EN",
        //        ScreenCode = 1,
        //        Type = 1,
        //        Priority = 1,
        //        Keyword = "test-keyword",
        //        Icon = "test-icon",
        //        IsGroup = false,
        //        IsNew = true,
        //        NewStartDate = DateTime.Now,
        //        NewEndDate = DateTime.Now.AddDays(1),
        //        IsActive = true
        //    };

        //    _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(new ErrorDataResult<long>(ResultMessages.MenuCreateError));

        //    _cacheServiceMock.Setup(c => c.RemoveCacheAsync(Cache.CacheKeyMenu))
        //        .ReturnsAsync(true);

        //    // Act
        //    var objectResult = await _controller.Create(request) as ObjectResult;

        //    // Assert
        //    Assert.NotNull(objectResult);
        //    Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

        //    var result = objectResult.Value as NotFoundDataResult<long>;
        //    Assert.NotNull(result);
        //}

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
        //    var objectResult = await _validatorMock.Setup(c => c.ValidateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync();

        //    // Assert
        //    objectResult.ShouldHaveValidationErrorFor(x => x.ParentId);
        //    objectResult.ShouldHaveValidationErrorFor(x => x.Name_TR);
        //    objectResult.ShouldNotHaveValidationErrorFor(x => x.Name_EN); // This property should be valid
        //    objectResult.ShouldHaveValidationErrorFor(x => x.ScreenCode);
        //    objectResult.ShouldHaveValidationErrorFor(x => x.Type);
        //    objectResult.ShouldHaveValidationErrorFor(x => x.Priority);
        //    objectResult.ShouldHaveValidationErrorFor(x => x.Keyword);
        //    objectResult.ShouldNotHaveValidationErrorFor(x => x.Icon); // This property should be valid
        //    objectResult.ShouldNotHaveValidationErrorFor(x => x.IsGroup); // These properties should be valid
        //    objectResult.ShouldNotHaveValidationErrorFor(x => x.IsNew);
        //    objectResult.ShouldNotHaveValidationErrorFor(x => x.IsActive);
        //}
    }
}
