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
        public string Content { get; set; } // Treść wpisu
        public string Author { get; private set; } // Email bez domeny (ustawiane automatycznie
        public int LikesCount { get; set; } = 0;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow; // Automatyczna data utworzenia w UTC
        public List<Comment> Comments { get; set; } = new List<Comment>();


        // Metoda do ustawiania autora
        public void SetAuthor(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                Author = "Anonymous";
            }
            else
            {
                Author = email.Contains("@") ? email.Split('@')[0] : email; // Wyodrębnia nazwę przed '@'
            }
        }

        // Metoda do dodawania polubień
        public void AddLike()
        {
            LikesCount++;
        }
    }

    public class Comment
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Treść komentarza jest wymagana.")]
        [StringLength(300, ErrorMessage = "Komentarz nie może przekraczać 300 znaków.")]
        public string Content { get; set; } // Treść komentarza
        [Required(ErrorMessage = "Autor komentarza jest wymagany.")]
        public string Author { get; set; } // Autor komentarza
        public int PostId { get; set; } // ID powiązanego posta
        public Post Post { get; set; } // Powiązany post
        public DateTime CreatedAt { get; private set; } = DateTime.Now; // Automatyczna data utworzenia
    }
}
