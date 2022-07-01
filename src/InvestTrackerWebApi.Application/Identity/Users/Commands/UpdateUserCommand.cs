namespace InvestTrackerWebApi.Application.Identity.Users;

using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.ImageStorage;
using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class UpdateUserCommand : IRequest
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ImageUploadRequest? Image { get; set; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ICurrentUser currentUser;
    private readonly IImageStorageService imageStorage;

    public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUser currentUser, IImageStorageService imageStorage)
    {
        this.userManager = userManager;
        this.currentUser = currentUser;
        this.imageStorage = imageStorage;
    }

    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await this.userManager.FindByIdAsync(request.Id.ToString());

        _ = user ?? throw new NotFoundException("User Not Found.");

        if (user.Email.ToLowerInvariant() == RootConstants.RootAdminUserEmail.ToLowerInvariant())
        {
            throw new IdentityException("Operation not allowed.");
        }

        string currentImage = user.ImageUrl;
        if (request.Image != null)
        {
            user = user.UpdateImageUrl(await this.imageStorage.UploadAsync<ApplicationUser>(request.Image), this.currentUser.GetUserId());
        }

        user = user.Update(request.FirstName, request.LastName, this.currentUser.GetUserId());

        string phoneNumber = await this.userManager.GetPhoneNumberAsync(user);
        if (request.PhoneNumber != phoneNumber)
        {
            await this.userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
        }

        user.AddDomainEvent(new ApplicationUserUpdatedEvent(user));
        var result = await this.userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new ValidationException(
                "Validation Errors Occurred.",
                result.Errors.GroupBy(e => e.Code, e => e.Description)
                    .ToDictionary(
                    failureGroup =>
                    failureGroup.Key,
                    failureGroup => failureGroup.ToArray()));
        }

        return Unit.Value;
    }
}
