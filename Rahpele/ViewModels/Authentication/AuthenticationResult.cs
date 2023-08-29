namespace Rahpele.ViewModels.Authentication
{
    public class AuthenticationResult
    {
        public AuthenticationResult(bool result, string message, string token = "", string refreshToken = "")
        {
            Result = result;
            Message = message;
            Token = token;
            RefreshToken = refreshToken;
        }

        public bool Result { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}