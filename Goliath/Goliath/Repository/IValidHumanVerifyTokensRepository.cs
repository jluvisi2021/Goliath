using System.Threading.Tasks;

namespace Goliath.Repository
{
    /// <summary>
    /// A repository which manages the cookie tokens for the user's captcha as well as validates if
    /// the captcha is correct.
    /// </summary>
    public interface IValidHumanVerifyTokensRepository
    {
        /// <summary>
        /// Add a new captcha token to the user.
        /// </summary>
        /// <param name="key"> The token to add. </param>
        /// <returns> </returns>
        Task AddTokenAsync(string key);

        /// <summary>
        /// Removes all tokens in the database.
        /// </summary>
        /// <returns> </returns>
        Task ClearAllTokensAsync();

        /// <param name="hashCode"> The hash of the number. </param>
        /// <returns>
        /// Whether or not a token exists in the database. This method automatically invokes <see
        /// cref="CleanUpUnusedTokensAsync" /> before searching the database.
        /// </returns>
        Task<bool> DoesTokenExistAsync(string hashCode);

        /// <summary>
        /// Removes all tokens which are past their expire time.
        /// </summary>
        /// <returns> </returns>
        Task CleanUpUnusedTokensAsync();
    }
}