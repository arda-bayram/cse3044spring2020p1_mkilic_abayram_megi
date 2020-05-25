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

        public ExchangesController(
            IRepository<Exchange> exchangeRepository,
            IRepository<ExchangeUserModel> exchangeUserModelRepository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager)
        {
            _exchangeRepository = exchangeRepository;
            _exchangeUserModelRepository = exchangeUserModelRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        // GET: Exchanges
        public IActionResult Index()
        {
            return View(_exchangeRepository.GetAll());
        }

        //// GET: Exchanges
        //[Authorize]
        //public IActionResult MyExchanges()
        //{
        //    return View(_exchangeRepository.FindBy(p => p.Sender.Id == _userManager.GetUserId(_httpContextAccessor.HttpContext.User)));
        //}

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
            return View();
        }


        // POST: Exchanges/Create
        // To protect from overexchangeing attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create([Bind("ExchangeNotes,PhotoUrl")] Exchange exchange)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
                exchange.Sender = _userManager.FindByIdAsync(currentUserId).Result; // current user
                exchange.SenderId = currentUserId;
                exchange.Receiver = null;
                exchange.Moderator = null;
                exchange.ModeratorNotes = String.Empty;
                exchange.TrackingNumber = String.Empty;
                exchange.Status = ExchangeStatus.Created;
                _exchangeRepository.Insert(exchange);

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
        // To protect from overexchangeing attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("ExchangeNotes,PhotoUrl")] Exchange exchange)
        {
            if (id != exchange.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentUserId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
                    exchange.Sender = _userManager.FindByIdAsync(currentUserId).Result; // current user
                    exchange.SenderId = currentUserId;
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
