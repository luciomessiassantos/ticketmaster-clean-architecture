using TicketMasterAPI.src.Domain;



/// <summary>
///     Unit Of Work ou Unidade de Trabalho é um padrão de projeto, e, 
///     de acordo com Martin Fowler, o padrão de unidade de trabalho: 
/// 
///     "mantém uma lista de objetos afetados por uma transação e coordena 
///     a escrita de mudanças e trata possíveis problemas de concorrência".
/// </summary>
public interface IUnitOfWork : IDisposable
{

    /// <summary>
    ///     Persiste todas as alterações do DbContext em uma única operação.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Persiste e retorna true se houve alterações > 0.
    /// </summary>
    Task<bool> SaveChangesWithResultAsync(CancellationToken cancellationToken = default);
}