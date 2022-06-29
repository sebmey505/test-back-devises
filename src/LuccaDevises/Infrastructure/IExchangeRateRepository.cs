namespace LuccaDevises.Infrastructure;

using LuccaDevises.Domain.Currency.Entities;

public interface IExchangeRateRepository
{
    IEnumerable<ExchangeRate> GetAllExchangeRates();
}