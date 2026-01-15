namespace ECommerce.Business.DTOs.RefreshTokens.Responses
{
    public class LoginSessionDto
    {
        public int Id { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Created { get; set; }
        public bool IsActive { get; set; }

    }
}
