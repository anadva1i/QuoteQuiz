namespace QuoteQuiz.Models
{
    public class ResultsModel
    {
        public Question? Question { get; set; }
        public Author? Author { get; set; }
        public string? UserAnswer { get; set; }
    }
}
