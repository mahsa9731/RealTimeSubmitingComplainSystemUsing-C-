using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using Microsoft.AspNetCore.SignalR;

namespace WebApplication1.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDBContext _context;

        public DashboardController(ApplicationDBContext context)
        {
            _context = context;
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveMessage([FromBody] MessageDto dto)
        {
            var complaint = await _context.Complaint
               .FirstOrDefaultAsync(c => c.ComplaintId == dto.ComplaintId);
            
            if (dto == null || string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest("Message content cannot be empty.");

            if(complaint.Status == ComplaintStatus.Resolved)
            {
                return BadRequest("ارسال پیام برای این شکایت دیگر امکان پذیر نیست.");
            }
            
            var message = new ComplaintMessageModel
            {
                ComplaintId = dto.ComplaintId,
                Message = dto.Content,
                SentAtDate = DateTime.Now,
                isFromAdmin = dto.IsFromAdmin
            };

            _context.ComplaintMessage.Add(message);

            if (dto.IsFromAdmin)
            {
                complaint.Status = ComplaintStatus.WaitingForUserResponse;
            }
            else
            {
                complaint.Status = ComplaintStatus.WaitingForAdminResponse;
            }

            await _context.SaveChangesAsync();

           
            var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<ComplaintChatHub>>();

            await hubContext.Clients.Group(dto.ComplaintId.ToString())
                .SendAsync("ReceiveMessage",
                    dto.IsFromAdmin ? "پشتیبانی" : "شما",
                    dto.Content,
                    message.SentAtDate.ToString("yyyy/MM/dd HH:mm"));

            Console.WriteLine($"ComplaintId received: {dto.ComplaintId}, Message: {dto.Content}");

            return Ok();
        }

        public IActionResult Chat(int complaintId)
        {
            var complaint = _context.Complaint.Find(complaintId);
            var messages = _context.ComplaintMessage
                .Where(m => m.ComplaintId == complaintId)
                .OrderBy(m => m.SentAtDate)
                .ToList();

            var viewModel = new ComplaintModel
            {
                //Complaint = complaint,
                Messages = messages
            };

            return View(viewModel);
        }


    }
}
