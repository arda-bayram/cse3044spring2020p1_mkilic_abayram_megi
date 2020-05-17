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
    public class CountriesController : Controller
    {
        // private readonly ApplicationDbContext _context;
        private readonly IRepository<Country> _countryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CountriesController(IRepository<Country> countryRepository, IHttpContextAccessor httpContextAccessor)
        {
            _countryRepository = countryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Countries
        public IActionResult Index()
        {
            return View(_countryRepository.GetAll());
        }

        // GET: Countries/Details/5
        public IActionResult Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            //IQueryable<Country> countryQuery = _countryRepository.GetAllLazyLoad(x => x.Id == id, c => c.Code);
            IQueryable<Country> countryQuery = _countryRepository.FindBy(x => x.Id == id);

            var country = countryQuery.FirstOrDefault();

            // var country = await _context.Countries.FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // GET: Countries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Currency,Code,Id")] Country country)
        {
            if (ModelState.IsValid)
            {
                _countryRepository.Insert(country);
                //country.Id = Guid.NewGuid();
                //_context.Add(country);
                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: Countries/Edit/5
        public IActionResult Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            //var country = await _context.Countries.FindAsync(id);
            var country = _countryRepository.GetById(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Name,Currency,Code,Id")] Country country)
        {
            if (id != country.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _countryRepository.Update(country);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.Id))
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
            return View(country);
        }

        // GET: Countries/Delete/5
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            //var country = await _context.Countries.FirstOrDefaultAsync(m => m.Id == id);
            var country = _countryRepository.GetById(id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            //var country = await _context.Countries.FindAsync(id);
            //_context.Countries.Remove(country);
            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            _countryRepository.Delete(id);
            return RedirectToAction(nameof(Index));

        }

        private bool CountryExists(Guid id)
        {
            var country = _countryRepository.GetById(id);
            return country != null;
        }
    }
}
