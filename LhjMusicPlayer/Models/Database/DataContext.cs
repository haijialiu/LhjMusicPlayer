using Microsoft.EntityFrameworkCore;
using LhjMusicPlayer.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LhjMusicPlayer.Models.Database
{
    public sealed class DataContext : DbContext
    {
        public DbSet<Music> Musics { get; set; }
        public DbSet<MusicList> MusicList { get; set; }
        public DbSet<MusicMusicList> MusicMusicLists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //string dbPath = Path.Combine(Path.GetDirectoryName(Environment.CurrentDirectory), "local.sqlite");
            string dbPath = @"C:\Users\haijialiu\source\repos\LhjMusicPlayer\LhjMusicPlayer\Models\Database\local.sqlite";
            string connectionString = $"Data Source={dbPath}";
            optionsBuilder.UseSqlite(connectionString);
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MusicMusicList>();
        }
    }
}