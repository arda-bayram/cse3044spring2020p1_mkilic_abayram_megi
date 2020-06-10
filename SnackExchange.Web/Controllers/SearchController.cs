using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SnackExchange.Web.Models;
using SnackExchange.Web.Models.Auth;
using SnackExchange.Web.Repository;

namespace SnackExchange.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly IRepository<Exchange> _exchangeRepository;
        private readonly IRepository<ExchangeUserModel> _exchangeUserModelRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepository<Product> _productRepository;

        public SearchController(
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
        }

        public IActionResult Index(string q)
        {
            IQueryable<Product> products = _productRepository.FindBy(p => p.Name.Contains(q) || p.Description.Contains(q));
            return View(products);
        }
    }
}