using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using test.Data;
using test.Interfaces;
using test.Models;

namespace test.Services
{
    public class TenantService : ITenantService
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TenantService(IMemoryCache cache, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _context = context;
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);
            return user?.Id;
        }

        public async Task<Tenant> GetCurrentTenantAsync()
        {
            var userId = await GetCurrentUserIdAsync();
            if (string.IsNullOrEmpty(userId)) return null;

            var cacheKey = $"tenant_{userId}";

            if (!_cache.TryGetValue(cacheKey, out Tenant tenant))
            {
                tenant = await _context.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.UserId == userId && t.IsActive);

                if (tenant != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                        SlidingExpiration = TimeSpan.FromMinutes(10),
                        Priority = CacheItemPriority.High
                    };

                    _cache.Set(cacheKey, tenant, cacheEntryOptions);
                }
            }

            return tenant;
        }

        public async Task<string> GetCurrentTenantConnectionStringAsync()
        {
            var tenant = await GetCurrentTenantAsync();
            return tenant?.DatabaseConnectionString;
        }

        public async Task<bool> HasValidTenantAsync()
        {
            var tenant = await GetCurrentTenantAsync();
            return tenant != null && !string.IsNullOrEmpty(tenant.DatabaseConnectionString);
        }

        public void ClearCache(string userId)
        {
            var cacheKey = $"tenant_{userId}";
            _cache.Remove(cacheKey);
        }
    }
}