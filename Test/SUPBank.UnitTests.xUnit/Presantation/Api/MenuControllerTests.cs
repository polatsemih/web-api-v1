using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SUPBank.Api.Controllers;
using SUPBank.Application.Features.Menu.Queries;
using SUPBank.Application.Interfaces.Services;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Results.Data;
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
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenCacheIsNotEmpty()
        {
            // Arrange
            var cachedMenus = new List<EntityMenu>
            {
                new EntityMenu
                {
                    Id = 1,
                    ParentId = 0,
                    Name_TR = "Profilim",
                    Name_EN = "My Profile",
                    ScreenCode = 10000,
                    Type = 10,
                    Priority = 0,
                    Keyword = "profilim, profile",
                    Icon = "asdsadasd",
                    IsGroup = false,
                    IsNew = false,
                    NewStartDate = null,
                    NewEndDate = null,
                    SubMenus = new List<EntityMenu>
                    {
                        new EntityMenu
                        {
                            Id = 4,
                            ParentId = 1,
                            Name_TR = "Gelir ve Çalışma Bilgileri",
                            Name_EN = "Income and Work Information",
                            ScreenCode = 10003,
                            Type = 10,
                            Priority = 100,
                            Keyword = "gelir, çalışma bilgileri, income, work information",
                            Icon = "asdsadasd",
                            IsGroup = false,
                            IsNew = false,
                            NewStartDate = null,
                            NewEndDate = null,
                            SubMenus = new List<EntityMenu>(),
                            IsActive = true,
                            CreatedDate = DateTime.Parse("2024-05-17T14:47:39.04"),
                            LastModifiedDate = DateTime.MinValue
                        },
                        new EntityMenu
                        {
                            Id = 10,
                            ParentId = 1,
                            Name_TR = "Contract/Documents",
                            Name_EN = "Contract/Documents",
                            ScreenCode = 10009,
                            Type = 10,
                            Priority = 400,
                            Keyword = "sözleşme, belgeler, contract, documents",
                            Icon = "asdsadasd",
                            IsGroup = false,
                            IsNew = false,
                            NewStartDate = null,
                            NewEndDate = null,
                            SubMenus = new List<EntityMenu>
                            {
                                new EntityMenu
                                {
                                    Id = 16,
                                    ParentId = 10,
                                    Name_TR = "Belgelerim",
                                    Name_EN = "My Documents",
                                    ScreenCode = 10015,
                                    Type = 10,
                                    Priority = 100,
                                    Keyword = "belgelerim, documents",
                                    Icon = "asdsadasd",
                                    IsGroup = false,
                                    IsNew = false,
                                    NewStartDate = null,
                                    NewEndDate = null,
                                    SubMenus = new List<EntityMenu>(),
                                    IsActive = true,
                                    CreatedDate = DateTime.Parse("2024-05-17T14:47:39.043"),
                                    LastModifiedDate = DateTime.MinValue
                                }
                            },
                            IsActive = true,
                            CreatedDate = DateTime.Parse("2024-05-17T14:47:39.04"),
                            LastModifiedDate = DateTime.MinValue
                        }
                    },
                    IsActive = true,
                    CreatedDate = DateTime.Parse("2024-05-17T14:47:39.04"),
                    LastModifiedDate = DateTime.MinValue
                }
            };

            _cacheServiceMock.Setup(c => c.GetCache<List<EntityMenu>>(CacheKeys.CacheKeyMenu)).Returns(cachedMenus);

            var request = new GetAllMenuQueryRequest();

            // Act
            var result = await _controller.GetAll(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var successDataResult = result.Value as SuccessDataResult<List<EntityMenu>>;
            Assert.NotNull(successDataResult);
            Assert.True(successDataResult.IsSuccess);
            Assert.Equal(cachedMenus, successDataResult.Data);
        }



















        //[Fact]
        //public async Task GetAll_ShouldReturnOk_WhenCacheIsEmpty()
        //{
        //    // Arrange
        //    _cacheServiceMock.Setup(x => x.GetCache<List<EntityMenu>>(CacheKeys.CacheKeyMenu)).Returns((List<EntityMenu>)null);
        //    var menus = new List<EntityMenu> { new EntityMenu { Id = 1, Name_EN = "Menu1" } };
        //    var mediatorResult = new SuccessDataResult<List<EntityMenu>>(menus);
        //    _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllMenuQueryRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(mediatorResult);

        //    // Act
        //    var result = await _controller.GetAll(new GetAllMenuQueryRequest()) as OkObjectResult;

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(200, result.StatusCode);
        //    var successDataResult = result.Value as SuccessDataResult<List<EntityMenu>>;
        //    Assert.NotNull(successDataResult);
        //    Assert.True(successDataResult.IsSuccess);
        //    Assert.Equal(menus, successDataResult.Data);
        //}
    }
}
