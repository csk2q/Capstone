using System;
using System.Collections.Generic;

namespace ServerBee.Data.Models;

public partial class CreditHistory
{
    public int CreditHistoryId { get; set; }

    public int? CustomerId { get; set; }

    public int? CreditScore { get; set; }

    public string? CreditLines { get; set; }

    public string? PastPaymentHistory { get; set; }
}
