namespace LuccaDevises.Domain.Currency.Entities;

public class ConversionStep
{
    public ExchangeRate ExchangeRate { get; private set; }

    public bool Inverse { get; private set; }

    public ConversionStep? Previous { get; private set; }

    public ConversionStep? Next { get; private set; }

    public IsoCurrency From { get => Inverse ? ExchangeRate.To : ExchangeRate.From; }

    public IsoCurrency To { get => Inverse ? ExchangeRate.From : ExchangeRate.To; }

    public ConversionStep(ExchangeRate exchangeRate, bool inverse = false)
    {
        ExchangeRate = exchangeRate;
        Inverse = inverse;
    }

    public ConversionStep(ConversionStep previous, ExchangeRate exchangeRate, bool inverse = false)
        : this(exchangeRate, inverse)
    {
        Previous = previous;
    }

    public int GetDepth()
    {
        int count = Next != null ? Next.GetDepth() : 0;
        count++;
        return count;
    }

    public bool MatchWithTargetCurrency(IsoCurrency target)
    {
        return To.Equals(target);
    }

    public IEnumerable<ExchangeRate> GetPreviousExchangeRates()
    {
        return (Previous != null ? Previous.GetPreviousExchangeRates() : new List<ExchangeRate>()).Append(ExchangeRate);
    }

    public bool ExistsNextStep(IEnumerable<ExchangeRate> exchangeRates, IsoCurrency target)
    {
        IEnumerable<ExchangeRate> alreadyUsedExchangeRates = GetPreviousExchangeRates();

        Next = exchangeRates
            .Where(exchangeRate => exchangeRate.Support(To) && !alreadyUsedExchangeRates.Contains(exchangeRate))
            .Select(exchangeRate => new ConversionStep(this, exchangeRate, exchangeRate.To.Equals(To)))
            .Where(next => next.MatchWithTargetCurrency(target) || next.ExistsNextStep(exchangeRates, target))
            .OrderBy(next => next.GetDepth())
            .ThenBy(next => next.To.Code)
            .ThenBy(next => next.Inverse)
            .FirstOrDefault();

        return Next != null;
    }

    public double Convert(double amount)
    {
        amount = Math.Round(Inverse ? amount * Math.Round(1 / ExchangeRate.Rate, 4) : amount * ExchangeRate.Rate, 4);
        Console.WriteLine($"{From.Code} -> {To.Code} : {amount}");
        return Next != null ? Next.Convert(amount) : amount;
    }
}
