using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DoanWebsiteTuyenDung.Models;

[Table("Resume")]
public partial class Resume
{
    [Key]
    [Column("R_id")]
    [StringLength(30)]
    [Unicode(false)]
    public string RId { get; set; } = null!;

    [Column("R_name")]
    [StringLength(30)]
    public string RName { get; set; } = null!;

    [Column("R_updateDate", TypeName = "date")]
    public DateTime RUpdateDate { get; set; }

    [Column("R_content")]
    [StringLength(100)]
    public string RContent { get; set; } = null!;

    [Column("JS_id")]
    [StringLength(30)]
    [Unicode(false)]
    public string JsId { get; set; } = null!;

    [ForeignKey("JsId")]
    [InverseProperty("Resumes")]
    public virtual JobSeeker Js { get; set; } = null!;

    [ForeignKey("RId")]
    [InverseProperty("RIds")]
    public virtual ICollection<Application> AIds { get; set; } = new List<Application>();
}
