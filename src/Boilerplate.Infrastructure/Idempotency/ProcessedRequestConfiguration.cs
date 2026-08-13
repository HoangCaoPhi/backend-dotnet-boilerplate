using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Boilerplate.Infrastructure.Idempotency;

public sealed class ProcessedRequestConfiguration : IEntityTypeConfiguration<ProcessedRequest>
{
    public void Configure(EntityTypeBuilder<ProcessedRequest> builder)
    {
        builder.Property(request => request.CommandName)
            .HasMaxLength(200)
            .IsRequired();
    }
}
