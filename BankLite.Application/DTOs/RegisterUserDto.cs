namespace BankLite.Application.DTOs
{
    public class RegisterUserDto
    {
        public required string FullName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
    }
}
