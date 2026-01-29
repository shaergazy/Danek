using Danek.BLL.Services.Contracts;

namespace Danek.BLL.Services
{
    public class EmailService : IEmailService
    {
        public Task SendEmailConfirmationAsync(string email, string callbackUrl)
        {
            throw new NotImplementedException();
        }

        public Task SendPasswordResetAsync(string email, string callbackUrl)
        {
            throw new NotImplementedException();
        }
    }
}
