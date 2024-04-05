using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LhjMusicPlayer.Models.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LhjMusicPlayer.Models
{
    [EntityTypeConfiguration(typeof(MusicListEntityTypeConfiguration))]
    public partial class MusicList : ObservableObject
    {
        [Column(Order = 0)]
        public int Id { get; set; }
        [Required, Column(Order = 1)]
        public required string Title { get; set; }
        [Required, Column(Order = 2)]
        public required string Name { get; set; }
        [Required, Column(Order = 3)]
        public string Type { get; set; } = "user";
        [Column(Order = 4)]
        public DateTime CreatedTime { get; set; }

        public List<Music> Musics { get; set; } = [];

        public void RemoveMusic(int MusicListId,int MusicId)
        {
            using var context = new DataContext();
            context.MusicMusicLists.Remove(new MusicMusicList() { MusicListId = MusicListId,MusicId=MusicId });
            context.SaveChanges();
            Musics.RemoveAt(MusicId);
        }
    }
    public class MusicListEntityTypeConfiguration : IEntityTypeConfiguration<MusicList>
    {
        public void Configure(EntityTypeBuilder<MusicList> builder)
        {

            builder
                .Property(musicList => musicList.CreatedTime)
                .HasDefaultValueSql("Datetime(CURRENT_TIMESTAMP,'localtime')");
            builder.Property(musicList => musicList.Type)
                .HasDefaultValue("user");

            builder
                .HasMany(musicList => musicList.Musics)
                .WithMany(music => music.MusicLists)
                .UsingEntity<MusicMusicList>();

            //builder.HasData(new MusicList()
            //{
            //    Id = 1,
            //    Title = "default",
            //    Name = "默认列表",
            //});
        }
    }
}
