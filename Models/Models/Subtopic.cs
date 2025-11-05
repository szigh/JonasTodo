using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Keyless]
public partial class Subtopic
{
    [Column("ID")]
    public int Id { get; set; }

    [Column("Topic_ID")]
    public int? TopicId { get; set; }

    [StringLength(10)]
    public string? Description { get; set; }

    [Column("Logged_Date")]
    public DateOnly? LoggedDate { get; set; }

    [Column("Estimated_hours")]
    public int? EstimatedHours { get; set; }

    public bool? Pluralsight { get; set; }

    [Column("Long_description", TypeName = "text")]
    public string? LongDescription { get; set; }

    public bool? Completed { get; set; }

    public int? Priority { get; set; }
}
