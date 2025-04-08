using System;
using System.Collections.Generic;

namespace ServerBee.Data.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? EmailAddress { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Ssn { get; set; }
}
