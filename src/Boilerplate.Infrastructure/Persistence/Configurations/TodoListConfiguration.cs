using Boilerplate.Domain.TodoLists;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Boilerplate.Infrastructure.Persistence.Configurations;

public sealed class TodoListConfiguration : IEntityTypeConfiguration<TodoList>
{
    public void Configure(EntityTypeBuilder<TodoList> builder)
    {
        builder.Ignore(list => list.DomainEvents);

        builder.Property(list => list.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.OwnsOne(
            list => list.Colour,
            colour => colour.Property(c => c.Code)
                .HasColumnName(nameof(Colour))
                .HasMaxLength(7)
                .IsRequired());

        builder.HasMany(list => list.Items)
            .WithOne()
            .HasForeignKey(item => item.TodoListId)
            .IsRequired();
    }
}
