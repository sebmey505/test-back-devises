namespace LuccaDevises.Domain.Currency.Services;

using LuccaDevises.Domain.Currency.Entities;

public interface ICurrencyService
{
    int Convert(int amount, IsoCurrency from, IsoCurrency to);
}