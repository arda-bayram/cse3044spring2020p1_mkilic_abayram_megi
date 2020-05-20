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
    public class ProductsController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductsController(IRepository<Product> productRepository, IHttpContextAccessor httpContextAccessor)
        {
            _productRepository = productRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View(_productRepository.GetAll());
        }

        // GET: Products/Details/5
        public IActionResult Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            IQueryable<Product> productQuery = _productRepository.FindBy(x => x.Id == id);

            var product = productQuery.FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }


        // POST: Products/Create
        // To protect from overproducting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Description,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepository.Insert(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public IActionResult Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overproducting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Name,Description,Price")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _productRepository.Update(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _productRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(Guid id)
        {
            var product = _productRepository.GetById(id);
            return product != null;
        }
    }
}
