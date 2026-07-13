using Microsoft.AspNetCore.SignalR;

public class ComplaintChatHub : Hub
{
    // ارسال پیام به گروه خاص (بر اساس complaintId)
    public async Task SendMessage(string complaintId, string sender, string message)
    {
        await Clients.Group(complaintId).SendAsync("ReceiveMessage", sender, message, DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
    }

    public override async Task OnConnectedAsync()
    {
        var complaintId = Context.GetHttpContext()?.Request.Query["complaintId"];

        if (!string.IsNullOrEmpty(complaintId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, complaintId);
        }

        await base.OnConnectedAsync();
    }
}

