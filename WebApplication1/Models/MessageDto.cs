namespace WebApplication1.Models
{
    public class MessageDto
    {
        public int ComplaintId { get; set; }
        public string Content { get; set; }
        public bool IsFromAdmin { get; set; }
    }
}
