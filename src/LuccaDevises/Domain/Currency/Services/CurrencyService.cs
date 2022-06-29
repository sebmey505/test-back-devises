namespace LuccaDevises.Domain.Currency.Services;

using LuccaDevises.Domain.Currency.Exceptions;
using LuccaDevises.Domain.Currency.Entities;
using LuccaDevises.Infrastructure;
using System.Collections.Generic;
using System.Linq;

public sealed class CurrencyService : ICurrencyService
{

    private IExchangeRateRepository exchangeRateRepository;

    public CurrencyService(IExchangeRateRepository exchangeRateRepository)
    {
        this.exchangeRateRepository = exchangeRateRepository;
    }

    public int Convert(int amount, IsoCurrency from, IsoCurrency to)
    {
        IEnumerable<ExchangeRate> exchangeRates = exchangeRateRepository.GetAllExchangeRates();

        if (amount <= 0)
            throw new AmountConvertionException($"The amount to convert {amount} is less or equal than 0");
        else if (!exchangeRates.Any())
            throw new AmountConvertionException("No exchange rates were found.");
        else if (!exchangeRates.Where(exchangeRate => exchangeRate.Support(from)).Any())
            throw new AmountConvertionException($"No exchange rate match with source currency {from.Code}");
        else if (!exchangeRates.Where(exchangeRate => exchangeRate.Support(to)).Any())
            throw new AmountConvertionException($"No exchange rate match with destination currency {to.Code}");

        CurrencyConverter converter = new CurrencyConverter(exchangeRates, from, to);

        if (!converter.CanConvert())
            throw new AmountConvertionException($"No convertion path with exchange rates was found to convert an amount from {from.Code} to {to.Code}");

        return System.Convert.ToInt32(Math.Round(converter.Convert(amount),0, MidpointRounding.ToPositiveInfinity));
    }
}
