namespace QuoteQuiz.Models
{
    public class QuizModel
    {
        public Question ?Question { get; set; }
        public ICollection<Author> ?Authors { get; set; }
        public int? UserAnswer { get; set; }
        public bool? Correct { get; set; }
        public int Counter { get; set; }
        public int MaxQuestions { get; set; }
    }
}
