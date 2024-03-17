using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TimeShareProject.Models;

public partial class Transaction
{
    public int Id { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy | HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime? Date { get; set; }

    public double? Amount { get; set; }

    public bool? Status { get; set; }

    public string? TransactionCode { get; set; }

    public int? ReservationId { get; set; }

    public int? Type { get; set; }

    public virtual Reservation? Reservation { get; set; }
}
