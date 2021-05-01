using Microsoft.AspNetCore.Http;
using System;

namespace Goliath.Services
{
    /// <inheritdoc cref="ICookieManager" />
    public class CookieManager : ICookieManager
    {
        private readonly IHttpContextAccessor _context;

        public CookieManager(IHttpContextAccessor context)
        {
            _context = context;
        }

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