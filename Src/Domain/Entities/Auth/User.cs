using Backend.Src.Domain.ValueObjects.Payments;

namespace Backend.Src.Domain.Entities.Auth;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Email { get; private set; } = string.Empty;
    public bool IsEmailVerified { get; private set; } = false;
    public string? Password { get; private set; } 
    public bool IsActive { get; private set; } = false;
    public int CreditBalance { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public User() {}

    public User(
        string email,
        bool isEmailVerified,
        string? passwordHash
    )
    {
        Id = Guid.NewGuid();
        Email = email;
        IsEmailVerified = isEmailVerified;
        Password = passwordHash;
        IsActive = true;
        CreditBalance = CreditPlanCatalog.Get(CreditPlan.Free).Credits;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddCredits(int credits)
    {
        CreditBalance += credits;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DeductCredits(int credits)
    {
        CreditBalance -= credits;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string passwordHash)
    {
        Password = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeEmail(string email)
    {
        Email = email;
        IsEmailVerified = false;
        UpdatedAt = DateTime.UtcNow;
    }

}
