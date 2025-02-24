using Entities;

namespace Business.Interface
{
    public interface ISecurityHandle
    {
        Task<SecurityEntity> AddSecurity(SecurityEntity request);
        Task<IList<SecurityEntity>> GetAllSecuritiesAsync();
    }
}
