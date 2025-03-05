namespace Wallet.Client.Web.Entities;

public class Transaction
{
    public string CategoryId { get; set; }
    public string? Name { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}