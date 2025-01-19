using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lighter_SocialMediaService.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Wpis jest pusty, uzupełnij pole.")]
        [StringLength(380, ErrorMessage = "Długość posta nie może przekroczyć 380 znaków.")]
        public string Content { get; set; }
        public string Author { get; private set; }
        public int LikesCount { get; set; } = 0;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public List<Comment> Comments { get; set; } = new List<Comment>();


        public void SetAuthor(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                Author = "Anonymous";
            }
            else
            {
                Author = email.Contains("@") ? email.Split('@')[0] : email; //tworzenie nicku
            }
        }

        public void AddLike()
        {
            LikesCount++;
        }
    }

    public class Comment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Treść komentarza jest wymagana.")]
        [StringLength(150, ErrorMessage = "Komentarz nie może przekraczać 150 znaków.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Autor komentarza jest wymagany.")]
        public string Author { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public string ReplyTo { get; set; }
    }
}
