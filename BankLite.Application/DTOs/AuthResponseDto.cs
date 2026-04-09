namespace BankLite.Application.DTOs
{
    public class AuthResponseDto
    {
        public required string Token { get; set; }
        public Guid UserId { get; set; }
    }
}
