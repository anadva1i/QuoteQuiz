using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuoteQuiz;
using QuoteQuiz.Models;

namespace QuoteQuiz.Controllers
{
    public class UsersController : Controller
    {
        private readonly DB_QuoteQuizContext _context;

        public UsersController(DB_QuoteQuizContext context)
        {
            _context = context;
        }

        public IActionResult LogOut()
        {
            string user = HttpContext.Request.Cookies["user"];
            if(user != null)
            {
                HttpContext.Response.Cookies.Delete("user");
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
              return _context.Users != null ? 
                          View(await _context.Users.ToListAsync()) :
                          Problem("Entity set 'DB_QuoteQuizContext.Users'  is null.");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Results(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
           ViewBag.User = _context.Users.Find(id).UserName;
           ViewBag.CurrentUser = HttpContext.Request.Cookies["user"];
            var userAnswers = _context.UserAnswers.Where(u => u.UserId == id);
            List<ResultsModel> modelList = new List<ResultsModel>();
            foreach(var answer in userAnswers)
            {
                ResultsModel model = new ResultsModel();
                model.Question = await _context.Questions.FindAsync(answer.QuoteId);
                model.Author = await _context.Authors.FindAsync(model.Question.AuthorId);
                Author userAnswered = await _context.Authors.FindAsync(answer.UserAnswer1);
                model.UserAnswer = userAnswered.FullName;
                if (answer.Correct == false)
                    model.UserAnswer = "Not " + userAnswered.FullName;
                if (model.UserAnswer == model.Author.FullName)
                    model.Class = "bg-correct";
                else model.Class = "bg-wrong";
                modelList.Add(model);
            }

            return View(modelList);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Age,RegistrationDate,Disabled")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,Age,RegistrationDate,Disabled")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public  IActionResult DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'DB_QuoteQuizContext.Users'  is null.");
            }
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var tests = _context.UserAnswers.Where(u => u.UserId == id);
                    foreach (var test in tests)
                    {
                        _context.UserAnswers.Remove(test);
                    }
                    _context.SaveChanges();
                    var user = _context.Users.Find(id);
                    if (user != null)
                    {
                        _context.Users.Remove(user);
                    }
                    _context.SaveChanges();
                    ts.Complete();
                }
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return Problem("Something went wrong. try again later");
            }
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
