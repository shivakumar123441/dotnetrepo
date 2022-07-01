namespace InvestTrackerWebApi.Application.Exceptions;
using FluentValidation.Results;

public class ValidationException : Exception
{
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures have occurred.") => this.ErrorMessages = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

    public ValidationException(string message, IDictionary<string, string[]> errors)
        : base(message) => this.ErrorMessages = errors;

    public IDictionary<string, string[]> ErrorMessages { get; }
}
