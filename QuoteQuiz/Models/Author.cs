using System;
using System.Collections.Generic;

namespace QuoteQuiz
{
    public partial class Author
    {
        public Author()
        {
            Questions = new HashSet<Question>();
            UserAnswers = new HashSet<UserAnswer>();
        }

        public int Id { get; set; }
        public string FullName { get; set; } = null!;

        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<UserAnswer> UserAnswers { get; set; }
    }
}
