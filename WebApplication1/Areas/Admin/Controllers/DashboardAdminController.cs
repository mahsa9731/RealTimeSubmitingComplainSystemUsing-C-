using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;



namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(policy:"AdminsOnly")]
    public class DashboardAdminController : Controller
    {
     
        private readonly ApplicationDBContext _context;
        private readonly IHubContext<ComplaintChatHub> _hubContext;
        public DashboardAdminController(ApplicationDBContext context, IHubContext<ComplaintChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var complaints = await _context.Complaint
                .Where(c=> c.Status != ComplaintStatus.Resolved).Include(c => c.Messages).ToListAsync();
            return View(complaints);
        }

        public async Task<IActionResult> DetailsForAdmin(int id)
        {
            var complaint = await _context.Complaint
                                .Include(c => c.Messages)
                                .FirstOrDefaultAsync(c => c.ComplaintId == id);

            if (complaint == null) return NotFound();
            return View(complaint);
        }

        [HttpGet]
        public IActionResult Chat(int complaintId)
        {
            var complaint = _context.Complaint
                .Where(c => c.ComplaintId == complaintId)
                .FirstOrDefault();

            if (complaint == null)
                return NotFound();

            
            var messages = _context.ComplaintMessage
                .Where(m => m.ComplaintId == complaintId)
                .OrderBy(m => m.SentAtDate)
                .ToList();

            complaint.Messages = messages;

            return View(complaint); 
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveMessage([FromBody] MessageDto dto)
        {
            var complaint = await _context.Complaint
               .FirstOrDefaultAsync(c => c.ComplaintId == dto.ComplaintId);

            if (dto == null || string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest("Message content cannot be empty.");

            var message = new ComplaintMessageModel
            {
                ComplaintId = dto.ComplaintId,
                Message = dto.Content,
                SentAtDate = DateTime.Now,
                isFromAdmin = dto.IsFromAdmin,
                Sender = UserRole.Admin
            };

            _context.ComplaintMessage.Add(message);
            
            if(dto.IsFromAdmin)
            {
                complaint.Status = ComplaintStatus.WaitingForUserResponse;
            }
            else
            {
                complaint.Status = ComplaintStatus.WaitingForAdminResponse;
            }
            
            await _context.SaveChangesAsync();

            
            await _hubContext.Clients.Group(dto.ComplaintId.ToString())
                .SendAsync("ReceiveMessage",
                    "ادمین",
                    dto.Content,
                    message.SentAtDate.ToString("yyyy/MM/dd HH:mm"));

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseComplaint([FromBody] MessageDto dto)
        {
            var complaint = await _context.Complaint
                .FirstOrDefaultAsync(c => c.ComplaintId == dto.ComplaintId);

            if (complaint == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest("پیام ادمین نمی‌تواند خالی باشد.");

           
            var message = new ComplaintMessageModel
            {
                ComplaintId = dto.ComplaintId,
                Message = dto.Content,
                SentAtDate = DateTime.Now,
                isFromAdmin = true,
                Sender = UserRole.Admin
            };
            _context.ComplaintMessage.Add(message);


            complaint.Status = ComplaintStatus.Resolved;
            await _context.SaveChangesAsync();

           
            var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<ComplaintChatHub>>();
            await hubContext.Clients.Group(dto.ComplaintId.ToString())
                .SendAsync("ReceiveMessage", "ادمین", dto.Content, message.SentAtDate.ToString("yyyy/MM/dd HH:mm"));

            return Ok();
        }


    }
}

