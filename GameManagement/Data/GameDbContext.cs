using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameManagement.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GameUser>()
                .HasMany(x => x.Blocks).WithOne().HasForeignKey(x=>x.userid);
            modelBuilder.Entity<GameUser>()
                .HasMany(x => x.Goodses).WithOne().HasForeignKey(x => x.userid);
            modelBuilder.Entity<GameUser>()
                .HasMany(x => x.Mails).WithOne().HasForeignKey(x => x.userid);
            modelBuilder.Entity<GameUser>()
                .HasMany(x => x.Chars).WithOne().HasForeignKey(x => x.userid);

            modelBuilder.Entity<GameUser.Goods>()
            .HasKey(p => new { p.id, p.userid });
        }

        public DbSet<AgentEvent> agent_events { get; set; }
        public DbSet<GameServer> game_servers { get; set; }
        public DbSet<GameServer.Notice> game_notices { get; set; }
        public DbSet<GameUser> game_users { get; set; }
        public DbSet<GameUser.Mail> game_mails { get; set; }
        public DbSet<GameUser.Block> game_userblocks { get; set; }
        public DbSet<GameUser.Goods> game_useritems { get; set; }
        public DbSet<GameChar> game_chars { get; set; }
        public DbSet<GameChar.Skill> game_charskills { get; set; }
        public DbSet<GameChar.Item> game_charitems { get; set; }
        public DbSet<GameLog> game_logs { get; set; }
    }
}
