using GameManagement.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GameManagement.Services
{
    public class AppUserHistoryWriter
    {
        IServiceScopeFactory ssf;
        public AppUserHistoryWriter(IServiceScopeFactory ssf)
        {
            this.ssf = ssf;
        }

        public async Task Write(AppUser.History.Types typeid, object obj = null)
        {
            using (var scope = ssf.CreateScope())
            {
                var asp = scope.ServiceProvider.GetService<AuthenticationStateProvider>();
                var user = await asp.CurrentUser();
                if (user != null)
                {
                    using (var ctx = scope.ServiceProvider.GetService<ApplicationDbContext>())
                    {
                        await Write(ctx, user, typeid, obj);
                        await ctx.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task Write(string username, AppUser.History.Types typeid, object obj = null)
        {
            using (var scope = ssf.CreateScope())
            using (var ctx = scope.ServiceProvider.GetService<ApplicationDbContext>())
            {
                var user = await ctx.Users.Where(u => u.UserName == username).FirstOrDefaultAsync();
                if (user == null)
                    return;
                await Write(ctx, user, typeid, obj);
                await ctx.SaveChangesAsync();
            }
        }
        public async Task Write(ClaimsPrincipal user, AppUser.History.Types typeid, object obj = null)
        {
            using (var scope = ssf.CreateScope())
            using (var ctx = scope.ServiceProvider.GetService<ApplicationDbContext>())
            {
                await Write(ctx, user, typeid, obj);
                await ctx.SaveChangesAsync();
            }
        }
        public async Task Write(IdentityUser user, AppUser.History.Types typeid, object obj = null)
        {
            using (var scope = ssf.CreateScope())
            using (var ctx = scope.ServiceProvider.GetService<ApplicationDbContext>())
            {
                await Write(ctx, user, typeid, obj);
                await ctx.SaveChangesAsync();
            }
        }
        public async Task Write(ApplicationDbContext ctx, ClaimsPrincipal user, AppUser.History.Types typeid, object obj = null)
        {
            await Write(ctx, user.GetID(), user.Identity.Name, typeid, obj);
        }
        public async Task Write(ApplicationDbContext ctx, IdentityUser user, AppUser.History.Types typeid, object obj = null)
        {
            await Write(ctx, user.Id, user.UserName, typeid, obj);
        }
        public async Task Write(ApplicationDbContext ctx, string userid, string username, AppUser.History.Types typeid, object obj = null)
        {
            await ctx.AddHistory(userid, username, typeid, obj != null ? JsonConvert.SerializeObject(obj, Formatting.None) : null);
        }
    }
}
