namespace LuccaDevises.Domain.Currency.Entities;

public record ExchangeRate(IsoCurrency From, IsoCurrency To, double Rate)
{
    public bool Support(IsoCurrency currency)
    {
        return From.Equals(currency) || To.Equals(currency);
    }
}