using System;
using System.Collections.Generic;

namespace TimeShareProject.Models;

public partial class New
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public DateTime? Date { get; set; }

    public int? Type { get; set; }

    public int TransactionId { get; set; }

    public virtual Transaction? Transaction { get; set; }

    public virtual User? User { get; set; }
}
