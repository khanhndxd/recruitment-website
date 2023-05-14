using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DoanWebsiteTuyenDung.Models;

[Table("Job_Seeker")]
public partial class JobSeeker
{
    [Key]
    [Column("JS_id")]
    [StringLength(30)]
    [Unicode(false)]
    public string JsId { get; set; } = null!;

    [Column("JS_name")]
    [StringLength(30)]
    public string JsName { get; set; } = null!;

    [Column("JS_email")]
    [StringLength(30)]
    [Unicode(false)]
    public string JsEmail { get; set; } = null!;

    [Column("JS_password")]
    [StringLength(30)]
    [Unicode(false)]
    public string JsPassword { get; set; } = null!;

    [Column("JS_phone")]
    [StringLength(11)]
    [Unicode(false)]
    public string? JsPhone { get; set; }

    [Column("JS_address")]
    [StringLength(100)]
    public string? JsAddress { get; set; }

    [Column("JS_skills")]
    public string? JsSkills { get; set; }

    [Column("JS_expectedSalary", TypeName = "money")]
    public decimal? JsExpectedSalary { get; set; }

    [Column("JS_image")]
    public string? JsImage { get; set; }

    [InverseProperty("Js")]
    public virtual ICollection<Resume> Resumes { get; set; } = new List<Resume>();
}
