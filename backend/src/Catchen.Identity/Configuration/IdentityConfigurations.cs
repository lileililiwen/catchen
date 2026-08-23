using Catchen.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catchen.Identity.Configuration;

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).HasMaxLength(320).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.PhoneE164).HasMaxLength(16);
        builder.HasIndex(u => u.PhoneE164).IsUnique().HasFilter("PhoneE164 IS NOT NULL");
        builder.Property(u => u.PasswordHash).HasMaxLength(512).IsRequired();
        builder.Property(u => u.Role).HasMaxLength(32).IsRequired();
    }
}

public sealed class AgreementAcceptanceConfiguration : IEntityTypeConfiguration<AgreementAcceptance>
{
    public void Configure(EntityTypeBuilder<AgreementAcceptance> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.AgreementVersion).HasMaxLength(64).IsRequired();
        builder.Property(a => a.ClientIpHash).HasMaxLength(64).IsRequired();
        builder.Property(a => a.ClientUserAgent).HasMaxLength(128).IsRequired();
        builder.HasIndex(a => new { a.UserId, a.AgreementVersion }).IsUnique();
    }
}

public sealed class AuditEventConfiguration : IEntityTypeConfiguration<AuditEvent>
{
    public void Configure(EntityTypeBuilder<AuditEvent> builder)
    {
        builder.ToTable("AuditEvents");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Category).HasMaxLength(64).IsRequired();
        builder.Property(e => e.Action).HasMaxLength(128).IsRequired();
        builder.Property(e => e.SubjectType).HasMaxLength(64).IsRequired();
        builder.Property(e => e.SubjectId).HasMaxLength(128).IsRequired();
        builder.Property(e => e.PayloadJson).IsRequired();
        builder.HasIndex(e => e.OccurredAtUtc);
    }
}

public sealed class ApprovedChannelConfiguration : IEntityTypeConfiguration<ApprovedChannel>
{
    public void Configure(EntityTypeBuilder<ApprovedChannel> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Channel).HasMaxLength(64).IsRequired();
        builder.Property(c => c.Kind).HasMaxLength(16).IsRequired();
        builder.HasIndex(c => new { c.Channel, c.Kind }).IsUnique();
    }
}
