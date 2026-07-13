using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;


namespace WebApplication1.Controllers
{
    [Authorize]
    public class ComplaintsController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> _contextUserManager;

        public ComplaintsController(ApplicationDBContext context ,UserManager<User> contextUserManager )
        {
            _context = context;
            _contextUserManager = contextUserManager;
        }
        
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if(User.IsInRole("Admin"))
            {
                var allComplaints = await _context.Complaint.OrderByDescending(c => c.DateOfCreateComplaint).ToListAsync();
                return View(allComplaints);
            }
            else
            {
                var userName = User.Identity.Name;
                var UserComplaints = await _context.Complaint.Where(c => c.UserId == userId).OrderByDescending(c => c.DateOfCreateComplaint).ToListAsync();
                return View(UserComplaints);
            }
            
        }
        public async Task<IActionResult> Details(int id)
        {
            var Complaint = await _context.Complaint.Include(c=> c.Messages).FirstOrDefaultAsync(i => i.ComplaintId == id);
            
            if(Complaint == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if(!User.IsInRole("Admin") && Complaint.UserId != userId)
            {
                return Forbid();
            }

            //_context.SaveChanges();
            //return RedirectToAction(nameof(StatusCheck), new { id = Complaint.ComplaintId });
            return View(Complaint);

        }

        // Submit Complain(Get)

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var currentUser = await _contextUserManager.GetUserAsync(User);
            var Email = currentUser.Email;

            return View();        }
        //Post

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComplaintModel complaint)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            complaint.UserId = userId;
            complaint.TrackingCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            complaint.Status = ComplaintStatus.New;
            complaint.DateOfCreateComplaint = DateTime.Now;
    

            ModelState.Remove(nameof(complaint.UserId));
            ModelState.Remove(nameof(complaint.TrackingCode));


            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                foreach (var error in errors)
                {
                    Console.WriteLine("Model Error: " + error);
                }

                return View(complaint);
            }

            _context.Add(complaint);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = complaint.ComplaintId });
        }

        public async Task<IActionResult> StatusCheck()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var complaints = await _context.Complaint
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.DateOfCreateComplaint)
                .ToListAsync();

            return View(complaints);
        }

        public async Task<IActionResult> DetailsForRegisteredComplaint(int id)
        {
            var complaint = await _context.Complaint
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.ComplaintId == id);

            if (complaint == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (complaint.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return View(complaint);
        }



        // Reply
        //[HttpPost]
        //public async Task<IActionResult> Reply(int complaintId, string message, UserRole sender)
        //{
        //    //var complaintMess = await _context.ComplaintMessage.FindAsync(complaintId);
        //    var complaint = await _context.Complaint.FindAsync(complaintId);
        //    if (complaint == null)
        //    {
        //        return NotFound();
        //    }

        //    complaint.Messages.Add(
        //        new ComplaintMessageModel()
        //    {
        //        ComplaintId = complaintId,
        //        Message = message,
        //        Sender = sender
        //    });
            
        //    if (sender == UserRole.User && complaint.Status == ComplaintStatus.WaitingForUserResponse)
        //        complaint.Status = ComplaintStatus.WaitingForUserResponse;

        //    _context.Update(complaint);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("Details", new { id = complaintId });
        //}
            

          //Change Status only with Admin

         [Authorize(Roles = "Admin")]
        async Task<IActionResult> ChangeStatus(int id, ComplaintStatus newStatus)
        {
            var complaint = await _context.Complaint.FindAsync(id);
            if (complaint == null)
            {
                return NotFound();
            }
            complaint.Status = newStatus;
            _context.Update(complaint);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details) , new {id});
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(ComplaintStatus status,int complaintId)
        {
            var complaint = await _context.Complaint.FindAsync(complaintId);

            if (complaint == null)
            {
                return NotFound();
            }

            complaint.Status = status;
            _context.Update(complaint);

            await _context.SaveChangesAsync();

            return RedirectToAction("Details" , new {id = complaintId});
        }

        // Close after solving

        [HttpPost]
        public async Task<IActionResult> CloseComplaint(int complainId)
        {
            var complaint = await _context.Complaint.FindAsync(complainId);

            if (complaint == null)
            {
                return NotFound();
            }

            complaint.Status = ComplaintStatus.Resolved;

            _context.Update(complaint);

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = complainId });
        }

    }

}
