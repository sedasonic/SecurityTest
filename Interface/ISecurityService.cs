using Entities;

namespace Interfaces
{
    public interface ISecurityService
    {
        public Task<IList<SecurityEntity>> GetSecuritiesAsync();
        public Task<SecurityEntity> AddSecurityAsync(SecurityEntity security);
    }
}
