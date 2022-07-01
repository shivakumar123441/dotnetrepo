namespace InvestTrackerWebApi.Application.ImageStorage;
using InvestTrackerWebApi.Application.Validation;
using FluentValidation;

public class ImageUploadRequest
{
    public string Name { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public string Data { get; set; } = default!;
}

public class ImageUploadRequestValidator : CustomValidator<ImageUploadRequest>
{
    public ImageUploadRequestValidator()
    {
        _ = this.RuleFor(p => p.Name)
            .NotEmpty()
                .WithMessage("Image Name cannot be empty!")
            .MaximumLength(150);

        _ = this.RuleFor(p => p.Extension)
            .NotEmpty()
                .WithMessage("Image Extension cannot be empty!")
            .MaximumLength(5);

        _ = this.RuleFor(p => p.Data)
            .NotEmpty()
                .WithMessage("Image Data cannot be empty!");
    }
}
