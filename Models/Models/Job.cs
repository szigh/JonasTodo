using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Keyless]
public partial class Job
{
    [Column("ID")]
    public int Id { get; set; }

    [Column(TypeName = "text")]
    public string Company { get; set; } = null!;

    [Column("Job_title", TypeName = "text")]
    public string? JobTitle { get; set; }

    [Column("Date_posted")]
    public DateOnly? DatePosted { get; set; }

    [Column("Date_found")]
    public DateOnly DateFound { get; set; }

    [Column("Date_deadline")]
    public DateOnly? DateDeadline { get; set; }

    [Column("Req_test")]
    public bool? ReqTest { get; set; }

    [Column("Req_cover_letter")]
    public bool? ReqCoverLetter { get; set; }

    [Column("Date_submitted")]
    public DateOnly? DateSubmitted { get; set; }

    [Column("Last_status", TypeName = "text")]
    public string? LastStatus { get; set; }

    [Column("Status_date")]
    public DateOnly? StatusDate { get; set; }
}
