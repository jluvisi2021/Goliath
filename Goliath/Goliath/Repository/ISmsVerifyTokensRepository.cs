using System.Threading.Tasks;

namespace Goliath.Repository
{
    public interface ISmsVerifyTokensRepository
    {
        Task AddRequestAsync(string uid);

        Task<bool> IsUserResendValidAsync(string uid);

        Task RemoveRequestAsync(string uid);
    }
}