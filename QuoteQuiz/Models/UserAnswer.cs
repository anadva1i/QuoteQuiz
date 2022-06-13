using System;
using System.Collections.Generic;

namespace QuoteQuiz
{
    public partial class UserAnswer
    {
        public int Id { get; set; }
        public int QuoteId { get; set; }
        public int UserId { get; set; }
        public bool? Correct { get; set; }
        public int? UserAnswer1 { get; set; }

        public virtual Question Quote { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual Author? UserAnswer1Navigation { get; set; }
    }
}
