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
    public class CountriesController : Controller
    {
        private readonly IRepository<Country> _countryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public CountriesController(IRepository<Country> countryRepository, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _countryRepository = countryRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        // GET: Countries
        [Authorize]
        public IActionResult Index()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (user.IsModerator)
            {
                return View(_countryRepository.GetAll());
            }
            else
            {
                var myCountry = _countryRepository.FindBy(c => c.Users.Contains(user));
                return View(myCountry);
            }
        }

        // GET: Countries/Details/5
        [Authorize]
        public IActionResult Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            IQueryable<Country> countryQuery = _countryRepository.FindBy(x => x.Id == id);

            var country = countryQuery.FirstOrDefault();
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // GET: Countries/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }


        // POST: Countries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create([Bind("Name,Currency,Code,Id")] Country country)
        {
            if (ModelState.IsValid)
            {
                _countryRepository.Insert(country);
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: Countries/Edit/5
        [Authorize]
        public IActionResult Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var country = _countryRepository.GetById(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // POST: Countries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
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
                    country.UpdatedAt = DateTime.Now;
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
        [Authorize]
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
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
        [Authorize]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _countryRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(Guid id)
        {
            var country = _countryRepository.GetById(id);
            return country != null;
        }

        public IActionResult AddNewAddress(Country country, Address address)
        {
            country.Addresses.Add(address);
            return View(country);
        }

        public IActionResult AddNewUser(Country country, AppUser user)
        {
            country.Users.Add(user);
            return View(country);
        }
    }
}
