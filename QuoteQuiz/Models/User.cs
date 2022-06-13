using System;
using System.Collections.Generic;

namespace QuoteQuiz
{
    public partial class User
    {
        public User()
        {
            UserAnswers = new HashSet<UserAnswer>();
        }

        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public int Age { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool Disabled { get; set; }

        public virtual ICollection<UserAnswer> UserAnswers { get; set; }
    }
}
