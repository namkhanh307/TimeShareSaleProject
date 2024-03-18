using System;
using System.Collections.Generic;

namespace TimeShareProject.Models;

public partial class News
{
    public int Id { get; set; }

    public DateTime? PublishedDate { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }
}
