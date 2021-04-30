using System.Threading.Tasks;

namespace Goliath.Repository
{
    public interface ITwoFactorAuthorizeTokenRepository
    {
        Task CreateTokenAsync(string userName, string token);

        Task<bool> TokenValidAsync(string userId);
    }
}