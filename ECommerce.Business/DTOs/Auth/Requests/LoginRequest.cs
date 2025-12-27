namespace ECommerce.Business.DTOs.Auth.Requests
{
    public class LoginRequest // Input Dto to Login Endpoint
    {
        public string Identifier { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }
}
