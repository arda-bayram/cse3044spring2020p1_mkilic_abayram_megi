using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public class OffersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Offer> _offerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Exchange> _exchangeRepository;

        private int BASETRACKINGNUMBER = 123;

        public OffersController(ApplicationDbContext context,
            IRepository<Offer> offerRepository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager,
            IRepository<Product> productRepository,
            IRepository<Exchange> exchangeRepository)
        {
            _context = context;
            _offerRepository = offerRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _productRepository = productRepository;
            _exchangeRepository = exchangeRepository;
        }

        // GET: Offers
        [Authorize]
        public IActionResult Index()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var myOffers = _offerRepository.FindBy(o => o.Offerer.Id == user.Id);
            return View(myOffers);
        }

        // GET: Offers/Details/5
        [Authorize]
        public IActionResult Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            IQueryable<Offer> offerQuery = _offerRepository.FindBy(x => x.Id == id);

            var offer = offerQuery.FirstOrDefault();
            if (offer == null)
            {
                return NotFound();
            }

            return View(offer);
        }

        // GET: Offers/Accept/5
        [Authorize]
        public IActionResult Accept(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            // current user
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            IQueryable<Offer> offerQuery = _offerRepository.FindBy(x => x.Id == id);

            Offer offer = offerQuery.FirstOrDefault();
            if (offer == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Exchange exchange = _exchangeRepository.FindBy(e => e.Id == offer.ExchangeId).FirstOrDefault();
            if (exchange == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (user.Id != exchange.SenderId)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                exchange.Receiver = offer.Offerer;
                exchange.ReceiverId = offer.Offerer.Id;
            }

            offer.Status = OfferStatus.Accepted;
            exchange.Status = ExchangeStatus.Accepted;

            exchange.Moderator = _userManager.Users.FirstOrDefault(u => u.IsModerator);
            exchange.TrackingNumber = string.Format("UPS_TEMP_{0}", BASETRACKINGNUMBER + _offerRepository.GetAll().Count());
            exchange.ModeratorNotes = "No moderator notes.";
            //reject other offers
            foreach (Offer o in exchange.Offers)
            {
                if (o.Id != offer.Id)
                {
                    o.Status = OfferStatus.Rejected;
                    o.UpdatedAt = DateTime.Now;
                }
            }
            exchange.UpdatedAt = DateTime.Now;

            _exchangeRepository.Update(exchange);
            _offerRepository.Update(offer);

            return RedirectToAction("Details", "Exchanges", exchange);
        }

        // GET: Offers/Reject/5
        [Authorize]
        public IActionResult Reject(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            // current user
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            IQueryable<Offer> offerQuery = _offerRepository.FindBy(x => x.Id == id);

            Offer offer = offerQuery.FirstOrDefault();
            if (offer == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Exchange exchange = _exchangeRepository.FindBy(e => e.Id == offer.ExchangeId).FirstOrDefault();
            if (exchange == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (user.Id != exchange.SenderId)
            {
                return RedirectToAction("Index", "Home");
            }

            offer.Status = OfferStatus.Rejected;

            offer.UpdatedAt = DateTime.Now;
            exchange.UpdatedAt = DateTime.Now;

            _exchangeRepository.Update(exchange);
            _offerRepository.Update(offer);

            return RedirectToAction("Details", "Exchanges", exchange);
        }


        // GET: Offers/Create
        [Authorize]
        public IActionResult Create(string Id)
        {
            var model = new Offer
            {
                ExchangeId = new Guid(Id)
            };
            var exchange = _exchangeRepository.GetById(model.ExchangeId);
            if (exchange.Status == ExchangeStatus.Accepted)
            {
                return RedirectToAction("Details", "Exchanges", exchange);
            }
            return View(model);
        }

        // POST: Offers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create([Bind("OfferNotes,PhotoUrl,ExchangeId,Products")] Offer offer)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

                offer.OffererId = currentUserId;
                offer.Offerer = _userManager.FindByIdAsync(currentUserId).Result;
                offer.Status = OfferStatus.Created;
                offer.Products = offer.Products.Where(p => !string.IsNullOrEmpty(p.Name) && !string.IsNullOrEmpty(p.Price)).ToList();

                foreach (Product p in offer.Products)
                {
                    p.Offer = offer;
                    p.OriginCountry = offer.Offerer.Country;
                }

                _offerRepository.Insert(offer);
                return RedirectToAction(nameof(AfterCreate), new { Id = offer.ExchangeId});
            }
            //ViewData["OffererId"] = new SelectList(_context.AppUsers, "Id", "Id", offer.OffererId);
            return View(offer);
        }

        [Authorize]
        public IActionResult AfterCreate(string Id)
        {
            var exchange = _exchangeRepository.GetById(new Guid(Id));
            return RedirectToAction("Details", "Exchanges", exchange);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult CreateProduct([Bind("Products")] Offer offer)
        {
            if (ModelState.IsValid)
            {
                offer.Products.Add(new Product());
            }
            return PartialView("Products", offer);
        }

        // GET: Offers/Edit/5
        [Authorize]
        public IActionResult Edit(Guid? id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            IQueryable<Offer> offerQuery = _offerRepository.FindBy(x => x.Id == id);

            var offer = offerQuery.FirstOrDefault();
            if (offer == null)
            {
                return NotFound();
            }

            return View(offer);
        }

        // POST: Offers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Edit(Guid id, [Bind("Id,OfferNotes,PhotoUrl,Products")] Offer offer)
        {

            if (id != offer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
                    var offerDb = _offerRepository.GetById(offer.Id);
                    if (offerDb.OffererId == user.Id || user.IsModerator)
                    {
                        offerDb.OfferNotes = offer.OfferNotes;
                        offerDb.PhotoUrl = offer.PhotoUrl;

                        //TODO: not working right now
                        foreach (var editProduct in offer.Products)
                        {
                            var oldProduct = offerDb.Products.Where(x => x.Id == editProduct.Id).FirstOrDefault();
                            if (oldProduct != null)
                            {
                                offerDb.Products.Remove(oldProduct);
                                offerDb.Products.Add(editProduct);
                            }
                        }

                        offerDb.UpdatedAt = DateTime.Now;
                        _offerRepository.Update(offerDb);
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfferExists(offer.Id))
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
            ViewData["OffererId"] = new SelectList(_context.AppUsers, "Id", "Id", offer.OffererId);
            return View(offer);

            //return View(offer);
        }

        // GET: Offers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offer = await _context.Offer
                .Include(o => o.Offerer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offer == null)
            {
                return NotFound();
            }

            return View(offer);
        }

        // POST: Offers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var offer = await _context.Offer.FindAsync(id);
            _context.Offer.Remove(offer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OfferExists(Guid id)
        {
            return _context.Offer.Any(e => e.Id == id);
        }
    }
}
