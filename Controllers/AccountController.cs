using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Data;
using test.Interfaces;
using test.Models;
using test.Models.ViewModels;

namespace test.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;
        private readonly ITenantService _tenantService;

        public AccountController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        ApplicationDbContext applicationDbContext,
        ITenantService tenantService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
            _tenantService = tenantService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    try
                    {
                        await CreateTenantDatabaseAsync(user);

                        return RedirectToAction("Login", "Account");
                    }
                    catch (Exception ex)
                    {
                        // User'ı sil çünkü tenant oluşturulamadı
                        await _userManager.DeleteAsync(user);
                        ModelState.AddModelError("", "Hesap oluşturulurken bir hata oluştu. Lütfen tekrar deneyin.");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı.");
                }
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private async Task CreateTenantDatabaseAsync(ApplicationUser user)
        {
            var databaseName = $"TenantDb_{user.Id}_{Guid.NewGuid():N}";
            var masterConnectionString = _configuration.GetConnectionString("DefaultConnection");
            var tenantConnectionString = masterConnectionString.Replace("test", databaseName);

            // Veritabanı oluştur
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseSqlServer(tenantConnectionString);

            using var tenantDbContext = new TenantDbContext(optionsBuilder.Options);
            await tenantDbContext.Database.MigrateAsync();

            // Sample data ekle
            // await SeedTenantDataAsync(tenantDbContext);

            // Tenant kaydını ana veritabanına ekle
            var tenant = new Tenant
            {
                Name = $"Tenant_{user.UserName}",
                DatabaseConnectionString = tenantConnectionString,
                UserId = user.Id,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            _applicationDbContext.Tenants.Add(tenant);
            await _applicationDbContext.SaveChangesAsync();

            // User'a tenant'ı ata
            user.TenantId = tenant.Id;
            await _userManager.UpdateAsync(user);
        }
    }
}