using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SnackExchange.Web.Data;
using SnackExchange.Web.Models;
using SnackExchange.Web.Repository;

namespace SnackExchange.Web.Controllers
{
    public class ExchangesController : Controller
    {
        // private readonly ApplicationDbContext _context;
        private readonly IRepository<Exchange> _exchangeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExchangesController(IRepository<Exchange> exchangeRepository, IHttpContextAccessor httpContextAccessor)
        {
            _exchangeRepository = exchangeRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Exchanges
        public IActionResult Index()
        {
            return View(_exchangeRepository.GetAll());
        }

        // GET: Exchanges/Details/5
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Exchanges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("SenderId,ReceiverId,ModeratorId,ModeratorNotes,ExchangeNotes,PhotoUrl,TrackingNumber,Id")] Exchange exchange)
        {
            if (ModelState.IsValid)
            {
                _exchangeRepository.Insert(exchange);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("SenderId,ReceiverId,ModeratorId,ModeratorNotes,ExchangeNotes,PhotoUrl,TrackingNumber,Id")] Exchange exchange)
        {
            if (id != exchange.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
        public IActionResult Delete(Guid id)
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

        // POST: Exchanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
    }
}
