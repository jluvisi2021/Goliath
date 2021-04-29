using Goliath.Enums;
using System.Threading.Tasks;

namespace Goliath.Repository
{
    public interface IUnauthorizedTimeoutsRepository
    {
        Task UpdateRequestAsync(string userId, UnauthorizedRequest requestType);
        Task<bool> CanRequestTwoFactorSmsAsync(string userId);
        Task<bool> CanRequestResendTwoFactorSmsAsync(string userId);
    }
}