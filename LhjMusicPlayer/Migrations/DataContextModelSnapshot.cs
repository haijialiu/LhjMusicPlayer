﻿// <auto-generated />
using System;
using LhjMusicPlayer.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LhjMusicPlayer.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.3");

            modelBuilder.Entity("LhjMusicPlayer.Models.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnOrder(0);

                    b.Property<string>("Content")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(1);

                    b.Property<DateTime>("CreatedTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnOrder(3)
                        .HasDefaultValueSql("Datetime(CURRENT_TIMESTAMP,'localtime')");

                    b.Property<int>("MusicId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(2);

                    b.HasKey("Id");

                    b.HasIndex("MusicId");

                    b.ToTable("CommentList", t =>
                        {
                            t.HasComment("评论");
                        });
                });

            modelBuilder.Entity("LhjMusicPlayer.Models.Music", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnOrder(0);

                    b.Property<string>("Album")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(3)
                        .HasComment("专辑名");

                    b.Property<byte[]>("AlbumImage")
                        .HasColumnType("BLOB")
                        .HasColumnOrder(5)
                        .HasComment("专辑图");

                    b.Property<string>("Artist")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(2)
                        .HasComment("歌手");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT")
                        .HasColumnOrder(6)
                        .HasComment("文件路径");

                    b.Property<string>("LyricFilePath")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(7)
                        .HasComment("歌词路径");

                    b.Property<double>("Time")
                        .HasColumnType("REAL")
                        .HasColumnOrder(4)
                        .HasComment("时长（秒）");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnOrder(1)
                        .HasComment("歌名");

                    b.HasKey("Id");

                    b.HasIndex("FilePath")
                        .IsUnique();

                    b.ToTable("musics", t =>
                        {
                            t.HasComment("音乐库");
                        });
                });

            modelBuilder.Entity("LhjMusicPlayer.Models.MusicList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnOrder(0);

                    b.Property<DateTime>("CreatedTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnOrder(4)
                        .HasDefaultValueSql("Datetime(CURRENT_TIMESTAMP,'localtime')");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnOrder(2);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnOrder(1);

                    b.Property<string>("Type")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("user")
                        .HasColumnOrder(3);

                    b.HasKey("Id");

                    b.ToTable("MusicList");
                });

            modelBuilder.Entity("LhjMusicPlayer.Models.MusicMusicList", b =>
                {
                    b.Property<int>("MusicId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MusicListId")
                        .HasColumnType("INTEGER");

                    b.HasKey("MusicId", "MusicListId");

                    b.HasIndex("MusicListId");

                    b.ToTable("MusicMusicLists");
                });

            modelBuilder.Entity("LhjMusicPlayer.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnOrder(0);

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnOrder(1)
                        .HasComment("用户名");

                    b.HasKey("Id");

                    b.ToTable("user", t =>
                        {
                            t.HasComment("用户表");
                        });
                });

            modelBuilder.Entity("LhjMusicPlayer.Models.Comment", b =>
                {
                    b.HasOne("LhjMusicPlayer.Models.Music", "Music")
                        .WithMany()
                        .HasForeignKey("MusicId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Music");
                });

            modelBuilder.Entity("LhjMusicPlayer.Models.MusicMusicList", b =>
                {
                    b.HasOne("LhjMusicPlayer.Models.Music", null)
                        .WithMany()
                        .HasForeignKey("MusicId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LhjMusicPlayer.Models.MusicList", null)
                        .WithMany()
                        .HasForeignKey("MusicListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
