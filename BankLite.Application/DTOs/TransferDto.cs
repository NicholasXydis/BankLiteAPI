namespace BankLite.Application.DTOs
{
    public class TransferDto
    {
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
