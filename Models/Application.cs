using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DoanWebsiteTuyenDung.Models;

[Table("Application")]
public partial class Application
{
    [Key]
    [Column("A_id")]
    [StringLength(30)]
    [Unicode(false)]
    public string AId { get; set; } = null!;

    [Column("A_appliedDate", TypeName = "date")]
    public DateTime AAppliedDate { get; set; }

    [Column("A_status")]
    public int AStatus { get; set; }

    [Column("A_feedBack")]
    [StringLength(100)]
    public string? AFeedBack { get; set; }

    [Column("J_id")]
    [StringLength(30)]
    [Unicode(false)]
    public string JId { get; set; } = null!;

    [ForeignKey("JId")]
    [InverseProperty("Applications")]
    public virtual Job JIdNavigation { get; set; } = null!;

    [ForeignKey("AId")]
    [InverseProperty("AIds")]
    public virtual ICollection<Resume> RIds { get; set; } = new List<Resume>();
}
