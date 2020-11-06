using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PracticeChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PracticeChat.Database
{
    public class dataContext : IdentityDbContext
    {
        public dataContext(DbContextOptions<dataContext> options)
            : base(options)
        {
        }
        public DbSet<OneToOne> oneToOnes { get; set; }
        public DbSet<CreateGroup> createGroups { get; set; }
        public DbSet<GroupMeassage> groupMeassages { get; set; }
        public DbSet<ImageInfo> imageInfos { get; set; }
        public DbSet<FriendList> friendLists { get; set; }
        public DbSet<GroupInfo> groupInfos  { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            const string USER_ID = "8544ra-aa75-4af8-bd17-00rwrd9344e575";
            var hasher = new PasswordHasher<IdentityUser>();

            builder.Entity<IdentityUser>().HasData(new IdentityUser
            {
                Id = USER_ID,
                UserName = "kshkdsf",
                NormalizedUserName = "user",
                Email = "user@gmail.com",
                NormalizedEmail = "USER@gmail.com",
                EmailConfirmed = false,
                PasswordHash = hasher.HashPassword(null, "user"),
                AccessFailedCount = 0,
                PhoneNumber = "0189102812",
            });
        }
    }
}

