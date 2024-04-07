using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.UI.Xaml.Media.Imaging;
using LhjMusicPlayer.Common;
using LhjMusicPlayer.Models.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace LhjMusicPlayer.Models
{
    [Table("musics")]
    [Comment("音乐库")]
    [Index(nameof(FilePath),IsUnique = true)]
    [EntityTypeConfiguration(typeof(MusicEntityTypeConfiguration))]
    [method: SetsRequiredMembers]
    public class Music(string title, string filePath)
    {
        [Column(Order = 0)]
        public int Id { get; set; }

        [Column(Order = 1), Comment("歌名")]
        public required string Title { get; set; } = title;

        [Column(Order = 2), Comment("歌手")]
        public string? Artist { get; set; }

        [Column(Order = 3), Comment("专辑名")]
        public string? Album { get; set; }

        [Column(Order = 4), Comment("时长（秒）")]
        public double Time { get; set; }

        [Column(Order = 5), Comment("专辑图")]
        public byte[]? AlbumImage { get; set; }

        [Column(Order = 6), Required, MaxLength(200), Comment("文件路径")]
        public required string FilePath { get; set; } = filePath;
        [Column(Order = 7), Comment("歌词路径")]
        public string? LyricFilePath { get; set; } = null;

        [method: SetsRequiredMembers]
        public Music(int id, string title, string artist, string album, long time, byte[] albumImage, string filePath) : this(title, filePath)
        {
            Id = id;
            Title = title;
            Artist = artist;
            Album = album;
            Time = time;
            AlbumImage = albumImage;
            FilePath = filePath;
        }

        public List<MusicList> MusicLists { get; set; } = [];

        public static List<Music> SearchMusicsFromFolder(string folderPath)
        {
            using var context = new DataContext();
            var list = new List<Music>();
            var files_path = Directory.GetFiles(folderPath);

            foreach (var file_path in files_path)
            {
                if(File.Exists(file_path))
                {
                    var music = MediaInfo.GetMusic(file_path);
                    if(music != null)
                    {
                        var lrcPath = Path.Combine(Path.GetDirectoryName(file_path)!, Path.GetFileNameWithoutExtension(file_path) + ".lrc");
                        if(File.Exists(lrcPath))
                        {
                            music.LyricFilePath = lrcPath;
                        }
                        list.Add(music);
                    }
                }
            }
            return list;
        }
        public static void LoadLocalMusics(string folderPath)
        {
            using var context = new DataContext();
            var files_path = Directory.GetFiles(folderPath);
            var local = context.MusicList.Single(list => list.Title == "local");
            var love = context.MusicList.Single(list => list.Title == "loving");
            var playing = context.MusicList.Single(list => list.Title == "playing");
            foreach (var file_path in files_path)
            {
                if (context.Musics.SingleOrDefault(m => m.FilePath == file_path) != null)
                    continue;
                if (File.Exists(file_path))
                {
                    var music = MediaInfo.GetMusic(file_path);
                    if (music != null)
                    {

                        var lrcPath = Path.Combine(Path.GetDirectoryName(file_path)!, Path.GetFileNameWithoutExtension(file_path) + ".lrc");
                        if (File.Exists(lrcPath))
                        {
                            music.LyricFilePath = lrcPath;
                        }
                        context.Musics.Add(music);
                        local.Musics.Add(music);
                        love.Musics.Add(music);
                        playing.Musics.Add(music);
                    }
                }
            }
            context.SaveChanges();
        }
    }
    public class MusicEntityTypeConfiguration : IEntityTypeConfiguration<Music>
    {
        public void Configure(EntityTypeBuilder<Music> builder)
        {
            builder
                .HasMany(music => music.MusicLists)
                .WithMany(list => list.Musics)
                .UsingEntity<MusicMusicList>();


            //var folderPath = @"C:\Users\89422\source\repos\GetStartedApp\GetStartedApp\Assets\music";
            //var files_path = Directory.GetFiles(folderPath);
            //List<Music> list = [];

            //foreach (var file_path in files_path)
            //{
            //    var item = MediaInfo.GetMusic(file_path);
            //    item.Id = list.Count + 1;
            //    list.Add(item);
            //}
            //builder.HasData(list);
        }
    }
}
