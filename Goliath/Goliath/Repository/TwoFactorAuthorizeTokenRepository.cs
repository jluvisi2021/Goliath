using Goliath.Data;
using Goliath.Enums;
using Goliath.Helper;
using Goliath.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Goliath.Repository
{
    public class TwoFactorAuthorizeTokenRepository : ITwoFactorAuthorizeTokenRepository
    {
        private readonly GoliathContext _context;
        private readonly IAccountRepository _repository;
        private readonly ICookieManager _cookies;

        public TwoFactorAuthorizeTokenRepository(GoliathContext context, IAccountRepository repository, ICookieManager cookies)
        {
            _context = context;
            _repository = repository;
            _cookies = cookies;
        }

        public async Task CreateTokenAsync(string userName, string token)
        {
            string userId = (await _repository.GetUserByNameAsync(userName)).Id;
            if (await _context.TwoFactorTokens.FirstOrDefaultAsync(u => u.UserId.Equals(userId)) != null)
            {
                await DisposeTokenAsync(userId);
            }
            await _context.TwoFactorTokens.AddAsync(new TwoFactorAuthorizeToken()
            {
                UserId = userId,
                AuthorizeToken = token
            });
            _cookies.AddCookie(CookieKeys.TwoFactorAuthorizeCookie, GoliathHash.HashStringSHA256(token), DateTime.UtcNow.AddMinutes(10));
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TokenValidAsync(string userId)
        {
            if (await _context.TwoFactorTokens.FirstOrDefaultAsync(u => u.UserId.Equals(userId)) == null || !_cookies.HasCookie(CookieKeys.TwoFactorAuthorizeCookie))
            {
                return false;
            }
            if (GoliathHash.ValidateStringSHA256(_cookies.CookieValue(CookieKeys.TwoFactorAuthorizeCookie)))
            {
                if (GoliathHash.HashStringSHA256((await _context.TwoFactorTokens.FirstOrDefaultAsync(u => u.UserId.Equals(userId))).AuthorizeToken).Equals(_cookies.CookieValue(CookieKeys.TwoFactorAuthorizeCookie)))
                {
                    return true;
                }
            }
            return false;
        }

        private async Task DisposeTokenAsync(string userId)
        {
            _context.TwoFactorTokens.Remove(await _context.TwoFactorTokens.FirstOrDefaultAsync(u => u.UserId.Equals(userId)));
            if (_cookies.HasCookie(CookieKeys.TwoFactorAuthorizeCookie))
            {
                _cookies.DeleteCookie(CookieKeys.TwoFactorAuthorizeCookie);
            }
            await _context.SaveChangesAsync();
        }
    }
}