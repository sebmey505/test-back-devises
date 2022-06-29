namespace LuccaDevises.Domain.Currency.Exceptions;

using System;

public class AmountConvertionException : Exception
{
    public AmountConvertionException() { }

    public AmountConvertionException(string message) : base(message) { }

    public AmountConvertionException(string message, Exception inner) : base(message, inner) { }

}