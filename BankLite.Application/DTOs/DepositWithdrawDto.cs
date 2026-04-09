namespace BankLite.Application.DTOs
{
    public class DepositWithdrawDto
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
