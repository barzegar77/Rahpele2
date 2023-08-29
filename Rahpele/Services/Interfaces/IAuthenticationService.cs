
using Rahpele.ViewModels.Authentication;

namespace Rahpele.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> RegisterAsync(RegisterViewModel model);
        AuthenticationResult ConfrimPhoneNumber(PhoneNumberConfirmationViewModel model);
        AuthenticationResult ResendPhoneNumberVerificationCode(ResendPhoneNumberVerificationCodeViewModel model);
        AuthenticationResult ForgotPassword(ForgotPasswordViewModel model);
        AuthenticationResult ResetPassword(ResetPasswordViewModel model);
        AuthenticationResult ResendForgotPasswordVerificationCode(ResendPhoneNumberVerificationCodeViewModel model);
    }
}