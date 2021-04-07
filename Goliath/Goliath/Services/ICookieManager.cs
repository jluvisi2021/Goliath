using System;

namespace Goliath.Services
{
    public interface ICookieManager
    {
        void AddCookie(string key, string value, DateTime expireTime);

        string CookieValue(string key);

        bool HasCookie(string key);

        void DeleteCookie(string key);
    }
}