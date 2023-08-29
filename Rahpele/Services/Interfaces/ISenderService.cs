namespace Rahpele.Services.Interfaces
{
    public interface ISenderService
    {
        Task<bool> SendEmailAsync(string email, string subject, string message);
        bool SendSMS(string toNumber, string userName, string verificationCode, int patternStatus);
        Task<bool> SendSMSAsync(string toNumber, string userName, string verificationCode, int patternStatus);
    }
}
