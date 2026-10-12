using TicketMaster.Domain.Shared;

namespace TicketMaster.Application.Common.Models;

/// <summary>
///     Classe utilitária que centraliza o mapeamento de erros de validação por campo
/// </summary>
/// <param name="errors"></param>
public class ValidationError(IDictionary<string, string[]> errors) : Error(
    nameof(ValidationError), 
    ErrorType.ValidationError, 
    "Validation.Error", 
    "Um ou mais erros de validação ocorreram."
)
{
    public IDictionary<string, string[]> Errors { get; } = errors;

    public static ValidationError Create(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
    {
        var errorsDictionary = failures
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                failureGroup => failureGroup.Key,
                failureGroup => failureGroup.Select(e => e.ErrorMessage).ToArray()
            );

        return new ValidationError(errorsDictionary);
    }
}