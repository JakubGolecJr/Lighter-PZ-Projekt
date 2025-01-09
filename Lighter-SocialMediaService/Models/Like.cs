namespace Lighter_SocialMediaService.Models
{
    public class Like
    {
        public int Id { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; } // Relacja do posta

        public string UserId { get; set; } // Identyfikator użytkownika
    }
}
