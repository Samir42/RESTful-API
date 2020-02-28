using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RESTfulApi_Reddit.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.DbContexts {
    public class RedditDbContext : IdentityDbContext<
            User,
            Role,
            int,
            IdentityUserClaim<int>,
            UserRole,
            IdentityUserLogin<int>,
            IdentityRoleClaim<int>,
            IdentityUserToken<int>> {
        public DbSet<CommunitiesPosts> CommunitiesPosts { get; set; }
        public DbSet<CommunitiesUsers> CommunitiesUsers { get; set; }
        public DbSet<CommunityPost> CommunityPosts { get; set; }
        public DbSet<Community> Communities { get; set; }
        public DbSet<UserPost> UserPosts { get; set; }

        public RedditDbContext(DbContextOptions<RedditDbContext> options)
          : base(options) {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // seed the database with dummy data

            modelBuilder.Entity<User>()
                .HasData(
                    new User() {
                        Id = 1,
                        Name = "Samir",
                        Surname = "Osmanov",
                        Email = "samir123@mail.ru",
                        EmailConfirmed = false,
                        PhoneNumberConfirmed = false,
                        TwoFactorEnabled = false,
                        LockoutEnabled = false,
                        AccessFailedCount = 3
                    },
                    new User() {
                        Id = 2,
                        Name = "Jhon",
                        Surname = "Skeet",
                        Email = "jhon123@mail.ru",
                        EmailConfirmed = false,
                        PhoneNumberConfirmed = false,
                        TwoFactorEnabled = false,
                        LockoutEnabled = false,
                        AccessFailedCount = 3
                    }, new User() {
                        Id = 3,
                        Name = "Lola",
                        Surname = "King",
                        Email = "lola123@mail.ru",
                        EmailConfirmed = false,
                        PhoneNumberConfirmed = false,
                        TwoFactorEnabled = false,
                        LockoutEnabled = false,
                        AccessFailedCount = 3
                    }, new User() {
                        Id = 4,
                        Name = "Stephen",
                        Surname = "Steph",
                        Email = "stephen123@mail.ru",
                        EmailConfirmed = false,
                        PhoneNumberConfirmed = false,
                        TwoFactorEnabled = false,
                        LockoutEnabled = false,
                        AccessFailedCount = 3
                    }, new User() {
                        Id = 5,
                        Name = "Larry",
                        Surname = "Page",
                        Email = "larry123@mail.ru",
                        EmailConfirmed = false,
                        PhoneNumberConfirmed = false,
                        TwoFactorEnabled = false,
                        LockoutEnabled = false,
                        AccessFailedCount = 3
                    }
                );

            modelBuilder.Entity<UserPost>()
                .HasData(new UserPost() {
                    Id = 1,
                    UserId = 1,
                    Title = "This post created By Samir",
                }, new UserPost() {
                    Id = 2,
                    UserId = 1,
                    Title = "This post created By Samir second time",
                    Text = "This is the best post ever"
                }, new UserPost() {
                    Id = 3,
                    UserId = 2,
                    Title = "This post created By Jhon",
                    Text = "This is the best post ever"
                }, new UserPost() {
                    Id = 4,
                    UserId = 2,
                    Title = "This post created By Samir second time",
                    Text = "This is the best post ever"
                },
                new UserPost() {
                    Id = 5,
                    UserId = 5,
                    Title = "This post created By Larry",
                },
                new UserPost() {
                    Id = 6,
                    UserId = 5,
                    Title = "This post created By Larry second time",
                });

            modelBuilder.Entity<CommunitiesPosts>()
                .HasOne(x => x.Community)
                .WithMany(x => x.CommunitiesPosts)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .Property(p => p.RegisteredAt)
                .HasDefaultValue(DateTimeOffset.Now);

            modelBuilder.Entity<CommunityPost>()
                .Property(x => x.CreatedAt)
                .HasDefaultValue(DateTimeOffset.Now);

            modelBuilder.Entity<UserPost>()
                .Property(x => x.CreatedAt)
                .HasDefaultValue(DateTimeOffset.Now);

            base.OnModelCreating(modelBuilder);
        }
    }
}
