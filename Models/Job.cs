using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DoanWebsiteTuyenDung.Models;

[Table("Job")]
public partial class Job
{
    [Key]
    [Column("J_id")]
    [StringLength(30)]
    [Unicode(false)]
    public string JId { get; set; } = null!;

    [Column("J_title")]
    [StringLength(100)]
    public string JTitle { get; set; } = null!;

    [Column("J_description")]
    [StringLength(100)]
    public string JDescription { get; set; } = null!;

    [Column("J_requiredSkills")]
    [StringLength(50)]
    public string JRequiredSkills { get; set; } = null!;

    [Column("J_salary", TypeName = "money")]
    public decimal? JSalary { get; set; }

    [Column("J_postDate", TypeName = "date")]
    public DateTime JPostDate { get; set; }

    [Column("J_expirationDate", TypeName = "date")]
    public DateTime JExpirationDate { get; set; }

    [Column("J_status")]
    public int JStatus { get; set; }

    [Column("E_id")]
    [StringLength(30)]
    [Unicode(false)]
    public string EId { get; set; } = null!;

    [InverseProperty("JIdNavigation")]
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    [ForeignKey("EId")]
    [InverseProperty("Jobs")]
    public virtual Employer EIdNavigation { get; set; } = null!;
}
