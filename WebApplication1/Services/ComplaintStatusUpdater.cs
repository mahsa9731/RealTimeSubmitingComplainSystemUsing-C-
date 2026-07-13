using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class ComplaintStatusUpdater : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ComplaintStatusUpdater(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                    var complaintsToUpdate = _context.Complaint
                        .Where(c => c.Status == ComplaintStatus.New &&
                                    c.DateOfCreateComplaint <= DateTime.Now.AddHours(-1)).ToList();


                    foreach (var complaint in complaintsToUpdate)
                    {
                        complaint.Status = ComplaintStatus.InProgress;
                    }

                    if (complaintsToUpdate.Any())
                        await _context.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

    }
}
   





