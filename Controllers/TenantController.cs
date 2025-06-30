using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using test.Interfaces;

namespace test.Controllers
{
    [Authorize]

    public class TenantController : Controller
    {
        private readonly IApplicationUnitOfWork _applicationUnitOfWork;

        public TenantController(IApplicationUnitOfWork applicationUnitOfWork)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
        }

        public async Task<IActionResult> Index() => View(await _applicationUnitOfWork.TenantManagementRepository.GetAllAsync());        
    }
}