using System.Threading.Tasks;

namespace Goliath.Repository
{
    public interface IValidHumanVerifyTokensRepository
    {
        Task AddTokenAsync(string key);

        Task ClearAllTokensAsync();

        Task<bool> DoesTokenExistAsync(string hashCode);

        Task CleanUpUnusedTokensAsync();
    }
}