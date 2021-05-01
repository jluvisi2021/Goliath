using System.Threading.Tasks;

namespace Goliath.Repository
{
    /// <summary>
    /// A repository which manages the user's current status with resending SMS verification codes.
    /// </summary>
    public interface ISmsVerifyTokensRepository
    {
        /// <summary>
        /// Adds a entry in the database for the specified <paramref name="uid" /> and the current
        /// <c> DateTime.UtcNow </c>.
        /// </summary>
        /// <param name="uid"> UserId </param>
        Task AddRequestAsync(string uid);

        /// <param name="uid"> UserId </param>
        /// <returns> If the user is allowed to request another resend. </returns>
        Task<bool> IsUserResendValidAsync(string uid);

        /// <summary>
        /// Remove a user entry from the database.
        /// </summary>
        /// <param name="uid"> UserId </param>
        Task RemoveRequestAsync(string uid);
    }
}