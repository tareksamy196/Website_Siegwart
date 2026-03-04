using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Website.Siegwart.BLL.Services.Interfaces;
using Website.Siegwart.DAL.Data.Contexts;
using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.PL.Services
{
    public class ContactService : IContactService
    {
        private readonly AppDbContext _db;
        private readonly IAppEmailSender? _emailSender;
        private readonly ILogger<ContactService> _logger;

        public ContactService(AppDbContext db, IAppEmailSender? emailSender, ILogger<ContactService> logger)
        {
            _db = db;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task<ContactMessage> SaveAsync(UserContactFormDto vm, string? ip = null, string? userAgent = null)
        {
            var entity = new ContactMessage
            {
                Name = vm.Name,
                Email = vm.Email,
                Phone = vm.Phone,
                Subject = vm.Subject,
                Message = vm.Message,
                IpAddress = ip,
                UserAgent = userAgent,
                CreatedOn = DateTime.UtcNow
            };

            _db.ContactMessages.Add(entity);
            await _db.SaveChangesAsync();

            // Notify admin (best-effort)
            if (_emailSender != null)
            {
                try
                {
                    var to = "admin@siegwarteg.com"; // replace or read from configuration
                    var subj = $"New contact: {(entity.Subject ?? "(no subject)")}";
                    var body = $@"New contact message:
Name: {entity.Name}
Email: {entity.Email}
Phone: {entity.Phone}
Subject: {entity.Subject}
Message:
{entity.Message}

IP: {entity.IpAddress}
UserAgent: {entity.UserAgent}
Received: {entity.CreatedOn:u}
";
                    var email = new Website.Siegwart.DAL.Models.Email
                    {
                        To = to,
                        Subject = subj,
                        Body = body,
                        IsHtml = false
                    };

                    await _emailSender.SendEmailAsync(email);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send admin notification for contact message {Id}", entity.Id);
                }
            }

            return entity;
        }

        public async Task<ContactMessage?> GetByIdAsync(int id)
        {
            return await _db.ContactMessages.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task MarkAsReadAsync(int id)
        {
            var msg = await GetByIdAsync(id);
            if (msg == null) return;
            msg.IsRead = true;
            msg.LastModifiedOn = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }
}