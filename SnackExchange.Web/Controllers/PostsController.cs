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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace SnackExchange.Web.Controllers
{
    public class PostsController : Controller
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public PostsController(IRepository<Post> postRepository, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _postRepository = postRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        // GET: Posts
        [Authorize]
        public IActionResult Index()
        {
            return View(_postRepository.GetAll());
        }

        // GET: Posts
        [Authorize]
        public IActionResult MyPosts()
        {
            return View(_postRepository.FindBy(p => p.User.Id == _userManager.GetUserId(_httpContextAccessor.HttpContext.User)));
        }

        // GET: Posts/Details/5
        [Authorize]
        public IActionResult Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            IQueryable<Post> postQuery = _postRepository.FindBy(x => x.Id == id,p => p.User);

            var post = postQuery.FirstOrDefault();
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create([Bind("Id,Title,Description")] Post post)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
                post.User = _userManager.FindByIdAsync(currentUserId).Result; // current user
                post.UserId = currentUserId;
                _postRepository.Insert(post);
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
                    var currentUserId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
                    post.User = _userManager.FindByIdAsync(currentUserId).Result; // current user
                    post.UserId = currentUserId;
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
        [Authorize]
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            var post = _postRepository.GetById(id);
            if (post == null || _userManager.GetUserId(_httpContextAccessor.HttpContext.User) != post.User.Id)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
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
