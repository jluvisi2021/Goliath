using System.Threading.Tasks;

namespace Goliath.Repository
{
    /// <summary>
    /// <para> Manages the tokens which are granted to user's which have two factor authentication. </para>
    /// <para>
    /// The purpose of this repository is to check if the person requesting a two-factor token for
    /// the specific User has in fact already input their Username and Password.
    /// </para>
    /// <para>
    /// Without these checks then a user could simply enter the URL for the two-factor request:
    /// </para>
    /// <para> <em> ex: </em><c> https://(...)/account/validate/?userName=GoliathAdmin </c> </para>
    /// <para>
    /// They would be able to request a two-factor code and even request a resend despite never
    /// inputting their username and password.
    /// </para>
    /// <para>
    /// The <see cref="ITwoFactorAuthorizeTokenRepository" /> exists to add a temporary hash of a
    /// token to a user in a stored cookie ( <see cref="Enums.CookieKeys.TwoFactorAuthorizeCookie"
    /// />) after the user logs in with their username and password but has not yet put in their
    /// two-factor token.
    /// <para>
    /// Finally, the hashed cookie value and the value in the database can be compared whenever the
    /// URL to open the two-factor screen is requested.
    /// </para>
    /// </para>
    /// </summary>
    public interface ITwoFactorAuthorizeTokenRepository
    {
        /// <summary>
        /// Adds an entry into the database using a <paramref name="userName" /> and then adds a
        /// <paramref name="token" /> to it. <br /> This method will also automatically add a cookie
        /// to the user's session using <see cref="Services.ICookieManager" />.
        /// </summary>
        /// <param name="userName"> Username to search. </param>
        /// <param name="token"> Token value. Should be a secured random number. </param>
        /// <returns> </returns>
        Task CreateTokenAsync(string userName, string token);

        /// <summary>
        /// Gets the <see cref="Enums.CookieKeys.TwoFactorAuthorizeCookie" /> value and compares it
        /// to the relative value in the database. <br /><b> If they match </b> then the database
        /// entry is removed and the user is validated for that Url.
        /// </summary>
        /// <param name="userId"> UserId to search. </param>
        /// <returns> If the user is authorized to request a two-factor action. </returns>
        Task<bool> TokenValidAsync(string userId);
    }
}