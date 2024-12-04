namespace TimeShareProject.Models;

public partial class Rate
{
    public int ProjectId { get; set; }

    public int UserId { get; set; }

    public string? DetailRate { get; set; }

    public int? StarRate { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
