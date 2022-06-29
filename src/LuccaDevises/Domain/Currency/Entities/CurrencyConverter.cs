namespace LuccaDevises.Domain.Currency.Entities;

public class CurrencyConverter
{
    public IsoCurrency From { get; private set; }

    public IsoCurrency To { get; private set; }

    private ConversionStep? conversionPath;


    public CurrencyConverter(IEnumerable<ExchangeRate> exchangeRates, IsoCurrency from, IsoCurrency to)
    {
        From = from;
        To = to;
        conversionPath = SearchConversionPath(exchangeRates);
    }

    private ConversionStep? SearchConversionPath(IEnumerable<ExchangeRate> exchangeRates)
    {
        return exchangeRates.Where(exchangeRate => exchangeRate.Support(From))
            .Select(exchangeRate => new ConversionStep(exchangeRate, exchangeRate.To.Equals(From)))
            .Where(path => path.MatchWithTargetCurrency(To) || path.ExistsNextStep(exchangeRates, To))
            .OrderBy(path => path.GetDepth())
            .ThenBy(path => path.To.Code)
            .ThenBy(path => path.Inverse)
            .FirstOrDefault();
    }

    public bool CanConvert()
    {
        return conversionPath != null;
    }

    public double Convert(double amount)
    {
        return conversionPath != null ? conversionPath.Convert(amount) : -1;
    }

}
