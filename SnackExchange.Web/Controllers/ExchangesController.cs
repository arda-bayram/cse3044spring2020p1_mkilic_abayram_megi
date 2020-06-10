using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
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
            if (user != null)
            {
                if (user.UserStatus != UserStatus.Banned || user.UserStatus != UserStatus.Inactive)
                {
                    return View(_exchangeRepository.FindBy(e => e.Status != ExchangeStatus.Completed));
                }
                else
                {
                    var myExchanges = _exchangeRepository.FindBy(e => e.Sender.Id == user.Id);
                    return View(myExchanges);
                }
            }
            else
            {
                return RedirectToAction("Privacy", "Home");
            }
        }

        [Authorize]
        public IActionResult MyExchanges()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var myExchanges = _exchangeRepository.FindBy(e => e.Sender.Id == user.Id);
            return View("Index", myExchanges);
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
                var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
                exchange.Sender = user; // current user
                exchange.SenderId = user.Id;
                exchange.Status = ExchangeStatus.Created;

                exchange.Products = exchange.Products.Where(p => !string.IsNullOrEmpty(p.Name) && !string.IsNullOrEmpty(p.Price)).ToList();

                foreach (Product p in exchange.Products)
                {
                    p.OriginCountry = exchange.Sender.Country;
                }

                _exchangeRepository.Insert(exchange);

                //foreach()

                ExchangeUserModel exchangeUserModel = new ExchangeUserModel
                {
                    UserExchangeRole = UserExchangeRole.Sender,
                    Exchange = exchange,
                    AppUser = user,
                    UpdatedAt = DateTime.Now
                };

                user.Exchanges.Add(exchangeUserModel);

                _exchangeUserModelRepository.Insert(exchangeUserModel);
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
        public IActionResult Edit(Guid id, [Bind("Id,ModeratorNotes,ExchangeNotes,PhotoUrl,TrackingNumber")] Exchange exchange)
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
                    var exchangeDb = _exchangeRepository.GetById(exchange.Id);
                    if (user == exchangeDb.Sender || user.IsModerator)
                    {
                        exchangeDb.ExchangeNotes = exchange.ExchangeNotes;
                        exchangeDb.PhotoUrl = exchange.PhotoUrl;
                        exchangeDb.TrackingNumber = exchange.TrackingNumber;
                    }
                    exchangeDb.ModeratorNotes = exchange.ModeratorNotes;
                    if (exchangeDb.Moderator == null && user.IsModerator)
                    {
                        exchangeDb.Moderator = user;
                    }
                    exchangeDb.UpdatedAt = DateTime.Now;
                    _exchangeRepository.Update(exchangeDb);
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
            AppUser user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (user == null || user.Exchanges.Count <= 0)
            {
                return RedirectToAction(nameof(Index));
            }
            //            ExchangeUserModel userExchange = user.Exchanges.Where(e => e.Id == id).FirstOrDefault();

            Exchange exchange = _exchangeRepository.FindBy(e => e.Id == id).FirstOrDefault();
            if (exchange == null || exchange.Sender.Id != user.Id)
            {
                return RedirectToAction(nameof(Index));
            }

            ExchangeUserModel userExchangeModel = _exchangeUserModelRepository.FindBy(eu => eu.Exchange.Id == exchange.Id && eu.AppUser.Id == user.Id).FirstOrDefault();
            if (userExchangeModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (exchange.Sender.Id == user.Id || user.IsModerator)
            {

                for (int i = 0; i < exchange.Products.Count; i++)
                {
                    if (exchange.Products.Count > 0)
                    {
                        exchange.Products[i].Exchange = null;
                        _productRepository.Update(exchange.Products[i]);
                    }
                }

                user.Exchanges.Remove(userExchangeModel);

                _exchangeRepository.Delete(exchange.Id);
                _userManager.UpdateAsync(user);
            }
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
            exchange.Status = ExchangeStatus.Created;
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


        // GET
        [Authorize]
        public IActionResult Complete(string Id)
        {
            var exchange = _exchangeRepository.GetById(new Guid(Id));
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            if (user == exchange.Sender && exchange.Sender != null && exchange.Receiver != null && (exchange.Offers.Count(o => o.Status == OfferStatus.Accepted) >= 1))
            {
                exchange.Status = ExchangeStatus.Completed;
                _exchangeRepository.Update(exchange);
            }
            return RedirectToAction("Details", "Exchanges", exchange);
        }

    }
}
