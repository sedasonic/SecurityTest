namespace Interfaces
{
    public interface IPriceService
    {
        public Task<decimal?> GetPriceBySecurityIsinAsync(string securityIsin);
    }
}
