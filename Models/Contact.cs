using System;
using System.Collections.Generic;

namespace TimeShareProject.Models;

public partial class Contact
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Message { get; set; } = null!;

    public bool Status { get; set; }

    public string Phone { get; set; } = null!;
}
