using Boilerplate.Domain.TodoLists;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Boilerplate.Infrastructure.Persistence.Configurations;

public sealed class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.Property(item => item.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(item => item.Note)
            .HasMaxLength(2000);
    }
}
