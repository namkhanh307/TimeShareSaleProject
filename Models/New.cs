using System;
using System.Collections.Generic;

namespace TimeShareProject.Models;

public partial class New
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime Date { get; set; }

    public virtual User User { get; set; } = null!;
}
