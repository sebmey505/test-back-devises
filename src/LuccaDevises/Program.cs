namespace LuccaDevises;

using LuccaDevises.Domain.Currency.Services;
using LuccaDevises.Infrastructure;
using LuccaDevises.Utils;

class Program
{
    static void Main(string[] args)
    {
        if (args == null || args.Length == 0 || String.IsNullOrWhiteSpace(args[0]))
            throw new ArgumentNullException("the file path isn't specified.");

        ConvertionRequestFile convertionRequest = ConvertionRequestFile.Parse(args[0]);

        ICurrencyService conversionService = new CurrencyService(new ExchangeRateRepository(convertionRequest.ExchangeRates));

        int amountConverted = conversionService.Convert(convertionRequest.Amount, convertionRequest.From, convertionRequest.To);

        Console.WriteLine($"{convertionRequest.Amount} {convertionRequest.From.Code} -> {amountConverted} {convertionRequest.To.Code}");
    }
}
