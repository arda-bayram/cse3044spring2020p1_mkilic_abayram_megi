using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SnackExchange.Web.Data;
using SnackExchange.Web.Models;
using System.Security.Claims;
using SnackExchange.Web.Models.Auth;
using SnackExchange.Web.Repository;
using Microsoft.AspNetCore.Http;

namespace SnackExchange.Web.Controllers
{
    public class PostsController : Controller
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostsController(IRepository<Post> postRepository, IHttpContextAccessor httpContextAccessor)
        {
            _postRepository = postRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Posts
        public IActionResult Index()
        {
            return View(_postRepository.GetAll());
        }

        // GET: Posts/Details/5
        public IActionResult Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            IQueryable<Post> postQuery = _postRepository.GetAllLazyLoad(x => x.Id == id,p => p.User);

            var post = postQuery.FirstOrDefault();

            //var post = await _context.Posts.Include(u => u.User).FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Title,Description")] Post post)
        {
            if (ModelState.IsValid)
            {
                /*var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                post.User = _context.AppUsers.FirstOrDefault(u => u.Id == userId);*/
                //_context.Posts.Add(post);

                string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                post.UserId = userId;
                _postRepository.Insert(post);
                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        public IActionResult Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            //var post = await _context.Posts.FindAsync(id);
            var post = _postRepository.GetById(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,Title,Description")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _postRepository.Update(post);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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
            return View(post);
        }

        // GET: Posts/Delete/5
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            //var post = await _context.Posts.FirstOrDefaultAsync(m => m.Id == id);
            var post = _postRepository.GetById(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            //            var post = await _context.Posts.FindAsync(id);
            //var post = _postRepository.GetById(id);
            _postRepository.Delete(id);
            //_context.Posts.Remove(post);
            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(Guid id)
        {
            var post = _postRepository.GetById(id);
            return post != null;
        }
    }
}
