using System;
using System.Collections.Generic;

namespace Ziarah.Models;

public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string NormalizedUserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string NormalizedEmail { get; set; } = null!;

    public bool EmailConfirmed { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public int Status { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public int? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual ICollection<AmbulanceRequest> AmbulanceRequests { get; set; } = new List<AmbulanceRequest>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Clinic> Clinics { get; set; } = new List<Clinic>();

    public virtual ICollection<Department1> Department1s { get; set; } = new List<Department1>();

    public virtual ICollection<DoctorClinic> DoctorClinics { get; set; } = new List<DoctorClinic>();

    public virtual ICollection<Doctor> DoctorCreatedByNavigations { get; set; } = new List<Doctor>();

    public virtual ICollection<Doctor> DoctorUsers { get; set; } = new List<Doctor>();

    public virtual ICollection<HomeCareService> HomeCareServices { get; set; } = new List<HomeCareService>();

    public virtual ICollection<Hospital> Hospitals { get; set; } = new List<Hospital>();

    public virtual ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();

    public virtual ICollection<Lab> Labs { get; set; } = new List<Lab>();

    public virtual ICollection<Notification> NotificationCreatedByNavigations { get; set; } = new List<Notification>();

    public virtual ICollection<Notification> NotificationUsers { get; set; } = new List<Notification>();

    public virtual ICollection<Nurse> NurseCreatedByNavigations { get; set; } = new List<Nurse>();

    public virtual ICollection<Nurse> NurseUsers { get; set; } = new List<Nurse>();

    public virtual ICollection<Patient> PatientCreatedByNavigations { get; set; } = new List<Patient>();

    public virtual ICollection<Patient> PatientUsers { get; set; } = new List<Patient>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Pharmacy> Pharmacies { get; set; } = new List<Pharmacy>();

    public virtual ICollection<Radiology> Radiologies { get; set; } = new List<Radiology>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<Specialization> Specializations { get; set; } = new List<Specialization>();
}
