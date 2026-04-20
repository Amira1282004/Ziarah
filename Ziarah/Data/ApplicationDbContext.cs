using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Ziarah.Models;

namespace Ziarah.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AmbulanceRequest> AmbulanceRequests { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Clinic> Clinics { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Department1> Departments1 { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorClinic> DoctorClinics { get; set; }

    public virtual DbSet<HomeCareService> HomeCareServices { get; set; }

    public virtual DbSet<Hospital> Hospitals { get; set; }

    public virtual DbSet<Insurance> Insurances { get; set; }

    public virtual DbSet<Lab> Labs { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Nurse> Nurses { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Pharmacy> Pharmacies { get; set; }

    public virtual DbSet<Radiology> Radiologies { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Ziarah;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AmbulanceRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ambulanc__3214EC07FDC55D0B");

            entity.ToTable("AmbulanceRequests", "Ziarah_schema");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.RequestTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue(1);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AmbulanceRequests)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AmbulanceRequests_CreatedBy");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Appointm__3214EC0768FE697E");

            entity.ToTable("Appointments", "Ziarah_schema");

            entity.HasIndex(e => e.AppointmentDate, "IX_Appointments_Date");

            entity.HasIndex(e => e.DoctorId, "IX_Appointments_DoctorId");

            entity.HasIndex(e => e.PatientId, "IX_Appointments_PatientId");

            entity.HasIndex(e => e.Status, "IX_Appointments_Status");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);
            entity.Property(e => e.Status).HasDefaultValue(1);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_CreatedBy");
        });

        modelBuilder.Entity<Clinic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clinics__3214EC075555E5BA");

