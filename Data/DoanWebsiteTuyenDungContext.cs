using System;
using System.Collections.Generic;
using DoanWebsiteTuyenDung.Models;
using Microsoft.EntityFrameworkCore;

namespace DoanWebsiteTuyenDung.Data;

public partial class DoanWebsiteTuyenDungContext : DbContext
{
    public DoanWebsiteTuyenDungContext()
    {
    }

    public DoanWebsiteTuyenDungContext(DbContextOptions<DoanWebsiteTuyenDungContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Employer> Employers { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<JobSeeker> JobSeekers { get; set; }

    public virtual DbSet<Resume> Resumes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-OR9L1H31\\SQLEXPRESS;Initial Catalog=Doan_WebsiteTuyenDung;Trusted_Connection=yes;TrustServerCertificate=True;User ID=sa;password=123456789");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.AId).HasName("PK__Applicat__71AD61D965051634");

            entity.Property(e => e.AAppliedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.JIdNavigation).WithMany(p => p.Applications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_App");
        });

        modelBuilder.Entity<Employer>(entity =>
        {
            entity.HasKey(e => e.EId).HasName("PK__Employer__D7E94DACAE73398E");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.JId).HasName("PK__Job__92B5B2ABE3BCBB3F");

            entity.Property(e => e.JPostDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.EIdNavigation).WithMany(p => p.Jobs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Job");
        });

        modelBuilder.Entity<JobSeeker>(entity =>
        {
            entity.HasKey(e => e.JsId).HasName("PK__Job_Seek__D295F8456F6C8D3E");
        });

        modelBuilder.Entity<Resume>(entity =>
        {
            entity.HasKey(e => e.RId).HasName("PK__Resume__DE142AC1B5EA5130");

            entity.Property(e => e.RUpdateDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Js).WithMany(p => p.Resumes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resume");

            entity.HasMany(d => d.AIds).WithMany(p => p.RIds)
                .UsingEntity<Dictionary<string, object>>(
                    "IntermediateTable",
                    r => r.HasOne<Application>().WithMany()
                        .HasForeignKey("AId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Intermediate1"),
                    l => l.HasOne<Resume>().WithMany()
                        .HasForeignKey("RId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Intermediate"),
                    j =>
                    {
                        j.HasKey("RId", "AId").HasName("PK__Intermed__590EFCDC9BE67B54");
                        j.ToTable("Intermediate_table");
                        j.IndexerProperty<string>("RId")
                            .HasMaxLength(30)
                            .IsUnicode(false)
                            .HasColumnName("R_id");
                        j.IndexerProperty<string>("AId")
                            .HasMaxLength(30)
                            .IsUnicode(false)
                            .HasColumnName("A_id");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
