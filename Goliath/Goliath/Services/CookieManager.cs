using Microsoft.AspNetCore.Http;
using System;

namespace Goliath.Services
{
    /// <summary>
    /// Manages custom cookies by Goliath.
    /// </summary>
    public class CookieManager : ICookieManager
    {
        private readonly IHttpContextAccessor _context;

        public CookieManager(IHttpContextAccessor context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a secure cookie to the browser. All cookies should be UTC Expire Time.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        public void AddCookie(string key, string value, DateTime expireTime)
        {
            CookieOptions cookieOptions = new();
            cookieOptions.Expires = expireTime;
            cookieOptions.HttpOnly = true;
            cookieOptions.SameSite = SameSiteMode.Strict;
            cookieOptions.Secure = true;
            _context.HttpContext.Response.Cookies.Append(key, value, cookieOptions);
        }

        public bool HasCookie(string key)
        {
            return _context.HttpContext.Request.Cookies.ContainsKey(key);
        }

        public string CookieValue(string key)
        {
            return _context.HttpContext.Request.Cookies[key];
        }

        public void DeleteCookie(string key)
        {
            if (HasCookie(key))
            {
                _context.HttpContext.Response.Cookies.Delete(key);
            }

        }
    }
}