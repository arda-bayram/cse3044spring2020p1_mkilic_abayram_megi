﻿using System;
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
    public class AddressesController : Controller
    {
        private readonly IRepository<Address> _addressRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public AddressesController(IRepository<Address> addressRepository, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _addressRepository = addressRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        // GET: Addresses
        [Authorize]
        public IActionResult Index()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (user.IsModerator)
            {
                return View(_addressRepository.GetAll());
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
                //var myAddresses = _addressRepository.FindBy(a => a.UserId == user.Id);
                //return View(myAddresses);
            }

        }

        // GET: Addresses
        [Authorize]
        public IActionResult MyAddresses()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var myAddress = _addressRepository.FindBy(p => p.User.Id == user.Id);
            return View(user);
        }

        // GET: Addresses/Details/5
        [Authorize]
        public IActionResult Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            IQueryable<Address> addressQuery = _addressRepository.FindBy(x => x.Id == id, p => p.User);

            var address = addressQuery.FirstOrDefault();
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // GET: Addresses/Create
        [Authorize]
        public IActionResult Create()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (!user.IsModerator)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }


        // POST: Addresses/Create
        // To protect from overaddressing attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create([Bind("Id,Title,Text,PlusCode")] Address address)
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (!user.IsModerator)
            {
                return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                address.User = user; // current user
                address.UserId = user.Id;
                address.Country = address.User.Country;
                _addressRepository.Insert(address);
                return RedirectToAction(nameof(Index));
            }
            return View(address);
        }

        // GET: Addresses/Edit/5
        public IActionResult Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (!user.IsModerator)
            {
                return RedirectToAction(nameof(Index));
            }

            var address = _addressRepository.GetById(id);
            if (address == null)
            {
                return NotFound();
            }
            return View(address);
        }

        // POST: Addresses/Edit/5
        // To protect from overaddressing attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,Title,Text,PlusCode")] Address address)
        {
            if (id != address.Id)
            {
                return NotFound();
            }
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (!user.IsModerator)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentUserId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
                    address.User = _userManager.FindByIdAsync(currentUserId).Result; // current user
                    address.UserId = currentUserId;
                    address.Country = address.User.Country;
                    address.UpdatedAt = DateTime.Now;
                    _addressRepository.Update(address);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddressExists(address.Id))
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
            return View(address);
        }

        // GET: Addresses/Delete/5
        [Authorize]
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (!user.IsModerator)
            {
                return RedirectToAction(nameof(Index));
            }

            var address = _addressRepository.GetById(id);
            if (address == null || _userManager.GetUserId(_httpContextAccessor.HttpContext.User) != address.User.Id)
            {
                return NotFound();
            }

            return View(address);
        }

        // POST: Addresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (!user.IsModerator)
            {
                return RedirectToAction(nameof(Index));
            }
            _addressRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private bool AddressExists(Guid id)
        {
            var address = _addressRepository.GetById(id);
            return address != null;
        }
    }
}
