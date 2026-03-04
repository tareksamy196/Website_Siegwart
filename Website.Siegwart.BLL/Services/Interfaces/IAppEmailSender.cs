

namespace Website.Siegwart.BLL.Services.Interfaces
{
    public interface IAppEmailSender
    {
        Task SendEmailAsync(Email email);
    }
}