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
    public class QuestionsController : Controller
    {
        private readonly DB_QuoteQuizContext _context;

        public QuestionsController(DB_QuoteQuizContext context)
        {
            _context = context;
        }

        // GET: Questions
        public async Task<IActionResult> Index()
        {
            var dB_QuoteQuizContext = _context.Questions.Include(q => q.Author);
            return View(await dB_QuoteQuizContext.ToListAsync());
        }

       

        // GET: Questions/Create
        public IActionResult Create()
        {
            ViewData["Author"] = new SelectList(_context.Authors, "Id", "FullName");
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Author,Text")] QuotesModel quote)
        {
            try
            {
                using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope())
                {
                    int authorId = 0;
                    foreach (var author in _context.Authors)
                    {
                        if (author.FullName.ToLower() == quote.Author.ToLower())
                            authorId = author.Id;
                    }
                    if (authorId == 0)
                    {
                        Author author = new Author();
                        author.FullName = quote.Author;
                        _context.Authors.Add(author);
                        _context.SaveChanges();
                        authorId = author.Id;
                    }
                    Question question = new Question();
                    question.Text = quote.Text;
                    question.AuthorId = authorId;
                    _context.Questions.Add(question);
                    _context.SaveChanges();
                    ts.Complete();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("Text", "Something went wrong. try again later...");
                return View("Create");
            }
        }

        // GET: Questions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var question = await _context.Questions.FindAsync(id);
            var author = await _context.Authors.FindAsync(question.AuthorId);
            QuotesModel quote = new QuotesModel();
            quote.Id = question.Id;
            quote.Text = question.Text;
            quote.Author = author.FullName;
            if (question == null)
            {
                return NotFound();
            }
            
            //ViewData["Author"] = _context.Authors.FindAsync(question.AuthorId);
            return View(quote);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Author,Text")] QuotesModel quote)
        {
            if (id != quote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using(TransactionScope ts = new TransactionScope())
                    {
                        int authorId = 0;
                        foreach (var author in _context.Authors)
                        {
                            if (author.FullName.ToLower() == quote.Author.ToLower())
                                authorId = author.Id;
                        }
                        if (authorId == 0)
                        {
                            Author author = new Author();
                            author.FullName = quote.Author;
                            _context.Authors.Add(author);
                            _context.SaveChanges();
                            authorId = author.Id;
                        }
                        Question question = _context.Questions.FirstOrDefault(q => q.Id == id); 
                        question.Text = quote.Text;
                        question.AuthorId = authorId;
                        _context.SaveChanges();
                        ts.Complete();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(quote.Id))
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
            //ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Id", question.AuthorId);
            return View(Index);
        }

        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_context.Questions == null)
            {
                return Problem("Entity set 'DB_QuoteQuizContext.Questions'  is null.");
            }
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var tests = _context.UserAnswers.Where(u => u.QuoteId == id);
                    foreach (var test in tests)
                    {
                        _context.UserAnswers.Remove(test);
                    }
                    _context.SaveChanges();
                    var question = _context.Questions.Find(id);
                    if (question != null)
                    {
                        _context.Questions.Remove(question);
                    }
                    _context.SaveChanges();
                    ts.Complete();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Problem("Something went wrong. try again later");
            }
        }

        private bool QuestionExists(int id)
        {
          return (_context.Questions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
