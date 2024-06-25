using AuthServer.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Data.Contexts
{
    /// <summary>
    /// The application's database context, integrating Identity for user management.
    /// </summary>
    public class AppDbContext : IdentityDbContext<UserApp, IdentityRole, string>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        #endregion

        #region DbSets

        /// <summary>
        /// Gets or sets the products.
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Gets or sets the user refresh tokens.
        /// </summary>
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        #endregion

        #region ModelCreating

        /// <summary>
        /// Configures the schema needed for the identity framework.
        /// </summary>
        /// <param name="builder">The <see cref="ModelBuilder"/> being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            base.OnModelCreating(builder);
        }

        #endregion
    }
}
