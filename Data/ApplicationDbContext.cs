using ApiProduct.Mdoels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProduct.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser,ApplicationRole,string>
        
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=AppUser;Integrated Security=True;");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            const string ADMIN_ID = "8005b0dc-ae8f-478d-9f97-b2851d9511c5";
            const string ROLE_ID = ADMIN_ID;
            base.OnModelCreating(builder);
            builder.Entity<ApplicationRole>().HasData(new ApplicationRole
            {
                Id = ROLE_ID,
                Name = "Admin",
                NormalizedName = "ADMIN",

            }
            );
            builder.Entity<ApplicationUser>().HasData(new
            {


                Id = ADMIN_ID,
                UserName = "alinouriare",
                AccessFailedCount = 0,
                NormalizedUserName = "ALINOURIARE",
                Email = "alinouriare@yahoo.com",
                NormalizedEmail = "ALINOURIARE@YAHOO.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAEAACcQAAAAEMDYhBQ5TSFRxozAE0HvCLWJBOIidn2wg7DhIBLNlw0tNxHaBhrJEY/U1UpvM/QHIg==",
                SecurityStamp = "2635MHXDXFOW52EX2CNWUIUI3E63H3WY",
                ConcurrencyStamp = "3a082ba6-d7bb-4bfd-b4ad-e9cb130822f5",
                LockoutEnabled = true,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                FirstName = "Ali",
                LastName = "Nouri",
                PhoneNumber = "09359504672"


            });
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {

                RoleId = ROLE_ID,
                UserId = ADMIN_ID

            });
        }
    }
}
