using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SnackExchange.Web.Data;
using SnackExchange.Web.Models;
using SnackExchange.Web.Models.Auth;
using SnackExchange.Web.Repository;

namespace SnackExchange.Web.Controllers
{
    public class ExchangesController : Controller
    {
        private readonly IRepository<Exchange> _exchangeRepository;
        private readonly IRepository<ExchangeUserModel> _exchangeUserModelRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepository<Product> _productRepository;

        public ExchangesController(
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

        // GET: Exchanges
        [Authorize]
        public IActionResult Index()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (user.UserStatus != UserStatus.Banned || user.UserStatus != UserStatus.Inactive)
            {
                return View(_exchangeRepository.GetAll());
            }
            else
            {
                var myExchanges = _exchangeRepository.FindBy(e => e.Sender.Id == user.Id);
                return View(myExchanges);
            }

        }

        // GET: Exchanges/Details/5
        [Authorize]
        public IActionResult Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            IQueryable<Exchange> exchangeQuery = _exchangeRepository.FindBy(x => x.Id == id);

            var exchange = exchangeQuery.FirstOrDefault();
            if (exchange == null)
            {
                return NotFound();
            }

            return View(exchange);
        }

        // GET: Exchanges/Create
        [Authorize]
        public IActionResult Create()
        {
            var model = new Exchange();
            model.Products.Add(new Product());
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create([Bind("ExchangeNotes,PhotoUrl,Products")] Exchange exchange)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
                exchange.Sender = _userManager.FindByIdAsync(currentUserId).Result; // current user
                exchange.SenderId = currentUserId;
                exchange.Status = ExchangeStatus.Created;

                exchange.Products = exchange.Products.Where(p => !string.IsNullOrEmpty(p.Name) && !string.IsNullOrEmpty(p.Price)).ToList();

                foreach (Product p in exchange.Products)
                {
                    p.OriginCountry = exchange.Sender.Country;
                }

                _exchangeRepository.Insert(exchange);

                //foreach()

                var user = exchange.Sender;
                var exchangeUserModel = new ExchangeUserModel
                {
                    UserExchangeRole = UserExchangeRole.Sender,
                    Exchange = exchange,
                    UpdatedAt = DateTime.Now
                };

                _exchangeUserModelRepository.Insert(exchangeUserModel);
                user.Exchanges.Add(exchangeUserModel);
                _userManager.UpdateAsync(user);

                return RedirectToAction(nameof(Index));
            }
            return View(exchange);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProduct([Bind("Products")] Exchange exchange)
        {
            if (ModelState.IsValid)
            {
                //_productRepository.Insert(exchange.Products.Last());
                exchange.Products.Add(new Product());
            }
            return PartialView("Products", exchange);
        }


        // GET: Exchanges/Edit/5
        public IActionResult Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var exchange = _exchangeRepository.GetById(id);
            if (exchange == null)
            {
                return NotFound();
            }
            return View(exchange);
        }

        // POST: Exchanges/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,ExchangeNotes,PhotoUrl,TrackingNumber")] Exchange exchange)
        {
            if (id != exchange.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
                    exchange.Sender = user;
                    exchange.SenderId = user.Id;
                    exchange.UpdatedAt = DateTime.Now;
                    _exchangeRepository.Update(exchange);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExchangeExists(exchange.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(exchange);
        }


        // GET: Exchanges/Delete/5
        [Authorize]
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            var exchange = _exchangeRepository.GetById(id);
            if (exchange == null || _userManager.GetUserId(_httpContextAccessor.HttpContext.User) != exchange.Sender.Id)
            {
                return NotFound();
            }

            return View(exchange);
        }

        // POST: Exchanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _exchangeRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private bool ExchangeExists(Guid id)
        {
            var exchange = _exchangeRepository.GetById(id);
            return exchange != null;
        }

        public IActionResult AddSender(Exchange exchange, AppUser user)
        {
            exchange.Sender = user;
            exchange.SenderId = user.Id;
            exchange.Status = ExchangeStatus.Waiting;
            return View(exchange);
        }
        public IActionResult AddModerator(Exchange exchange, AppUser user)
        {
            exchange.Moderator = user;
            exchange.ModeratorId = user.Id;
            return View(exchange);
        }
        public IActionResult AddModeratorNotes(Exchange exchange, String notes)
        {
            exchange.ModeratorNotes = notes;
            return View(exchange);
        }
        public IActionResult AddTrackingNumber(Exchange exchange, String trackingNumber)
        {
            exchange.TrackingNumber = trackingNumber;
            exchange.Status = ExchangeStatus.OnAir;
            return View(exchange);
        }


    }
}
