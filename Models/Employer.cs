﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DoanWebsiteTuyenDung.Models;

[Table("Employer")]
public partial class Employer
{
    [Key]
    [Column("E_id")]
    [StringLength(30)]
    [Unicode(false)]
    public string EId { get; set; } = null!;

    [Column("E_name")]
    [StringLength(30)]
    [DisplayName("Tên nhà tuyển dụng")]
    public string EName { get; set; } = null!;

    [Column("E_email")]
    [StringLength(30)]
	[DisplayName("Email")]
	public string EEmail { get; set; } = null!;

    [Column("E_password")]
    [StringLength(30)]
    [Unicode(false)]
	[DisplayName("Mật khẩu")]
	public string EPassword { get; set; } = null!;

    [Column("E_contactPerson")]
    [StringLength(30)]
    public string? EContactPerson { get; set; }

    [Column("E_phone")]
    [StringLength(11)]
    [Unicode(false)]
    public string? EPhone { get; set; }

    [Column("E_address")]
    [StringLength(100)]
    public string? EAddress { get; set; }

    [Column("E_about")]
    [StringLength(100)]
    public string? EAbout { get; set; }

    [InverseProperty("EIdNavigation")]
    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
