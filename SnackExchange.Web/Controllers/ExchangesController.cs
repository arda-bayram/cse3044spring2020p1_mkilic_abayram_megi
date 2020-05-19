using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SnackExchange.Web.Data;
using SnackExchange.Web.Models;

namespace SnackExchange.Web.Controllers
{
    public class ExchangesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExchangesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Exchanges
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Exchanges.Include(e => e.Moderator).Include(e => e.Receiver).Include(e => e.Sender);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Exchanges/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exchange = await _context.Exchanges
                .Include(e => e.Moderator)
                .Include(e => e.Receiver)
                .Include(e => e.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exchange == null)
            {
                return NotFound();
            }

            return View(exchange);
        }

        // GET: Exchanges/Create
        public IActionResult Create()
        {
            ViewData["ModeratorId"] = new SelectList(_context.AppUsers, "Id", "Id");
            ViewData["ReceiverId"] = new SelectList(_context.AppUsers, "Id", "Id");
            ViewData["SenderId"] = new SelectList(_context.AppUsers, "Id", "Id");
            return View();
        }

        // POST: Exchanges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SenderId,ReceiverId,ModeratorId,ModeratorNotes,ExchangeNotes,PhotoUrl,TrackingNumber,Status,Id,CreatedAt,UpdatedAt")] Exchange exchange)
        {
            if (ModelState.IsValid)
            {
                exchange.Id = Guid.NewGuid();
                _context.Add(exchange);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ModeratorId"] = new SelectList(_context.AppUsers, "Id", "Id", exchange.ModeratorId);
            ViewData["ReceiverId"] = new SelectList(_context.AppUsers, "Id", "Id", exchange.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.AppUsers, "Id", "Id", exchange.SenderId);
            return View(exchange);
        }

        // GET: Exchanges/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exchange = await _context.Exchanges.FindAsync(id);
            if (exchange == null)
            {
                return NotFound();
            }
            ViewData["ModeratorId"] = new SelectList(_context.AppUsers, "Id", "Id", exchange.ModeratorId);
            ViewData["ReceiverId"] = new SelectList(_context.AppUsers, "Id", "Id", exchange.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.AppUsers, "Id", "Id", exchange.SenderId);
            return View(exchange);
        }

        // POST: Exchanges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("SenderId,ReceiverId,ModeratorId,ModeratorNotes,ExchangeNotes,PhotoUrl,TrackingNumber,Status,Id,CreatedAt,UpdatedAt")] Exchange exchange)
        {
            if (id != exchange.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exchange);
                    await _context.SaveChangesAsync();
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
            ViewData["ModeratorId"] = new SelectList(_context.AppUsers, "Id", "Id", exchange.ModeratorId);
            ViewData["ReceiverId"] = new SelectList(_context.AppUsers, "Id", "Id", exchange.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.AppUsers, "Id", "Id", exchange.SenderId);
            return View(exchange);
        }

        // GET: Exchanges/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exchange = await _context.Exchanges
                .Include(e => e.Moderator)
                .Include(e => e.Receiver)
                .Include(e => e.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exchange == null)
            {
                return NotFound();
            }

            return View(exchange);
        }

        // POST: Exchanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var exchange = await _context.Exchanges.FindAsync(id);
            _context.Exchanges.Remove(exchange);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExchangeExists(Guid id)
        {
            return _context.Exchanges.Any(e => e.Id == id);
        }
    }
}
