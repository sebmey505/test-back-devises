namespace LuccaDevises.Infrastructure;

using LuccaDevises.Domain.Currency.Entities;

public class ExchangeRateRepository : IExchangeRateRepository
{

    private IEnumerable<ExchangeRate> exchangeRates;

    public ExchangeRateRepository(IEnumerable<ExchangeRate> exchangeRates)
    {
        this.exchangeRates = exchangeRates;
    }

    public IEnumerable<ExchangeRate> GetAllExchangeRates()
    {
        return exchangeRates;
    }

}