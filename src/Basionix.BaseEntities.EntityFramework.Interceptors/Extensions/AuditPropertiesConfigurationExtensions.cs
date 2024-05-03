namespace Basionix.BaseEntities.EntityFramework.Interceptors;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Constants;

public static class AuditPropertiesConfigurationExtensions
{
    /// <summary>
    /// Extension method that sets the properties for entities that implements the "IAmAuditable" interface
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="entity">The entity builder for the type</param>
    /// <param name="sizeOfUserField">The size of the "user" name/code field. Defaults to 36 characters (length of a regular-formatted System.Guid). Choose this wisely.</param>
    /// <param name="overrideMinimum">This flag makes the method to override the minimum size of 36 characters. Choose this wisely.</param>
    public static void ConfigureAuditProperties<T>(this EntityTypeBuilder<T> entity, int sizeOfUserField = MinimumUserStringLength, bool overrideMinimum = false)
        where T : class, IAmAuditable
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sizeOfUserField, nameof(sizeOfUserField));
        if (!overrideMinimum)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(sizeOfUserField, MinimumUserStringLength, nameof(sizeOfUserField));
        }

        entity
            .Property<DateTimeOffset>(nameof(IAmAuditable.CreatedAt))
            .IsRequired();
        
        entity
            .Property<string>(nameof(IAmAuditable.CreatedBy))
            .HasMaxLength(sizeOfUserField)
            .IsRequired();
        
        entity
            .Property<DateTimeOffset?>(nameof(IAmAuditable.UpdatedAt))
            .IsRequired(false);
            
        entity
            .Property<string?>(nameof(IAmAuditable.LastUpdatedBy))
            .HasMaxLength(sizeOfUserField)
            .IsRequired(false);
    }
}
