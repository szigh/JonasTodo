using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Keyless]
public partial class Topic
{
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(10)]
    public string? Description { get; set; }

    public int? Priority { get; set; }

    [Column("Date_Logged")]
    public DateOnly? DateLogged { get; set; }

    [Column("Long_Descriptions", TypeName = "text")]
    public string? LongDescriptions { get; set; }
}
