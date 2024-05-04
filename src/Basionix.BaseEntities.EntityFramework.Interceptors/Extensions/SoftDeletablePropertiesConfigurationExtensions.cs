namespace Basionix.BaseEntities.EntityFramework.Interceptors.Extensions
{
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using static Extensions.Constants;

    public static class SoftDeletablePropertiesConfigurationExtensions
    {
        /// <summary>
        /// Extension method that sets the properties for entities that implements the "IAmSoftDeletable" interface
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <param name="entity">The entity builder for the type</param>
        /// <param name="sizeOfUserField">The size of the "user" name/code field. Defaults to 36 characters (length of a regular-formatted System.Guid). Choose this wisely.</param>
        /// <param name="overrideMinimum">This flag makes the method to override the minimum size of 36 characters. Choose this wisely.</param>
        public static void ConfigureSoftDeletableProperties<T>(this EntityTypeBuilder<T> entity, int sizeOfUserField = MinimumUserStringLength, bool overrideMinimum = false)
            where T : class, IAmSoftDeletable
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sizeOfUserField, nameof(sizeOfUserField));
            if (!overrideMinimum)
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(sizeOfUserField, MinimumUserStringLength, nameof(sizeOfUserField));
            }

            entity
                .Property<DateTimeOffset?>(nameof(IAmSoftDeletable.DeletedAt))
                .IsRequired(false);
        
            entity
                .Property<string?>(nameof(IAmSoftDeletable.DeletedBy))
                .HasMaxLength(sizeOfUserField)
                .IsRequired(false);
        
            entity
                .Property<bool>(nameof(IAmSoftDeletable.IsDeleted))
                .IsRequired();
        }
    }
}
