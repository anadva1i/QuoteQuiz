using System;
using System.Collections.Generic;

namespace QuoteQuiz
{
    public partial class Question
    {
        public Question()
        {
            UserAnswers = new HashSet<UserAnswer>();
        }

        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string? Text { get; set; }

        public virtual Author Author { get; set; } = null!;
        public virtual ICollection<UserAnswer> UserAnswers { get; set; }
    }
}
