using Microsoft.Extensions.Logging;
using Moq;
using SteamGameTracker.DataTransferObjects;
using SteamGameTracker.DataTransferObjects.SteamApi.Models;
using SteamGameTracker.Models;
using SteamGameTracker.Services.API;
using SteamGameTracker.Services.API.URLs;
using SteamGameTracker.Services;
using SteamGameTracker.Components;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SteamGameTracker.Tests
{
    [TestClass]
    public class AppDetailsServiceTests
    {
        private Mock<IUrlFormatter> _urlFormatterMock = null!;
        private Mock<ICacheService> _cacheServiceMock = null!;
        private Mock<ILogger<AppDetailsService>> _loggerMock = null!;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock = null!;
        private HttpClient _httpClient = null!;
        private AppDetailsDTO _fakeDto = null!;

        [TestInitialize]
        public void Initialize()
        {
            _urlFormatterMock = new Mock<IUrlFormatter>();
            _cacheServiceMock = new Mock<ICacheService>();
            _loggerMock = new Mock<ILogger<AppDetailsService>>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://test.com/")
            };
            _fakeDto = CreateFakeAppDetailsDto((123, "Test Game"), (456, "Game 456"));
        }

        [TestMethod]
        public async Task GetAppDetailsAsync_WithCachedValue_ReturnsModel()
        {
            var cacheKey = "AppDetails_123";
            _cacheServiceMock
                .Setup(x => x.GetDtoAsync<SuccessDTO>(cacheKey, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_fakeDto[456]);

            var service = CreateService();

            var result = await service.GetAppDetailsAsync(123);

            Assert.IsNotNull(result);
            Assert.AreEqual("Game 456", result.Name);
        }

        [TestMethod]
        public async Task GetAppDetailsAsync_WithoutCache_FetchesFromApiAndCaches()
        {
            var cacheKey = "AppDetails_456";

            _cacheServiceMock
                .Setup(x => x.GetDtoAsync<SuccessDTO>(cacheKey, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SuccessDTO?)null);

            _cacheServiceMock
                .Setup(x => x.SetDtoAsync<SuccessDTO>(cacheKey, _fakeDto[456], It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _urlFormatterMock
                .Setup(x => x.GetFormattedUrl(It.IsAny<GetAppDetailsUrl>()))
                .Returns("https://test");

            var service = CreateService(_fakeDto);

            var result = await service.GetAppDetailsAsync(456);

            Assert.IsNotNull(result);
            Assert.AreEqual("Game 456", result.Name);
        }

        [TestMethod]
        public async Task GetAppDetailsAsync_Multiple_ReturnsAllModels()
        {
            int[] appIds = [123, 456];

            _cacheServiceMock
                .Setup(x => x.GetDtoAsync<SuccessDTO>("AppDetails_123", It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_fakeDto[123]);

            _cacheServiceMock
                .Setup(x => x.GetDtoAsync<SuccessDTO>("AppDetails_456", It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_fakeDto[456]);

            _urlFormatterMock
                .Setup(x => x.GetFormattedUrl(It.IsAny<GetAppDetailsUrl>()))
                .Returns("https://test");

            var service = CreateService(_fakeDto);

            var result = await service.GetAppDetailsAsync(appIds);

            Assert.IsNotNull(result);
            var list = new List<AppDetailsModel>(result!);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Test Game", list[0].Name);
            Assert.AreEqual("Game 456", list[1].Name);
        }

        [TestMethod]
        public async Task GetAppDetailsAsync_CacheReturnsInvalidDTO_RemovesCacheAndContinues()
        {
            int appId = 123;
            string cacheKey = $"AppDetails_{appId}";

            // Setup cache to return corrupted DTO (e.g. missing required fields)
            var corruptedDto = new SuccessDTO
            {
                Data = null // or partial invalid data
            };

            _cacheServiceMock
                .Setup(x => x.GetDtoAsync<SuccessDTO>(cacheKey, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(corruptedDto);

            _cacheServiceMock
                .Setup(x => x.RemoveAsync(cacheKey, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Setup API fetch to return valid DTO
            var validDto = new SuccessDTO
            {
                Data = new GameDataDTO { SteamAppId = appId, Name = "Recovered Game", Type = "game" }
            };
            var apiDto = new AppDetailsDTO { [appId] = validDto };

            _urlFormatterMock
                .Setup(x => x.GetFormattedUrl(It.IsAny<GetAppDetailsUrl>()))
                .Returns("https://test");

            var service = CreateService(apiDto);

            var result = await service.GetAppDetailsAsync(appId);

            Assert.IsNotNull(result);
            Assert.AreEqual("Recovered Game", result.Name);

            _cacheServiceMock.Verify(x => x.RemoveAsync(cacheKey, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task GetAppDetailsAsync_ApiReturnsNull_ReturnsNull()
        {
            int appId = 789;
            string cacheKey = $"AppDetails_{appId}";

            // Cache miss
            _cacheServiceMock
                .Setup(x => x.GetDtoAsync<SuccessDTO>(cacheKey, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SuccessDTO?)null);

            // API returns null
            var service = CreateService(dto: null);

            var result = await service.GetAppDetailsAsync(appId);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetAppDetailsAsync_EmptyAppIds_ReturnsNull()
        {
            var service = CreateService();

            var result = await service.GetAppDetailsAsync(Array.Empty<int>());

            Assert.IsTrue(result.Count() == 0);
        }

        [TestMethod]
        public async Task GetAppDetailsAsync_InvalidDtoData_ReturnsNull()
        {
            var invalidSuccessDto = new SuccessDTO
            {
                Data = new GameDataDTO()
            };
            var invalidAppDetailsDto = new AppDetailsDTO
            {
                { 0, invalidSuccessDto }
            };
            var service = CreateService(invalidAppDetailsDto);

            var model = await service.GetAppDetailsAsync(0);
            Assert.IsNull(model);
        }


        private AppDetailsDTO CreateFakeAppDetailsDto(params (int appId, string name)[] entries)
        {
            var dto = new AppDetailsDTO();
            foreach (var (appId, name) in entries)
            {
                dto[appId] = new SuccessDTO
                {
                    Data = new GameDataDTO { SteamAppId= appId, Name = name, Type = "game" }
                };
            }
            return dto;
        }


        private TestableAppDetailsService CreateService(AppDetailsDTO? dto = null)
        {
            return new TestableAppDetailsService(
                _urlFormatterMock.Object,
                _httpClient,
                _loggerMock.Object,
                _cacheServiceMock.Object,
                dto ?? _fakeDto);
        }

        private class TestableAppDetailsService : AppDetailsService
        {
            private readonly AppDetailsDTO? _dto;

            public TestableAppDetailsService(
                IUrlFormatter urlFormatter,
                HttpClient httpClient,
                ILogger<AppDetailsService> logger,
                ICacheService cacheService,
                AppDetailsDTO? dto)
                : base(urlFormatter, httpClient, logger, cacheService)
            {
                _dto = dto;
            }

            protected override Task<TDto?> GetDtoAsync<TDto>(string url, 
                CancellationToken cancellationToken = default) where TDto : class
            {
                if (typeof(TDto) == typeof(AppDetailsDTO))
                {
                    return Task.FromResult(_dto as TDto);
                }

                return Task.FromResult<TDto?>(default);
            }
        }
    }
}
