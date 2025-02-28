using System.Threading.Tasks;

namespace EcommerceAPI.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}
