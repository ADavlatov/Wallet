namespace Wallet.Client.Web.Entities;

public class Transaction
{
    public string Id { get; set; }
    public string CategoryId { get; set; }
    public string? Name { get; set; }
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);
}