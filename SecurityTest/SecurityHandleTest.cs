using Business;
using Entities;
using Interfaces;
using Moq;

namespace SecurityTest
{
    public class SecurityHandleTest
    {
        private readonly Mock<ISecurityService> _mockSecurityService;
        private readonly Mock<IPriceService> _mockPriceService;
        private readonly SecurityHandle _securityHandle;
        public SecurityHandleTest()
        {
            _mockSecurityService = new Mock<ISecurityService>();
            _mockPriceService = new Mock<IPriceService>();
            _securityHandle = new SecurityHandle(_mockSecurityService.Object, _mockPriceService.Object);
        }

        private SecurityEntity GetValidSecurityEntity() => new() { Isin = "123456789012", Price = 12m };
        private SecurityEntity GetValidSecurityEntityWithoutPrice() => new() { Isin = "123456789012" };
        private SecurityEntity GetValidSecurityEntityWithReplacedPrice() => new() { Isin = "123456789012", Price = 15m };
        private SecurityEntity GetInvalidSecurityEntity() => new() { Isin = "1234567890", Price = 12m };
        private IList<SecurityEntity> GetValidSecurityEntityList() =>
            [
                GetValidSecurityEntity(),
                GetValidSecurityEntityWithReplacedPrice(),
                GetValidSecurityEntity(),
                GetValidSecurityEntityWithReplacedPrice(),
                GetValidSecurityEntity()
            ];

        private IList<SecurityEntity> GetValidSecurityEntityListMixedWithoutPrice() =>
            [
                GetValidSecurityEntity(),
                GetValidSecurityEntityWithoutPrice(),
                GetValidSecurityEntity(),
                GetValidSecurityEntityWithoutPrice(),
                GetValidSecurityEntity()
            ];

        private IList<SecurityEntity> GetInvalidSecurityEntityList() =>
            [
                GetValidSecurityEntity(),
                GetValidSecurityEntityWithoutPrice(),
                GetInvalidSecurityEntity(),
                GetValidSecurityEntityWithoutPrice(),
                GetValidSecurityEntity()
            ];

        [Fact]
        public async void AddSecurity_with_price_should_add_security()
        {
            //Arrange
            var validSecurity = GetValidSecurityEntity();
            _mockSecurityService.Setup(x => x.AddSecurityAsync(It.IsAny<SecurityEntity>())).ReturnsAsync(validSecurity);

            //Act
            var result = await _securityHandle.AddSecurity(validSecurity);

            //Assert
            _mockSecurityService.Verify(x => x.AddSecurityAsync(It.IsAny<SecurityEntity>()), Times.Once());
            _mockPriceService.Verify(x => x.GetPriceBySecurityIsinAsync(It.IsAny<string>()), Times.Never());
            Assert.Equal(result, validSecurity);
        }

        [Fact]
        public async void AddSecurity_without_price_should_add_security()
        {
            //Arrange
            var validSecurity = GetValidSecurityEntityWithReplacedPrice();
            _mockSecurityService.Setup(x => x.AddSecurityAsync(It.IsAny<SecurityEntity>())).ReturnsAsync(validSecurity);
            _mockPriceService.Setup(x => x.GetPriceBySecurityIsinAsync(It.IsAny<string>())).ReturnsAsync(15m);

            //Act
            var result = await _securityHandle.AddSecurity(GetValidSecurityEntityWithoutPrice());

            //Assert
            _mockSecurityService.Verify(x => x.AddSecurityAsync(It.IsAny<SecurityEntity>()), Times.Once());
            _mockPriceService.Verify(x => x.GetPriceBySecurityIsinAsync(It.IsAny<string>()), Times.Once());
            Assert.Equal(result, validSecurity);
        }

        [Fact]
        public async void AddSecurity_with_invalid_isin_should_throw_exception()
        {
            //Arrange
            _mockSecurityService.Setup(x => x.AddSecurityAsync(It.IsAny<SecurityEntity>())).ThrowsAsync(new Exception("Invalid ISIN"));

            //Act
            await Assert.ThrowsAsync<Exception>(async () => await _securityHandle.AddSecurity(GetInvalidSecurityEntity()));

            //Assert
            _mockSecurityService.Verify(x => x.AddSecurityAsync(It.IsAny<SecurityEntity>()), Times.Never());
            _mockPriceService.Verify(x => x.GetPriceBySecurityIsinAsync(It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public async void GetSecurities_should_return_5_items()
        {
            //Arrange
            var validSecurities = GetValidSecurityEntityList();
            _mockSecurityService.Setup(x => x.GetSecuritiesAsync()).ReturnsAsync(validSecurities);

            //Act
            var result = await _securityHandle.GetAllSecuritiesAsync();

            //Assert
            _mockSecurityService.Verify(x => x.GetSecuritiesAsync(), Times.Once());
            _mockPriceService.Verify(x => x.GetPriceBySecurityIsinAsync(It.IsAny<string>()), Times.Never());
            Assert.Equal(5, result.Count);
        }

        [Fact]
        public async void GetSecurities_mixed_without_price_should_return_5_items()
        {
            //Arrange
            _mockSecurityService.Setup(x => x.GetSecuritiesAsync()).ReturnsAsync(GetValidSecurityEntityListMixedWithoutPrice());
            _mockPriceService.Setup(x => x.GetPriceBySecurityIsinAsync(It.IsAny<string>())).ReturnsAsync(15m);

            //Act
            var result = await _securityHandle.GetAllSecuritiesAsync();

            //Assert
            _mockSecurityService.Verify(x => x.GetSecuritiesAsync(), Times.Once());
            _mockPriceService.Verify(x => x.GetPriceBySecurityIsinAsync(It.IsAny<string>()), Times.Exactly(2));
            Assert.Equal(5, result.Count);
            Assert.Equal(0, result.Count(w => !w.Price.HasValue));
        }

        [Fact]
        public async void GetSecurities_mixed_invalid_security_should_throw_exception()
        {
            //Arrange
            _mockSecurityService.Setup(x => x.GetSecuritiesAsync()).ReturnsAsync(GetInvalidSecurityEntityList());
            _mockPriceService.Setup(x => x.GetPriceBySecurityIsinAsync(It.IsAny<string>())).ReturnsAsync(15m);

            //Act
            await Assert.ThrowsAsync<Exception>(_securityHandle.GetAllSecuritiesAsync);

            //Assert
            _mockSecurityService.Verify(x => x.GetSecuritiesAsync(), Times.Once());
            _mockPriceService.Verify(x => x.GetPriceBySecurityIsinAsync(It.IsAny<string>()), Times.Once());
        }
    }
}