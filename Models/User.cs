using System;
using System.Collections.Generic;

namespace TimeShareProject.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? Sex { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? IdfrontImage { get; set; }

    public string? IdbackImage { get; set; }

    public int? AccountId { get; set; }

    public bool? Status { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? BankAccountHolder { get; set; }

    public string? BankName { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<New> News { get; set; } = new List<New>();

    public virtual ICollection<Rate> Rates { get; set; } = new List<Rate>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
