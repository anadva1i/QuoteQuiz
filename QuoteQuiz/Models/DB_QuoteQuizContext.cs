using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace QuoteQuiz
{
    public partial class DB_QuoteQuizContext : DbContext
    {
        public DB_QuoteQuizContext()
        {
        }

        public DB_QuoteQuizContext(DbContextOptions<DB_QuoteQuizContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Author> Authors { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserAnswer> UserAnswers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-371GG7N\\SQLEXPRESS;Initial Catalog=DB_QuoteQuiz;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(entity =>
            {
                entity.Property(e => e.FullName).HasMaxLength(50);
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.Property(e => e.Text).HasMaxLength(150);

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Questions_Authors");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.RegistrationDate).HasColumnType("date");

                entity.Property(e => e.UserName).HasMaxLength(50);
            });

            modelBuilder.Entity<UserAnswer>(entity =>
            {
                entity.Property(e => e.UserAnswer1).HasColumnName("UserAnswer");

                entity.HasOne(d => d.Quote)
                    .WithMany(p => p.UserAnswers)
                    .HasForeignKey(d => d.QuoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAnswers_Questions");

                entity.HasOne(d => d.UserAnswer1Navigation)
                    .WithMany(p => p.UserAnswers)
                    .HasForeignKey(d => d.UserAnswer1)
                    .HasConstraintName("FK_UserAnswers_Authors");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserAnswers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAnswers_Users");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