            entity.ToTable("Clinics", "Ziarah_schema");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Building).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Street).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Clinics)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clinics_CreatedBy");
        });

        modelBuilder.Entity<Department1>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC079C0F2581");

            entity.ToTable("Departments", "Ziarah_schema");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DepartmentName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Department1s)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Departments_CreatedBy");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Doctors__3214EC07071F1EDF");

            entity.ToTable("Doctors", "Ziarah_schema");

            entity.HasIndex(e => e.Rating, "IX_Doctors_Rating");

            entity.HasIndex(e => e.SpecializationId, "IX_Doctors_SpecializationId");

            entity.Property(e => e.ConsultationPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);
            entity.Property(e => e.Rating).HasColumnType("decimal(3, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DoctorCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Doctors_CreatedBy");

            entity.HasOne(d => d.Specialization).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.SpecializationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Doctors_Specializations");

            entity.HasOne(d => d.User).WithMany(p => p.DoctorUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Doctors_Users");
        });

        modelBuilder.Entity<DoctorClinic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DoctorCl__3214EC07053C2733");

            entity.ToTable("DoctorClinics", "Ziarah_schema");

            entity.HasIndex(e => new { e.DoctorId, e.ClinicId }, "UQ_DoctorClinics_DoctorClinic").IsUnique();

            entity.Property(e => e.ConsultationPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DoctorClinics)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DoctorClinics_CreatedBy");
        });

        modelBuilder.Entity<HomeCareService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HomeCare__3214EC07E52FA273");

            entity.ToTable("HomeCareServices", "Ziarah_schema");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);
            entity.Property(e => e.RequestDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ServiceLocation).HasMaxLength(500);
            entity.Property(e => e.Status).HasDefaultValue(1);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.HomeCareServices)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HomeCareServices_CreatedBy");
        });

        modelBuilder.Entity<Hospital>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Hospital__3214EC072DA95890");

            entity.ToTable("Hospital", "Ziarah_schema");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.HospitalName).HasMaxLength(200);
            entity.Property(e => e.Hotline).HasMaxLength(50);
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);
            entity.Property(e => e.Location).HasMaxLength(500);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Hospitals)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Hospital_CreatedBy");

            entity.HasOne(d => d.Insurance).WithMany(p => p.Hospitals)
                .HasForeignKey(d => d.InsuranceId)
                .HasConstraintName("FK_Hospital_Insurance");
        });

        modelBuilder.Entity<Insurance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Insuranc__3214EC07E5164143");

            entity.ToTable("Insurance", "Ziarah_schema");

            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);
            entity.Property(e => e.PolicyNumberFormat).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Insurances)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Insurance_CreatedBy");
        });

        modelBuilder.Entity<Lab>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lab__3214EC078358B50B");

            entity.ToTable("Lab", "Ziarah_schema");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LabName).HasMaxLength(200);
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Labs)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lab_CreatedBy");

            entity.HasOne(d => d.Insurance).WithMany(p => p.Labs)
                .HasForeignKey(d => d.InsuranceId)
                .HasConstraintName("FK_Lab_Insurance");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC0748FF7BEE");

            entity.ToTable("Notifications", "Ziarah_schema");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.NotificationCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_CreatedBy");

            entity.HasOne(d => d.User).WithMany(p => p.NotificationUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_Users");
        });

        modelBuilder.Entity<Nurse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Nurses__3214EC072EDA09D6");

            entity.ToTable("Nurses", "Ziarah_schema");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Photo).HasMaxLength(500);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.NurseCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Nurses_CreatedBy");

            entity.HasOne(d => d.User).WithMany(p => p.NurseUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Nurses_User");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Patients__3214EC070EC87AB7");

            entity.ToTable("Patients", "Ziarah_schema");

            entity.Property(e => e.BloodType).HasMaxLength(10);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Height).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PatientCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patients_CreatedBy");

            entity.HasOne(d => d.Insurance).WithMany(p => p.Patients)
                .HasForeignKey(d => d.InsuranceId)
                .HasConstraintName("FK_Patients_Insurance");

            entity.HasOne(d => d.User).WithMany(p => p.PatientUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patients_Users");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC0789121149");

            entity.ToTable("Payments", "Ziarah_schema");

            entity.HasIndex(e => e.AppointmentId, "IX_Payments_AppointmentId");

            entity.HasIndex(e => e.Status, "IX_Payments_Status");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);
            entity.Property(e => e.TransactionReference).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_CreatedBy");
        });

        modelBuilder.Entity<Pharmacy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pharmacy__3214EC0751EFF8E0");

            entity.ToTable("Pharmacy", "Ziarah_schema");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.PharmacyName).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Pharmacies)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pharmacy_CreatedBy");

            entity.HasOne(d => d.Insurance).WithMany(p => p.Pharmacies)
                .HasForeignKey(d => d.InsuranceId)
                .HasConstraintName("FK_Pharmacy_Insurance");
        });

        modelBuilder.Entity<Radiology>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Radiolog__3214EC07033EB5EE");

            entity.ToTable("Radiology", "Ziarah_schema");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.NameRadiology).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Radiologies)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Radiology_CreatedBy");

            entity.HasOne(d => d.Insurance).WithMany(p => p.Radiologies)
                .HasForeignKey(d => d.InsuranceId)
                .HasConstraintName("FK_Radiology_Insurance");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reviews__3214EC07D5B891D5");

            entity.ToTable("Reviews", "Ziarah_schema");

            entity.HasIndex(e => e.AppointmentId, "UQ_Reviews_Appointment").IsUnique();

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_CreatedBy");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC07952A5046");

            entity.ToTable("Schedules", "Ziarah_schema");

            entity.HasIndex(e => e.ClinicId, "IX_Schedules_ClinicId");

            entity.HasIndex(e => e.DoctorId, "IX_Schedules_DoctorId");

            entity.HasIndex(e => new { e.DoctorId, e.ClinicId, e.DayOfWeek }, "UQ_Schedules_DoctorClinicDay").IsUnique();

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);
            entity.Property(e => e.SlotDuration).HasDefaultValue(30);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedules_CreatedBy");
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Speciali__3214EC079A212331");

            entity.ToTable("Specializations", "Ziarah_schema");

            entity.HasIndex(e => e.Name, "UQ__Speciali__737584F6A5592D03").IsUnique();

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.LastModifiedBy).HasMaxLength(450);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Specializations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Specializations_CreatedBy");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC073D7965AE");

            entity.ToTable("Users", "Ziarah_schema");

            entity.HasIndex(e => e.NormalizedEmail, "UQ__Users__368B291A81916B42").IsUnique();

            entity.HasIndex(e => e.NormalizedUserName, "UQ__Users__54E8BE2210F254D0").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534C0B92196").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__Users__C9F28456C48C369D").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.FullName)
                .HasMaxLength(201)
                .HasComputedColumnSql("(([FirstName]+N' ')+[LastName])", true);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.LockoutEnabled).HasDefaultValue(true);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
