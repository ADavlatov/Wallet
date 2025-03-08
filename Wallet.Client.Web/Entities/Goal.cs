namespace Wallet.Client.Web.Entities;

public class Goal
{
    public string? Id { get; set; }
    public string? User { get; set; } // Assuming User can be represented as a string or another model if it's more complex.
    public string? UserId { get; set; }
    public string? Name { get; set; }
    public decimal TargetSum { get; set; }
    public decimal CurrentSum { get; set; }
    public DateTime? Deadline { get; set; } 
    public decimal AmountToAdd { get; set; } = 0;
    
    public int GetPercentage
    {
        get
        {
            if (TargetSum == 0) return 0; 
            return (int)(CurrentSum / TargetSum * 100);
        }
    }
}

public class NewGoal
{
    public string Name { get; set; }
    public decimal TargetSum { get; set; }
    public DateOnly? Deadline { get; set; }
}