using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public enum UserRole
    {
        Admin,
        User
    }
    public class ComplaintMessageModel
    {
       
        public int Id { get; set; }
        [ForeignKey("Complaint")]
        public int ComplaintId { get; set; }

        public ComplaintModel Complaint { get; set; }
        public string Message { get; set; }
        public DateTime SentAtDate { get; set; }
        public UserRole Sender { get; set; }
        public bool isFromAdmin {  get; set; }


    }
}
