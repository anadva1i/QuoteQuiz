using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuoteQuiz.Models;
using System.Diagnostics;

namespace QuoteQuiz.Controllers
{
    public class HomeController : Controller
    {
        private readonly DB_QuoteQuizContext _context;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DB_QuoteQuizContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            string user = HttpContext.Request.Cookies["user"];
            var db_user = _context.Users.FirstOrDefault(u => u.UserName.Equals(user));
            if(db_user == null)
               HttpContext.Response.Cookies.Delete("user");
            if (db_user != null)
            {
                return RedirectToAction("Quiz");
            }
            else
                return View();
        }

        public IActionResult Result()
        {
            return View();
        }

        public async Task<IActionResult> Quiz()
        {
            string user = HttpContext.Request.Cookies["user"];
            var db_user = _context.Users.FirstOrDefault(u => u.UserName.Equals(user));
            if (db_user == null)
                HttpContext.Response.Cookies.Delete("user");
            if (db_user == null)
            {
                return RedirectToAction("Index");
            }
            int userId = _context.Users.FirstOrDefault(u => u.UserName == user).Id;
            Random random = new Random();
            var dB_QuoteQuizContext = _context.Questions.Include(q => q.Author).ToList();
            int r = random.Next(dB_QuoteQuizContext.Count());
            var previousQuestions = _context.UserAnswers.Where(a => a.UserId == userId).ToList();
            if (previousQuestions.Count < 10 && previousQuestions.Count < dB_QuoteQuizContext.Count())
            {
                while (previousQuestions.Where(c => c.QuoteId == dB_QuoteQuizContext[r].Id).Count() > 0)
                {
                    r = random.Next(dB_QuoteQuizContext.Count());
                }
                QuizModel model = new QuizModel();
                model.MaxQuestions = 10;
                if (dB_QuoteQuizContext.Count() < model.MaxQuestions)
                    model.MaxQuestions = dB_QuoteQuizContext.Count();
                model.Counter = previousQuestions.Count() + 1;
                model.Question = dB_QuoteQuizContext[r];
                model.Authors = new List<Author>();
                int authorIndex = random.Next(_context.Authors.Count());
                var randomAuthor = _context.Authors.ToList()[authorIndex];
                int randIndex = random.Next(3);
                for (int i = 0; i < 3; i++)
                {
                    while (model.Authors.Contains(randomAuthor))
                    {
                        authorIndex = random.Next(_context.Authors.Count());
                        randomAuthor = _context.Authors.ToList()[authorIndex];
                    }
                    if (i == randIndex && !model.Authors.Contains(dB_QuoteQuizContext[r].Author))
                        model.Authors.Add(dB_QuoteQuizContext[r].Author);
                    else
                        model.Authors.Add(randomAuthor);
                }
                TempData["author"] = dB_QuoteQuizContext[r].Author.FullName;
                return View(model);
            }
            else return RedirectToAction("results", "users", new { id = userId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser([Bind("Id,UserName,Age")] User user)
        {
            if ((_context.Users?.Any(e => e.UserName == user.UserName)).GetValueOrDefault())
            {
                ModelState.AddModelError("UserName", "User already exists! Please try different one");
                return View("Index", user);
            }
            else if(user.Age == 0)
            {
                ModelState.AddModelError("Age", "Please Enter Your Age");
                return View("Index", user);
            }
            else
            {
                try
                {

                    User newUser = new User();
                    newUser.UserName = user.UserName;
                    newUser.Age = user.Age;
                    newUser.RegistrationDate = DateTime.Now;
                    newUser.Disabled = false;
                    _context.Users.Add(newUser);
                    _context.SaveChanges();
                    HttpContext.Response.Cookies.Append("user", newUser.UserName);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("UserName", "Something went wrong. Please try again later");
                    return View("Index", user);
                }
                return RedirectToAction("Quiz");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAnswer(QuizModel quiz)
        {
            quiz.Authors = new List<Author>();
            UserAnswer answer = new UserAnswer();
            string user = HttpContext.Request.Cookies["user"];
            int userId = _context.Users.FirstOrDefault(u => u.UserName == user).Id;
            answer.UserId = userId;
            answer.QuoteId = quiz.Question.Id;
            answer.UserAnswer1 = quiz.UserAnswer;
            if (quiz.Correct != null)
            {
                answer.Correct = quiz.Correct;
            }
            _context.UserAnswers.Add(answer);
            _context.SaveChanges();
            return RedirectToAction("Quiz");
        }

        public IActionResult Settings()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}