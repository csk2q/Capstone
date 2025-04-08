using System;
using System.Collections.Generic;

namespace ServerBee.Data.Models;

public partial class Loan
{
    public int LoanId { get; set; }

    public int? CustomerId { get; set; }

    public decimal? LoanAmount { get; set; }

    public decimal? InterestRate { get; set; }

    public string? RepaymentSchedule { get; set; }

    public string? CollateralDetails { get; set; }
}
