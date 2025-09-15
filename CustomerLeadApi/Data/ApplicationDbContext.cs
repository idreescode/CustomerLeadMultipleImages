using CustomerLeadApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerLeadApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactImage> ContactImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationship between Contact and ContactImage
            modelBuilder.Entity<Contact>()
                .HasMany(c => c.Images)
                .WithOne(i => i.Contact)
                .HasForeignKey(i => i.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add any additional configurations here
        }
    }
}
