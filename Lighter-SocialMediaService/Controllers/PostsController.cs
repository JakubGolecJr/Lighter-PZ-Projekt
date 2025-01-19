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

        [HttpPost]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                TempData["Error"] = "Komentarz nie może być pusty.";
                return RedirectToAction(nameof(Details), new { id = postId });
            }

            if (content.Length > 150)
            {
                TempData["Error"] = "Komentarz nie może przekraczać 150 znaków.";
                return RedirectToAction(nameof(Details), new { id = postId });
            }

            var post = await _context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
            {
                return NotFound();
            }

            string replyTo = null;

            //Sprawdz czy komentarz ma nick użytkownika
            if (content.StartsWith("@"))
            {
                var parts = content.Split(new[] { ' ' }, 2);
                if (parts.Length > 1 && parts[0].EndsWith("-"))
                {
                    replyTo = parts[0].TrimEnd('-');
                    content = parts.Length > 1 ? parts[1].Trim() : "";
                }
            }

            //Tworzenie nicku do tworzenia posta
            var email = User.Identity?.Name ?? "Anonymous";
            var author = email.Contains("@") ? email.Split('@')[0] : email;

            var comment = new Comment
            {
                Content = content,
                Author = author,
                PostId = postId,
                ReplyTo = replyTo ?? string.Empty
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = postId });
        }

        [HttpPost]
        //Lajki
        public async Task<IActionResult> ToggleLike(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            var userId = User.Identity.Name;
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(like => like.PostId == postId && like.UserId == userId);

            if (existingLike != null)
            {
                //Odlub
                _context.Likes.Remove(existingLike);
                post.LikesCount--;
            }
            else
            {
                //Polub
                var like = new Like
                {
                    PostId = postId,
                    UserId = userId
                };
                _context.Likes.Add(like);
                post.LikesCount++;
            }

            await _context.SaveChangesAsync();

            return Json(new { likesCount = post.LikesCount });
        }

        // GET: Posts
        //Pod dodaniu posta przenies na Latest (jak na wykopie)
        public async Task<IActionResult> Index()
        {
            return RedirectToAction(nameof(Latest));
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

            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
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
            //Tworzenie nicku
            var email = User.Identity?.Name ?? "Anonymous";
            post.SetAuthor(email);

            ModelState.Remove("Author");

            if (string.IsNullOrEmpty(post.Content))
            {
                ViewBag.Error = "Post nie może być pusty.";
                return View(post);
            }

            if (post.Content.Length > 380)
            {
                ViewBag.Error = "Post nie może przekraczać 380 znaków.";
                return View(post);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving post: {ex.Message}");
                }
            }

            return View(post);
        }

        // GET: Posts/Popular
        //Listuje posty od najpopularniejszych
        public async Task<IActionResult> Popular()
        {
            var posts = await _context.Posts
                .OrderByDescending(p => p.LikesCount)
                .ToListAsync();
            return View(posts);
        }

        // GET: Posts/Latest
        //Listuje posty od daty (CreatedAt) najnowsze
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
            //Tworzenie nicku
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
