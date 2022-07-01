namespace InvestTrackerWebApi.Application.FileStorage;
using InvestTrackerWebApi.Application.Validation;
using FluentValidation;
using System.IO;

public class Attachment
{
    public string Name { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public Stream Contents { get; set; } = default!;

    public Attachment(string name, string contentType, Stream contents)
    {
        this.Name = name;
        this.ContentType = contentType;
        this.Contents = contents;
    }
}

public class AttachmentsDTOValidator : CustomValidator<Attachment>
{
    public AttachmentsDTOValidator()
    {
        _ = this.RuleFor(p => p.Name)
            .NotEmpty()
                .WithMessage("File Name cannot be empty!")
            .MaximumLength(150);

        _ = this.RuleFor(p => p.ContentType)
            .NotEmpty()
                .WithMessage("File Extension cannot be empty!")
            .MaximumLength(5);

        _ = this.RuleFor(p => p.Contents)
            .NotEmpty()
                .WithMessage("File Contents cannot be empty!");
    }
}
