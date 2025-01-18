namespace BankAccount.Core.Exceptions;

public class InvalidTransactionException(string message) : Exception(message);