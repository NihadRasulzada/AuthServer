using AuthServer.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthServer.Data.Configurations
{
    /// <summary>
    /// Configures the properties and relationships of the <see cref="Product"/> entity.
    /// </summary>
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        #region Configure Method

        /// <summary>
        /// Configures the <see cref="Product"/> entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity.</param>
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(x => x.Price)
                .HasColumnType("decimal(18,2)");
            builder.Property(x => x.Stock)
                .IsRequired();
            builder.Property(x => x.UserId)
                .IsRequired();
        }

        #endregion
    }
}
