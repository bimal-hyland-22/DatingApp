

namespace API.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string  SenderUserName { get; set; }
        public string SenderPhotoUrl { get; set; }
        public int RecipentId { get; set; }
        public string RecipentUserName { get; set; }
        public string RecipentPhotoUrl { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } 
    }
}