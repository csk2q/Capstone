using System;
using System.Collections.Generic;

namespace ServerBee.Data.Models;

public partial class CustomerCredential
{
    public int CustomerId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;
}
