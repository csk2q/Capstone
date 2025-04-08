using System;
using System.Collections.Generic;

namespace ServerBee.Data.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string AccountNumber { get; set; } = null!;

    public string? AccountType { get; set; }

    public decimal? OpeningBalance { get; set; }

    public decimal? CurrentBalance { get; set; }

    public int? CustomerId { get; set; }
}
