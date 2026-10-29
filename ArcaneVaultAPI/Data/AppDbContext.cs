// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using System.Security.Cryptography;
using ArcaneVaultAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ArcaneVaultAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ArcaneVaultUserRole> ArcaneVaultUserRoles { get; set; } = null!;
        public DbSet<ArcaneVaultUser> ArcaneVaultUsers { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<CollectionItem> CollectionItems { get; set; } = null!;
        public DbSet<CollectionItemCategory> CollectionItemCategories { get; set; } = null!;
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;
        public DbSet<LoginAttempt> LoginAttempts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite primary key for CollectionItemCategories
            modelBuilder.Entity<CollectionItemCategory>()
                .HasKey(x => new { x.ItemId, x.CategoryCode });

            // Relationships
            modelBuilder.Entity<CollectionItemCategory>()
                .HasOne(cic => cic.CollectionItem)
                .WithMany(ci => ci.CollectionItemCategories)
                .HasForeignKey(cic => cic.ItemId);

            modelBuilder.Entity<CollectionItemCategory>()
                .HasOne(cic => cic.Category)
                .WithMany(c => c.CollectionItemCategories)
                .HasForeignKey(cic => cic.CategoryCode);

            modelBuilder.Entity<ArcaneVaultUser>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<CollectionItem>()
                .HasOne(ci => ci.User)
                .WithMany(u => u.CollectionItems)
                .HasForeignKey(ci => ci.UserName);

            // --- SEED DATA ---

            // Roles
            modelBuilder.Entity<ArcaneVaultUserRole>().HasData(
                new ArcaneVaultUserRole { RoleId = 1, RoleName = "Staff" },
                new ArcaneVaultUserRole { RoleId = 2, RoleName = "User" }
            );

            // Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryCode = "FIRE", CategoryName = "Fire Type" },
                new Category { CategoryCode = "WATER", CategoryName = "Water Type" },
                new Category { CategoryCode = "GRASS", CategoryName = "Grass Type" },
                new Category { CategoryCode = "ELECTRIC", CategoryName = "Electric Type" },
                new Category { CategoryCode = "PSYCHIC", CategoryName = "Psychic Type" },
                new Category { CategoryCode = "NORMAL", CategoryName = "Normal Type" },
                new Category { CategoryCode = "POISON", CategoryName = "Poison Type" },
                new Category { CategoryCode = "FLYING", CategoryName = "Flying Type" }
            );

            // Users
            var (staffHash, staffSalt) = ArcaneVaultAPI.Helpers.PasswordHelper.HashPassword("Password123!");
            var (u1Hash, u1Salt) = ArcaneVaultAPI.Helpers.PasswordHelper.HashPassword("Password123!");
            var (u2Hash, u2Salt) = ArcaneVaultAPI.Helpers.PasswordHelper.HashPassword("Password123!");
            var (u3Hash, u3Salt) = ArcaneVaultAPI.Helpers.PasswordHelper.HashPassword("Password123!");
            var (u4Hash, u4Salt) = ArcaneVaultAPI.Helpers.PasswordHelper.HashPassword("Password123!");

            modelBuilder.Entity<ArcaneVaultUser>().HasData(
                new ArcaneVaultUser { UserName = "staff1", Email = "staff1@arcanevault.test", IsDeleted = false, RoleId = 1, PasswordHash = staffHash, PasswordSalt = staffSalt },
                new ArcaneVaultUser { UserName = "user1", Email = "user1@arcanevault.test", IsDeleted = false, RoleId = 2, PasswordHash = u1Hash, PasswordSalt = u1Salt },
                new ArcaneVaultUser { UserName = "user2", Email = "user2@arcanevault.test", IsDeleted = false, RoleId = 2, PasswordHash = u2Hash, PasswordSalt = u2Salt },
                new ArcaneVaultUser { UserName = "user3", Email = "user3@arcanevault.test", IsDeleted = false, RoleId = 2, PasswordHash = u3Hash, PasswordSalt = u3Salt },
                new ArcaneVaultUser { UserName = "user4", Email = "user4@arcanevault.test", IsDeleted = false, RoleId = 2, PasswordHash = u4Hash, PasswordSalt = u4Salt }
            );

            // Collection Items — 10 items distributed per requirements
            modelBuilder.Entity<CollectionItem>().HasData(
                // staff1
                new CollectionItem { ItemId = 1, ItemName = "Charizard Base Set", IsDeleted = false, StartingQuantity = 5, CurrentQuantity = 3, UserName = "staff1", Price = 350.00m },
                new CollectionItem { ItemId = 2, ItemName = "Arcanine Promo", IsDeleted = false, StartingQuantity = 4, CurrentQuantity = 1, UserName = "staff1", Price = 45.00m },
                // user1
                new CollectionItem { ItemId = 3, ItemName = "Squirtle First Edition", IsDeleted = false, StartingQuantity = 6, CurrentQuantity = 6, UserName = "user1", Price = 120.00m },
                new CollectionItem { ItemId = 4, ItemName = "Gyarados Holo", IsDeleted = false, StartingQuantity = 8, CurrentQuantity = 2, UserName = "user1", Price = 60.00m },
                // user2
                new CollectionItem { ItemId = 5, ItemName = "Bulbasaur Base Set", IsDeleted = false, StartingQuantity = 3, CurrentQuantity = 3, UserName = "user2", Price = 40.00m },
                new CollectionItem { ItemId = 6, ItemName = "Venusaur EX", IsDeleted = false, StartingQuantity = 5, CurrentQuantity = 1, UserName = "user2", Price = 75.00m },
                // user3
                new CollectionItem { ItemId = 7, ItemName = "Pikachu Illustrator", IsDeleted = false, StartingQuantity = 2, CurrentQuantity = 2, UserName = "user3", Price = 5000.00m },
                new CollectionItem { ItemId = 8, ItemName = "Raichu Holo", IsDeleted = false, StartingQuantity = 4, CurrentQuantity = 0, UserName = "user3", Price = 30.00m },
                // user4
                new CollectionItem { ItemId = 9, ItemName = "Mewtwo GX", IsDeleted = false, StartingQuantity = 10, CurrentQuantity = 8, UserName = "user4", Price = 85.00m },
                new CollectionItem { ItemId = 10, ItemName = "Eevee Promo", IsDeleted = false, StartingQuantity = 5, CurrentQuantity = 5, UserName = "user4", Price = 15.00m }
            );

            // CollectionItemCategories — assign categories to items
            modelBuilder.Entity<CollectionItemCategory>().HasData(
                new CollectionItemCategory { ItemId = 1, CategoryCode = "FIRE" },
                new CollectionItemCategory { ItemId = 1, CategoryCode = "FLYING" }, // Charizard Dual Type
                new CollectionItemCategory { ItemId = 2, CategoryCode = "FIRE" },
                new CollectionItemCategory { ItemId = 3, CategoryCode = "WATER" },
                new CollectionItemCategory { ItemId = 4, CategoryCode = "WATER" },
                new CollectionItemCategory { ItemId = 4, CategoryCode = "FLYING" }, // Gyarados Dual Type
                new CollectionItemCategory { ItemId = 5, CategoryCode = "GRASS" },
                new CollectionItemCategory { ItemId = 5, CategoryCode = "POISON" }, // Bulbasaur Dual Type
                new CollectionItemCategory { ItemId = 6, CategoryCode = "GRASS" },
                new CollectionItemCategory { ItemId = 6, CategoryCode = "POISON" }, // Venusaur Dual Type
                new CollectionItemCategory { ItemId = 7, CategoryCode = "ELECTRIC" },
                new CollectionItemCategory { ItemId = 8, CategoryCode = "ELECTRIC" },
                new CollectionItemCategory { ItemId = 9, CategoryCode = "PSYCHIC" },
                new CollectionItemCategory { ItemId = 10, CategoryCode = "NORMAL" }
            );
        }


    }
}
