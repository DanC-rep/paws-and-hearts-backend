namespace PawsAndHearts.Accounts.Domain;

public class RefreshSession
{
    public Guid Id { get; init; }
    
    public Guid UserId { get; init; }
    
    public User User { get; init; }
    
    public Guid RefreshToken { get; init; }
    
    public Guid Jti { get; init; }
    
    public DateTime ExpirationDate { get; init; }
    
    public DateTime CreationDate { get; init; }
}