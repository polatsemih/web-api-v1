using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SUPBank.Api.Controllers;
using SUPBank.Application.Features.Menu.Queries.Requests;
using SUPBank.Application.Features.Menu.Commands.Requests;
using SUPBank.Application.Interfaces.Services;
using SUPBank.Application.Validations.Menu;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Responses;
using SUPBank.Domain.Responses.Data;
using SUPBank.Infrastructure.Services;
using SUPBank.UnitTests.xUnit.Utilities.Helpers;
using Xunit;

namespace SUPBank.UnitTests.xUnit.Presantation.Api
{
    public class MenuControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IRedisCacheService> _cacheServiceMock;
        private readonly Mock<IValidationService> _validationServiceMock;
        private readonly MenuController _controller;

        public MenuControllerTests()
        {
            _mediatorMock = new();
            _cacheServiceMock = new();
            _validationServiceMock = new();
            _controller = new(_mediatorMock.Object, _cacheServiceMock.Object, _validationServiceMock.Object);

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
            var objectResult = await _controller.GetAll(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(200, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkDataResponse<List<EntityMenu>>;
            Assert.NotNull(result);

            Assert.Empty(result.Message);

            Assert.NotNull(result.Data);
            Assert.Equal(cachedMenus, result.Data);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenCacheIsEmpty_AndReturnMenus()
        {
            // Arrange Data
            GetAllMenuQueryRequest request = new();
            List<EntityMenu>? cachedMenus = null;
            List<EntityMenu> menusFromMediator =
            [
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1" },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2" },
                new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1" },
                new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2" },
                new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1" }
            ];

            // Arrange Service
            SetupCacheService(cachedMenus);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new OkDataResponse<List<EntityMenu>>(menusFromMediator));

            // Act
            var objectResult = await _controller.GetAll(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(200, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkDataResponse<List<EntityMenu>>;
            Assert.NotNull(result);

            Assert.Empty(result.Message);

            Assert.NotNull(result.Data);
            Assert.Equal(menusFromMediator.Where(menu => menu.ParentId == 0), result.Data);
        }

        [Fact]
        public async Task GetAll_ShouldReturnNotFound_WhenCacheIsEmpty_AndReturnNoMenu()
        {
            // Arrange Data
            GetAllMenuQueryRequest request = new();
            List<EntityMenu>? cachedMenus = null;

            // Arrange Service
            SetupCacheService(cachedMenus);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new NotFoundResponse(ResultMessages.MenuNoDatas));

            // Act
            var objectResult = await _controller.GetAll(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(404, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as NotFoundResponse;
            Assert.NotNull(result);

            Assert.Equal(ResultMessages.MenuNoDatas, result.Message);
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
            GetMenuByIdQueryRequest request = new() { Id = id };
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
            var objectResult = await _controller.GetById(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(200, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkDataResponse<EntityMenu>;
            Assert.NotNull(result);

            Assert.Empty(result.Message);

            Assert.NotNull(result.Data);
            Assert.Equal(id, result.Data.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenCacheIsEmpty_AndReturnMenu()
        {
            // Arrange Data
            GetMenuByIdQueryRequest request = new() { Id = 1 };
            List<EntityMenu>? cachedMenus = null;
            EntityMenu menuFromMediator = new() { Id = 1, ParentId = 0, Name_EN = "Menu1" };

            // Arrange Service
            SetupCacheService(cachedMenus);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new OkDataResponse<EntityMenu>(menuFromMediator));

            // Act
            var objectResult = await _controller.GetById(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(200, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkDataResponse<EntityMenu>;
            Assert.NotNull(result);

            Assert.Empty(result.Message);

            Assert.NotNull(result.Data);
            Assert.Equal(menuFromMediator, result.Data);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenCacheIsEmpty_AndReturnNoMenu()
        {
            // Arrange Data
            GetMenuByIdQueryRequest request = new() { Id = 1234 };
            List<EntityMenu>? cachedMenus = null;

            // Arrange Service
            SetupCacheService(cachedMenus);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new NotFoundResponse(ResultMessages.MenuNoData));

            // Act
            var objectResult = await _controller.GetById(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(404, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as NotFoundResponse;
            Assert.NotNull(result);

            Assert.Equal(ResultMessages.MenuNoData, result.Message);
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
            GetMenuByIdWithSubMenusQueryRequest request = new() { Id = id };
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
            var objectResult = await _controller.GetByIdWithSubMenus(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(200, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkDataResponse<EntityMenu>;
            Assert.NotNull(result);

            Assert.Empty(result.Message);

            Assert.NotNull(result.Data);
            Assert.Equal(id, result.Data.Id);
            if (id == 1 || id == 4)
            {
                Assert.NotEmpty(result.Data.SubMenus);
            }
            else
            {
                Assert.Empty(result.Data.SubMenus);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public async Task GetByIdWithSubMenus_ShouldReturnOk_WhenCacheIsEmpty_AndReturnMenu(long id)
        {
            // Arrange Data
            GetMenuByIdWithSubMenusQueryRequest request = new() { Id = id };
            List<EntityMenu>? cachedMenus = null;
            List<EntityMenu> menusFromMediator =
            [
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1" },
                new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1" },
                new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2" }
            ];

            // Arrange Service
            SetupCacheService(cachedMenus);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new OkDataResponse<List<EntityMenu>>(menusFromMediator));

            // Act
            var objectResult = await _controller.GetByIdWithSubMenus(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(200, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as OkDataResponse<EntityMenu>;
            Assert.NotNull(result);

            Assert.Empty(result.Message);

            Assert.NotNull(result.Data);
            Assert.Equal(id, result.Data.Id);
            if (id == 1)
            {
                Assert.NotEmpty(result.Data.SubMenus);
            }
            else
            {
                Assert.Empty(result.Data.SubMenus);
            }
        }

        [Fact]
        public async Task GetByIdWithSubMenus_ShouldReturnNotFound_WhenCacheIsEmpty_AndReturnNoMenu()
        {
            // Arrange Data
            GetMenuByIdWithSubMenusQueryRequest request = new() { Id = 1234 };
            List<EntityMenu>? cachedMenus = null;

            // Arrange Service
            SetupCacheService(cachedMenus);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new NotFoundResponse(ResultMessages.MenuNoData));

            // Act
            var objectResult = await _controller.GetByIdWithSubMenus(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(404, objectResult.StatusCode);

            // Assert Data Result
            var result = objectResult.Value as NotFoundResponse;
            Assert.NotNull(result);

            Assert.Equal(ResultMessages.MenuNoData, result.Message);
        }


        [Theory]
        [InlineData("Keyword", true, 5)]
        [InlineData("Keyword1", false, 1)]
        [InlineData("Keyword2", true, 3)]
        public async Task Search_ShouldReturnOk_WhenCacheIsNotEmpty_AndReturnMenu(string keyword, bool isList, long idOrCount)
        {
            // Arrange Data
            SearchMenuQueryRequest request = new() { Keyword = keyword };
            List<EntityMenu> cachedMenus =
            [
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", Keyword = "Keyword1", SubMenus = [
                    new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", Keyword = "Keyword3", SubMenus = [] },
                    new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", Keyword = "Keyword2", SubMenus = [
                        new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", Keyword = "Keyword2", SubMenus = [] }
                    ] }
                ] },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", Keyword = "Keyword2", SubMenus = [] }
            ];

            // Arrange Service
            SetupCacheService(cachedMenus);

            // Act
            var objectResult = await _controller.Search(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(200, objectResult.StatusCode);

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
            SearchMenuQueryRequest request = new() { Keyword = "Keyword" };
            List<EntityMenu>? cachedMenus = null;
            List<EntityMenu> menusFromMediator =
            [
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", Keyword = "Keyword1" },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", Keyword = "Keyword2" },
                new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", Keyword = "Keyword3" },
                new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", Keyword = "Keyword4" },
                new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", Keyword = "Keyword5" }
            ];

            // Arrange Service
            SetupCacheService(cachedMenus);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new OkDataResponse<List<EntityMenu>>(menusFromMediator));

            // Act
            var objectResult = await _controller.Search(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(200, objectResult.StatusCode);

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
            SearchMenuQueryRequest request = new() { Keyword = "Keyword6" };
            List<EntityMenu>? cachedMenus = null;

            // Arrange Service
            SetupCacheService(cachedMenus);
            _mediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new NotFoundResponse(ResultMessages.MenuNoDatas));

            // Act
            var objectResult = await _controller.Search(request) as ObjectResult;

            // Assert Result
            Assert.NotNull(objectResult);
            Assert.Equal(404, objectResult.StatusCode);

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
        //    Assert.Equal(400, objectResult.StatusCode);

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
        //    Assert.Equal(400, objectResult.StatusCode);

        //    // Assert Data Result
        //    var result = objectResult.Value as BadRequestResponse;
        //    Assert.NotNull(result);

        //    Assert.Contains(result.Message, errorMessages);
        //}





        //[Fact]
        //public async Task RemoveCache_ShouldReturnOk_WhenCacheIsEmpty()
        //{
        //    // Arrange
        //    List<EntityMenu>? cachedMenus = null;

        //    SetupCacheService(cachedMenus);

        //    // Act
        //    var objectResult = await _controller.RemoveCache() as ObjectResult;

        //    // Assert
        //    Assert.NotNull(objectResult);
        //    Assert.Equal(200, objectResult.StatusCode);

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

        //    SetupCacheService(cachedMenus);
        //    _cacheServiceMock.Setup(c => c.RemoveCacheAsync(Cache.CacheKeyMenu)).ReturnsAsync(true);

        //    // Act
        //    var objectResult = await _controller.RemoveCache() as ObjectResult;

        //    // Assert
        //    Assert.NotNull(objectResult);
        //    Assert.Equal(200, objectResult.StatusCode);

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

        //    SetupCacheService(cachedMenus);
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
        //    Assert.Equal(200, objectResult.StatusCode);

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
        //    Assert.Equal(400, objectResult.StatusCode);

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
