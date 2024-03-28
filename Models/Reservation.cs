using System;
using System.Collections.Generic;

namespace TimeShareProject.Models;

public partial class Reservation
{
    public int Id { get; set; }

    public int? PropertyId { get; set; }

    public int UserId { get; set; }

    public DateTime? RegisterDate { get; set; }

    public int? YearQuantity { get; set; }

    public int? Type { get; set; }

    public int BlockId { get; set; }

    public int? Status { get; set; }

    public int? Order { get; set; }

    public virtual Block Block { get; set; } = null!;

    public virtual Property? Property { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User User { get; set; } = null!;
}
