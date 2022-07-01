namespace InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;

using InvestTrackerWebApi.Domain.Common;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser, IAuditableEntity, IHasDomainEvent
{
    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string ReferralEmail { get; private set; } = string.Empty;

    public string ImageUrl { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public string RefreshToken { get; private set; } = string.Empty;

    public DateTime RefreshTokenExpiryTime { get; private set; }

    public Guid CreatedBy { get; private set; }

    public DateTime CreatedOn { get; private set; }

    public Guid LastModifiedBy { get; private set; }

    public DateTime LastModifiedOn { get; private set; }

    public List<DomainEvent> DomainEvents { get; } = new();

    public ApplicationUser(string email, string firstName, string lastName, string referralEmail, string userName, string phoneNumber, Guid currentUserId)
    {
        this.Email = email;
        this.NormalizedEmail = email.ToUpperInvariant();
        this.FirstName = firstName;
        this.LastName = lastName;
        this.ReferralEmail = referralEmail;
        this.UserName = userName;
        this.NormalizedUserName = userName.ToUpperInvariant();
        this.PhoneNumber = phoneNumber;
        this.CreatedOn = DateTime.UtcNow;
        this.CreatedBy = currentUserId;
        this.LastModifiedOn = DateTime.UtcNow;
        this.LastModifiedBy = currentUserId;
        this.IsActive = false;
    }

    public ApplicationUser()
    {
    }

    public void AddDomainEvent(DomainEvent @event) => this.DomainEvents.Add(@event);

    public ApplicationUser Update(string firstName, string lastName, Guid currentUserId)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.LastModifiedOn = DateTime.UtcNow;
        this.LastModifiedBy = currentUserId;
        return this;
    }

    public ApplicationUser UpdateRefreshTokenParameters(string refreshToken, DateTime refreshTokenExpiryTime, Guid currentUserId)
    {
        this.RefreshToken = refreshToken;
        this.RefreshTokenExpiryTime = refreshTokenExpiryTime;
        this.LastModifiedOn = DateTime.UtcNow;
        this.LastModifiedBy = currentUserId;
        return this;
    }

    public ApplicationUser ToggleUserStatus(bool isActive, Guid currentUserId)
    {
        this.IsActive = isActive;
        this.LastModifiedOn = DateTime.UtcNow;
        this.LastModifiedBy = currentUserId;
        return this;
    }

    public ApplicationUser UpdateImageUrl(string imageUrl, Guid currentUserId)
    {
        this.ImageUrl = imageUrl;
        this.LastModifiedOn = DateTime.UtcNow;
        this.LastModifiedBy = currentUserId;
        return this;
    }

    public ApplicationUser ConfirmEmail(Guid currentUserId)
    {
        this.EmailConfirmed = true;
        this.LastModifiedOn = DateTime.UtcNow;
        this.LastModifiedBy = currentUserId;
        return this;
    }

    public ApplicationUser ConfirmPhoneNumber(Guid currentUserId)
    {
        this.PhoneNumberConfirmed = true;
        this.LastModifiedOn = DateTime.UtcNow;
        this.LastModifiedBy = currentUserId;
        return this;
    }
}
