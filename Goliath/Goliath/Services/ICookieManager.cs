using Microsoft.AspNetCore.Http;
using System;

namespace Goliath.Services
{
    /// <summary>
    /// Manages Cookies in the browser through the <see cref="IHttpContextAccessor" />.
    /// <para> <b> All cookies created by Goliath are required to follow: </b>
    /// <list type="bullet">
    /// <item>
    /// <term> <see cref="CookieOptions.HttpOnly" /> </term>
    /// <description> <c> true </c> </description>
    /// </item>
    /// <item>
    /// <term> <see cref="CookieOptions.SameSite" /> </term>
    /// <description> <see cref="SameSiteMode.Strict" /> </description>
    /// </item>
    /// <item>
    /// <term> <see cref="CookieOptions.Secure" /> </term>
    /// <description> <c> true </c> </description>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// Each cookie's key value should be a valid constant in <see cref="Enums.CookieKeys" />
    /// </para>
    /// </summary>
    public interface ICookieManager
    {
        /// <summary>
        /// Adds a cookie to the browser with the specified <paramref name="key" />, <paramref
        /// name="value" />, and <paramref name="expireTime" />. <br /> Expire time should be
        /// written like:
        /// <para> <c> _cookieManager.AddCookie([...], DateTime.UtcNow.AddMinutes(5)); </c> </para>
        /// </summary>
        /// <param name="key"> Key for the cookie. </param>
        /// <param name="value"> Cookie's value. </param>
        /// <param name="expireTime"> The time <b> UTC </b> from now when it expires. </param>
        void AddCookie(string key, string value, DateTime expireTime);

        /// <summary>
        /// The value of a cookie with the <paramref name="key" />.
        /// </summary>
        /// <param name="key"> Key string. </param>
        /// <returns> String representation of value. </returns>
        string CookieValue(string key);

        /// <summary>
        /// Returns whether or not the browser has the specified cookie with the <paramref
        /// name="key" />.
        /// </summary>
        /// <param name="key"> Key string. </param>
        /// <returns> If the cookie exists. </returns>
        bool HasCookie(string key);

        /// <summary>
        /// Finds the specified cookie in the browser and removes it.
        /// </summary>
        /// <param name="key"> Key string. </param>
        void DeleteCookie(string key);
    }
}