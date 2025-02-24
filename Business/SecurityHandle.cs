using Business.Extensions;
using Business.Interface;
using Entities;
using Interfaces;

namespace Business
{
    public class SecurityHandle : ISecurityHandle
    {
        private readonly ISecurityService _securityService;
        private readonly IPriceService _priceService;

        public SecurityHandle(ISecurityService securityService, IPriceService priceService)
        {
            _securityService = securityService;
            _priceService = priceService;
        }

        public async Task<SecurityEntity> AddSecurity(SecurityEntity request)
        {
            if (request == null) throw new Exception("Invalid request");
            if (!request.Isin.IsIsinValid()) throw new Exception("Invalid ISIN");

            request.Price ??= await _priceService.GetPriceBySecurityIsinAsync(request.Isin);
            return await _securityService.AddSecurityAsync(request);
        }

        public async Task<IList<SecurityEntity>> GetAllSecuritiesAsync()
        {
            var securities = await _securityService.GetSecuritiesAsync();

            foreach (var securityEntity in securities) {
                if (!securityEntity!.Isin.IsIsinValid()) throw new Exception("Invalid ISIN");
                securityEntity.Price ??= await _priceService.GetPriceBySecurityIsinAsync(securityEntity.Isin);
            }

            return securities;
        }
    }
}
