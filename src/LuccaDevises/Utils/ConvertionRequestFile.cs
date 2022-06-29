namespace LuccaDevises.Utils;

using LuccaDevises.Domain.Currency.Entities;
using System.Text.RegularExpressions;
using System.Globalization;

public class ConvertionRequestFile
{

    public readonly static string ARGUMENTS_LINE_PATTERN = "^([A-Z]{3});([0-9]+);([A-Z]{3})$";

    public readonly static string EXCHANGE_RATE_COUNT_LINE_PATTERN = "^([0-9]+)$";

    public readonly static string EXCHANGE_RATE_LINE_PATTERN = "^([A-Z]{3});([A-Z]{3});([0-9]+[.][0-9]{4})$";

    public IsoCurrency From { get; private set; }

    public IsoCurrency To { get; private set; }

    public int Amount { get; private set; }

    public IEnumerable<ExchangeRate> ExchangeRates { get; private set; }

    private ConvertionRequestFile(IsoCurrency from, int amount, IsoCurrency to, IEnumerable<ExchangeRate> exchangeRates)
    {
        From = from;
        Amount = amount;
        To = to;
        ExchangeRates = exchangeRates;
    }

    public static ConvertionRequestFile Parse(string filePath)
    {
        if (!File.Exists(filePath))
            throw new ArgumentException($"the file path specified [{filePath}] doesn't exist.");

        IEnumerable<string> lines = File.ReadAllLines(filePath);
        if (lines.Count() < 3)
            throw new ArgumentException($"the line count of file [{filePath}] is less than 3.");

        Tuple<IsoCurrency, int, IsoCurrency> arguments = lines.Take(1).Select(line => ParseArgumentsLine(line.Trim())).First();

        int count = lines.Skip(1).Take(1).Select(line => ParseExchangeRateCountLine(line)).First();
        if (count == 0)
            throw new ArgumentException($"the specified count of exchange rates in file is equal to 0");

        IEnumerable<string> exchangeRateLines = lines.Skip(2);
        if (!exchangeRateLines.Any())
            throw new ArgumentException($"the file contains no exchange rate");
        if (exchangeRateLines.Count() != count)
            throw new ArgumentException($"the count of exchange rates {exchangeRateLines.Count()} in file is not equal to the specified count {count}");

        IEnumerable<ExchangeRate> exchangeRates = exchangeRateLines.Select(line => ParseExchangeRate(line.Trim()));

        if (exchangeRates.GroupBy(exchangeRate => exchangeRate.From.Code + exchangeRate.To.Code).Any(group => group.Count() > 1))
            throw new ArgumentException("there are one or more duplicate exchange rates in the file");

        return new ConvertionRequestFile(arguments.Item1, arguments.Item2, arguments.Item3, exchangeRates);
    }

    private static Tuple<IsoCurrency, int, IsoCurrency> ParseArgumentsLine(string line)
    {
        Match match = Regex.Match(line, ARGUMENTS_LINE_PATTERN);
        if (!match.Success)
            throw new ArgumentException($"the line [{line}] with arguments is invalid -> doesn't match regex {ARGUMENTS_LINE_PATTERN}");

        IsoCurrency? from = IsoCurrencyResolver.GetIsoCurrency(match.Groups[1].Value);
        if (from == null)
            throw new ArgumentException($"the source currency [{match.Groups[1].Value}] is unknown");

        int amount = int.Parse(match.Groups[2].Value);
        if (amount == 0)
            throw new ArgumentException($"the amount to convert is equal to 0.");

        IsoCurrency? to = IsoCurrencyResolver.GetIsoCurrency(match.Groups[3].Value);
        if (to == null)
            throw new ArgumentException($"the destination currency [{match.Groups[3].Value}] is unknown");

        return new Tuple<IsoCurrency, int, IsoCurrency>(from, amount, to);
    }

    private static int ParseExchangeRateCountLine(string line)
    {
        if (!Regex.IsMatch(line, EXCHANGE_RATE_COUNT_LINE_PATTERN))
            throw new ArgumentException($"the line [{line}] with exchange rate count is invalid -> doesn't match regex {EXCHANGE_RATE_COUNT_LINE_PATTERN}");

        return int.Parse(line);
    }

    private static ExchangeRate ParseExchangeRate(string line)
    {
        Match match = Regex.Match(line, EXCHANGE_RATE_LINE_PATTERN);
        if (!match.Success)
            throw new ArgumentException($"the exchange rate line [{line}] is invalid -> doesn't match regex {EXCHANGE_RATE_LINE_PATTERN}");

        IsoCurrency? from = IsoCurrencyResolver.GetIsoCurrency(match.Groups[1].Value);
        if (from == null)
            throw new ArgumentException($"the source currency [{match.Groups[1].Value}] of exchange rate [{line}] is unknown");

        IsoCurrency? to = IsoCurrencyResolver.GetIsoCurrency(match.Groups[2].Value);
        if (to == null)
            throw new ArgumentException($"the destination currency [{match.Groups[2].Value}] of  exchange rate [{line}] is unknown");

        double rate = double.Parse(match.Groups[3].Value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator));
        if (rate == 0)
            throw new ArgumentException($"the rate of  exchange rate [{line}] is equal to 0.");

        return new ExchangeRate(from, to, rate);
    }
}