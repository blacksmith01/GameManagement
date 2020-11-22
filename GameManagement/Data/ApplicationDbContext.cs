using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;

namespace GameManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AppUser.ExtraInfo> Extras { get; set; }
        public DbSet<AppUser.History> Histories { get; set; }
        public DbSet<DBConfig.System> System { get; set; }
        public DbSet<StatData.Daily> DailyStats { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AppUser.ExtraInfo>().HasOne<IdentityUser>().WithOne().HasForeignKey<AppUser.ExtraInfo>(x => x.ID);
        }

        public async Task<bool> Initialize()
        {
            bool isChanged = Database.EnsureCreated();

            var newRoles = AppUser.Role.Names.Where(n => !Roles.Where(r => string.Equals(n, r.Name)).Any()).Select(n => new IdentityRole(n)).ToList();
            if(newRoles.Any())
            {
                await Roles.AddRangeAsync(newRoles);
                isChanged = true;
            }

            if (!Users.Any())
            {
                if (!await CreateUser(AppUser.DefaultAdminUserName, AppUser.DefaultAdminUserPassword, "", true))
                {
                    return false;
                }

                var adminUser = await Users.Where(u => u.UserName == AppUser.DefaultAdminUserName).FirstOrDefaultAsync();
                var adminRole = await Roles.Where(r => r.Name == AppUser.Role.Admin).FirstOrDefaultAsync();
                await UserRoles.AddAsync(new IdentityUserRole<string>
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                });
                isChanged = true;
            }

            if (!System.Any())
            {
                await System.AddAsync(new DBConfig.System
                {
                    Email = new DBConfig.System.EmailServer()
                });
                isChanged = true;
            }

            if (isChanged)
            {
                await SaveChangesAsync();
            }

            AppUser.Role.IdNameMap = await Roles.Where(r => AppUser.Role.Names.Contains(r.Name)).ToDictionaryAsync(r => r.Id, r=>r.Name);
            AppUser.Role.NameIdMap = AppUser.Role.IdNameMap.ToDictionary(p => p.Value, p => p.Key);

            return true;
        }
        public async Task<bool> CreateUser(string userName, string password, string email, bool emailConfirmed = false)
        {
            var user = new IdentityUser { UserName = userName, Email = email, EmailConfirmed = emailConfirmed };
            var result = await this.GetService<UserManager<IdentityUser>>().CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return false;
            }

            await Extras.AddAsync(new AppUser.ExtraInfo { ID = user.Id, RegisterTime = DateTime.Now });

            return true;
        }

        public async Task<bool> AddHistory(string userID, string userName, AppUser.History.Types typeid, string detail = default)
        {
            await Histories.AddAsync(new AppUser.History
            {
                UserID = userID,
                UserName = userName,
                Time = DateTime.Now,
                TypeID = typeid,
                Detail = detail
            });

            return true;
        }
    }
}
