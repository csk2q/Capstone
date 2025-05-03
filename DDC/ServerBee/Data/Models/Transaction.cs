using System;
using System.Collections.Generic;

namespace ServerBee.Data.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int? AccountId { get; set; }

    public DateOnly? Date { get; set; }

    public TimeOnly? Time { get; set; }

    public decimal? Amount { get; set; }

    public string? TransactionType { get; set; }

    public string? PayeePayerName { get; set; }

    public string? PayeePayerAccountNumber { get; set; }

    public string? Memo { get; set; }
}
