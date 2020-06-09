using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SnackExchange.Web.Models;
using SnackExchange.Web.Models.Auth;
using SnackExchange.Web.Repository;
using Microsoft.AspNetCore.Authorization;

namespace SnackExchange.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepository<Exchange> _exchangeRepository;
        private readonly IRepository<ExchangeUserModel> _exchangeUserModelRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepository<Product> _productRepository;

        public HomeController(ILogger<HomeController> logger,
            IRepository<Exchange> exchangeRepository,
            IRepository<ExchangeUserModel> exchangeUserModelRepository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager,
            IRepository<Product> productRepository)
        {
            _exchangeRepository = exchangeRepository;
            _exchangeUserModelRepository = exchangeUserModelRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _productRepository = productRepository;
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (User.Identity.Name != null)
            {
                var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
                var exchanges = _exchangeRepository.FindBy(e => e.Status != ExchangeStatus.Completed);
                return View(exchanges);
            }
            else
            {
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
