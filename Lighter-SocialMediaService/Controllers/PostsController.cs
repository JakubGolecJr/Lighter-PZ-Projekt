using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lighter_SocialMediaService.Data;
using Lighter_SocialMediaService.Models;

namespace Lighter_SocialMediaService.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Nowa metoda: polubienie lub odpolubienie posta
        [HttpPost]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            var userId = User.Identity.Name; // Identyfikator użytkownika
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(like => like.PostId == postId && like.UserId == userId);

            if (existingLike != null)
            {
                // Usuń istniejące polubienie
                _context.Likes.Remove(existingLike);
                post.LikesCount--; // Zmniejsz licznik
            }
            else
            {
                // Dodaj nowe polubienie
                var like = new Like
                {
                    PostId = postId,
                    UserId = userId
                };
                _context.Likes.Add(like);
                post.LikesCount++; // Zwiększ licznik
            }

            await _context.SaveChangesAsync();

            return Json(new { likesCount = post.LikesCount });
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Posts.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content")] Post post)
        {
            Console.WriteLine($"Logged-in user's email: {User.Identity?.Name}");

            // Pobranie emaila użytkownika lub ustawienie "Anonymous" dla niezalogowanych
            var email = User.Identity?.Name ?? "Anonymous";
            post.SetAuthor(email); // Ustawienie autora

            // Usunięcie walidacji pola Author, ponieważ jest ustawiane automatycznie
            ModelState.Remove("Author");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(post);
                    await _context.SaveChangesAsync();

                    Console.WriteLine($"Post saved successfully. Author: {post.Author}");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving post: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("ModelState is invalid.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
            }

            return View(post);
        }

        // GET: Posts/Popular
        public async Task<IActionResult> Popular()
        {
            var posts = await _context.Posts
                .OrderByDescending(p => p.LikesCount)
                .ToListAsync();
            return View(posts);
        }

        // GET: Posts/Latest
        public async Task<IActionResult> Latest()
        {
            var posts = await _context.Posts
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(posts);
        }

        // GET: Posts/MyPosts
        public async Task<IActionResult> MyPosts()
        {
            // Pobranie emaila użytkownika i przetworzenie na nazwę przed "@"
            var email = User.Identity?.Name ?? "Anonymous";
            var author = email.Contains("@") ? email.Split('@')[0] : email;

            var posts = await _context.Posts
                .Where(p => p.Author == author)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(posts);
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
